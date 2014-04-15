// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Vido.Media;
  using Vido.Qms.Exceptions;

  public class GateController : IObservable<EntryArgs>
  {
    private readonly List<TaskState> tasks;
    private readonly ControllerServices services;
    private readonly IInputDeviceList inputList;
    private readonly IUniqueIdStorage idStorage;
    private readonly IEntryRecorder recorder;

    public GateController(
      ControllerServices services,
      IInputDeviceList inputList,
      IUniqueIdStorage idStorage,
      IEntryRecorder recorder)
    {
      this.tasks = new List<TaskState>();
      this.services = services;
      this.inputList = inputList;
      this.idStorage = idStorage;
      this.recorder = recorder;
    }

    public IDisposable Subscribe(IObserver<EntryArgs> observer)
    {
      var gate = observer as IGate;
      if (gate == null)
        return (null);

      if (!tasks.Exists((t) => t.Gate == gate))
      {
        /// Đăng kí nhận dữ liệu đến từ Input.
        gate.Input.DataIn += Input_DataIn;

        /// Đăng kí input để bắt đầu nhận dữ liệu.
        gate.Input.Deregister = inputList.Register(gate.Input);

        tasks.Add(new TaskState(gate, WorkerThread));
      }

      return (new Deregister<IGate>(gate, (g) =>
      {
        var task = tasks.Find((t) => t.Gate == g);
        if (task != null)
        {
          /// Hủy đăng kí input.
          g.Input.Deregister.Dispose();
          g.Input.Deregister = null;

          /// Hủy đăng kí nhận dữ liệu trên input.
          g.Input.DataIn -= Input_DataIn;

          /// Đóng task (Bật event TaskStop và chờ Task kết thúc)
          task.Close();

          /// Bỏ task ra khỏi danh sách.
          tasks.Remove(task);
        }
      }));
    }

    #region Private Methods
    private void Input_DataIn(object sender, EventArgs e)
    {
      var args = e as DataInEventArgs;

      /// Duyệt qua các task...
      foreach (var task in tasks)
      {
        /// Tìm task của cổng có input từ thiết bị.
        if (task.Gate.Input == sender)
        {
          var gate = task.Gate;

          if (gate.State == GateState.Closed)
          {
            gate.OnError(new GateClosedException());
            continue;
          }

          /// Chụp ảnh đối tượng truy cập.
          var images = gate.CaptureImage();

          if (images == null)
          {
            gate.OnError(new CaptureImageFailedException());
            continue;
          }

          var entryArgs = new EntryArgs()
          {
            /// Cổng có truy cập.
            Gate = gate,

            /// Lấy định danh duy nhất, mã hóa base64 cho nó để có dạng có thể in.
            /// Ngoài ra, cũng có thể random Id ở đây, bằng cách override method.
            UniqueId = services.GetUniqueId(args.Data, args.Printable),

            /// Lấy dữ liệu người dùng, có thể override để xử lý Ocr nhận diện biển số,
            /// Hoặc nhận diện khuôn mặt rồi trả về tên...
            UserData = services.GetUserData(images),

            /// Ảnh chụp đối tượng
            Images = images
          };

          /// Cho vào hàng đợi của cổng, chờ xử lý. (^_^).
          task.Entries.Enqueue(entryArgs);
        }
      }
    }

    private void WorkerThread(TaskState state)
    {
      var gate = state.Gate;
      var entries = state.Entries;
      var stopTask = state.TaskStop;
      var entryAllow = state.EntryAllow;
      var entryBlock = state.EntryBlock;

      EntryArgs cur = null;
      while (!stopTask.WaitOne(10))
      {
        /// TODO: Review-Logic
        /// Cổng đang đóng, dừng xử lý.
        /// TODO: Nên thêm phương thức xóa hàng đợi?!
        /// ISSUES: Thêm vào đâu? Xử lý thế nào? @@
        if (gate.State == GateState.Closed)
        {
          continue;
        }

        try
        {
          /// Lấy phần tử từ hàng đợi.
          if (entries.TryDequeue(out cur, 0))
          {
            /// Thông báo cho cổng biết có đối tượng vào.
            gate.OnNext(cur);

            var uniqueId = cur.UniqueId.UniqueId;

            /// Kiểm tra định danh duy nhất.
            if (idStorage.CanUse(uniqueId))
            {
              if (gate.Direction == Direction.Import)
              {
                #region Import Processes
                if (recorder.IsFull)
                {
                  /// Bộ ghi đã đầy.
                  gate.OnError(new RecorderFullException());
                }
                else
                {
                  /// Chờ sự cho phép để đối tượng vào cổng
                  /// Trong thời gian chờ cho phép,
                  /// người dùng có thể thay đổi thông tin.
                  bool allow = services.EntryRequest(entryBlock, entryAllow, entries.NewItems, gate.TimeoutDisplay);

                  /// Chặn sự thay đổi từ người dùng từ thời điểm này.
                  /// Sao chép chuỗi, DotNet ngu ngốc,
                  /// không dùng được method string.Copy() (-_-).
                  var userData = cur.UserData.UserData.Substring(0);
                  var entry = new Entry()
                  {
                    EntryGate = gate.Name,
                    EntryTime = DateTime.Now,
                    UniqueId = uniqueId,
                    UserData = userData
                  };

                  /// Lưu ảnh xuống nơi chứa,
                  /// phụ thuộc vào cài đặt của service.
                  if (services.SaveImage(cur.Images, entry, gate.Direction))
                  {
                    /// Thông tin truyền cho cổng.
                    IImport import;

                    /// Kiểm tra đối tượng được phép vào cổng hay không.
                    if (allow && recorder.CanImport(uniqueId, userData, out import))
                    {
                      /// Được phép, ghi lại thông tin vào cổng.
                      if (recorder.Import(entry))
                      {
                        /// Hiển thị thông in vào.
                        gate.ImportDisplay(import);

                        /// Thông báo là có đối tượng được phép vào.
                        gate.OnCompleted();

                        /// TODO: Add Printer
                        if (gate.Printer != null)
                        {
                          gate.Printer.Print(services.ToPrintString(entry), services.PrintEncoding);
                        }
                      }
                      else
                      {
                        /// Ghi dữ liệu thất bại. (T_T)
                        gate.OnError(new WriteDataFailedException());
                      }
                    }
                    else
                    {
                      /// Ghi lại thông tin của đối tượng bị chặn.
                      recorder.Blocked(entry);

                      /// Thông báo chặn đối tượng,
                      /// không cho phép vào cổng.
                      gate.OnError(new EntryBlockedException());
                    }
                  }
                  else
                  {
                    /// Lưu ảnh thất bại. (T_T)
                    gate.OnError(new SaveImageFailedException());
                  }
                }
                #endregion
              }
              else
              {
                #region Export Processes
                /// Chặn sự thay đổi từ người dùng từ thời điểm này.
                /// Sao chép chuỗi, DotNet ngu ngốc,
                /// không dùng được method string.Copy() (-_-).
                var userData = cur.UserData.UserData.Substring(0);
                var entry = new Entry()
                {
                  EntryGate = gate.Name,
                  EntryTime = DateTime.Now,
                  UniqueId = uniqueId,
                  UserData = userData
                };

                /// Lưu ảnh xuống nơi chứa,
                /// phụ thuộc vào cài đặt của service.
                if (services.SaveImage(cur.Images, entry, gate.Direction))
                {
                  IExport export;

                  /// Kiểm tra đối tượng xem đối tượng có được phép ra hay không.
                  if (recorder.CanExport(uniqueId, userData, out export))
                  {
                    if (!services.ImageRoot.Exists(export.FirstImage) ||
                      !services.ImageRoot.Exists(export.SecondImage))
                    {
                      gate.OnError(new NotFoundImportImageException());
                    }
                    else
                    {
                      /// Tao đ' cần biết mày làm thế nào,
                      /// tao đưa đường dẫn đấy. Tự mà load ảnh. (-_-)
                      gate.SavedImage(services.ImageRoot, export.FirstImage, export.SecondImage);

                      /// Chờ sự cho phép đối tượng ra của người dùng.
                      if (services.EntryRequest(entryBlock, entryAllow, entries.NewItems, gate.TimeoutDisplay))
                      {
                        /// Ghi lại thông tin đối tượng ra.
                        if (recorder.Export(entry))
                        {
                          /// Hiển thị thông in ra.
                          gate.ExportDisplay(export);

                          /// Thông báo cho cổng biết có đối tượng ra.
                          gate.OnCompleted();

                          /// TODO: Add Printer
                          if (gate.Printer != null)
                          {
                            gate.Printer.Print(services.ToPrintString(entry), services.PrintEncoding);
                          }
                        }
                        else
                        {
                          /// Ghi dữ liệu thất bại. (T_T)
                          gate.OnError(new WriteDataFailedException());
                        }
                      }
                      else
                      {
                        /// Ghi lại thông tin đối tượng bị chặn.
                        recorder.Blocked(entry);

                        /// Báo cho cổng chặn đối tượng.
                        gate.OnError(new EntryBlockedException());
                      }
                    }
                  }
                  else
                  {
                    /// Ghi lại thông tin đối tượng bị chặn.
                    recorder.Blocked(entry);

                    /// Báo cho cổng chặn đối tượng.
                    gate.OnError(new EntryBlockedException());
                  }
                }
                else
                {
                  /// Lưu ảnh thất bại. (T_T)
                  gate.OnError(new SaveImageFailedException());
                }
                #endregion
              }
            }
            else
            {
              /// Định danh duy nhất không hợp lệ,
              /// không thể sử dụng, không tồn tại...
              gate.OnError(new InvalidUniqueIdException());
            }
          }
        }
        catch (Exception ex)
        {
          /// Bẫy lỗi hệ thống, thông báo cho cổng. (T_T)
          gate.OnError(new SystemErrorException(ex));
        }
        finally
        {
          /// Reset lại trạng thái của Event.
          entryAllow.Reset();
          entryBlock.Reset();
        }
      }
    }
    #endregion
  }
}