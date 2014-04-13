// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  using System;
  using System.Text;
  using System.Threading;
  using Vido.Media;

  public class ControllerServices
  {
    public IDailyDirectory ImageRoot { get; set; }
    public int EntryRequestTimeout { get; set; }
    public string ImportString { get; set; }
    public string ExportString { get; set; }


    /// <summary>
    /// {EntryTime} Thời điểm vào cổng,
    /// {EntryGate} Tên cổng vào,
    /// {Direction} Hướng di chuyển (vào/ra),
    /// {UniqueId} Định danh duy nhất,
    /// {UserData} Dữ liệu người dùng,
    /// {ImgIndex} Chỉ số ảnh
    ///</summary>
    public string ImageNameFormat { get; set; }

    public ControllerServices()
    {
      this.ImageRoot = null;
      this.EntryRequestTimeout = 15000;
      this.ImportString = "IM";
      this.ExportString = "EX";
      this.ImageNameFormat = "IMG_{EntryTime:HHmmss}{EntryGate}{Direction}_{UniqueId}_{UserData}{ImgIndex}.jpg";
    }

    public virtual IUniqueId GetUniqueId(byte[] uniqueId, bool printable)
    {
      return (new UniqueId(printable ? GetAscii(uniqueId) : Convert.ToBase64String(uniqueId)));
    }
    private string GetAscii(byte[] bytes)
    {
      StringBuilder sb = new StringBuilder();
      foreach (var b in bytes)
      {
        sb.Append(Convert.ToChar(b));
      }

      return (sb.ToString());
    }
    public virtual IUserData GetUserData(ImagePair image)
    {
      return (new UserData(string.Empty));
    }

    public virtual bool EntryRequest(
      EventWaitHandle entryBlock,
      EventWaitHandle entryAllow,
      EventWaitHandle newEntries)
    {
      bool allow = true;

      for (int t = EntryRequestTimeout; t > 0; t -= 1000)
      {
        if (entryBlock.WaitOne(500))
        {
          allow = false;
          break;
        }

        if (newEntries.WaitOne(0))
        {
          break;
        }

        if (entryAllow.WaitOne(500))
        {
          break;
        }
      }

      return (allow);
    }

    public virtual bool SaveImage(ImagePair image, Entry entry, Direction direction)
    {
      var imEx = direction == Direction.Import ? ImportString : ExportString;
      if (image.First != null && image.First.Available)
      {
        var formatArgs = new
        {
          EntryTime = entry.EntryTime,
          EntryGate = entry.EntryGate,
          Direction = imEx,
          UniqueId = entry.UniqueId,
          UserData = entry.UserData,
          ImgIndex = 0
        };

        var path = ImageRoot.GetPath(entry.EntryTime, ImageNameFormat.NamedFormat(formatArgs));

        if (image.First.Save(ImageRoot, path))
        {
          entry.FirstImage = path;
        }
      }

      if (image.Second != null && image.Second.Available)
      {
        var formatArgs = new
        {
          EntryTime = entry.EntryTime,
          EntryGate = entry.EntryGate,
          Direction = imEx,
          UniqueId = entry.UniqueId,
          UserData = entry.UserData,
          ImgIndex = 1
        };

        var path = ImageRoot.GetPath(entry.EntryTime, ImageNameFormat.NamedFormat(formatArgs));

        if (image.Second.Save(ImageRoot, path))
        {
          entry.SecondImage = path;
        }
      }

      return (true);
    }
  }
}