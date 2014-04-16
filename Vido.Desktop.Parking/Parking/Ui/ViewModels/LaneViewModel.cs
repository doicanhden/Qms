namespace Vido.Parking.Ui.ViewModels
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Windows.Input;
  using System.Windows.Media.Imaging;
  using System.Windows.Threading;
  using Vido.Media;
  using Vido.Media.Capture;
  using Vido.Parking.Ui.Commands;
  using Vido.Qms;
  using Vido.Utilities;

  public class LaneViewModel : NotificationObject, IGate
  {
    #region Data Members

    private string name = null;
    private string message = null;
    private IUniqueId uniqueId = null;
    private IUserData userData = null;
    private BitmapSource backImageSaved = null;
    private BitmapSource frontImageSaved = null;
    private BitmapSource frontImageCamera = null;
    private BitmapSource backImageCamera = null;

    private ICommand blockCommand = null;
    private ICommand allowCommand = null;
    private ICapture cameraFirst;
    private ICapture cameraSecond;
    private string allowButtonText;
    private string blockButtonText;
    private DispatcherTimer timer;
    private int secondsTimeout = 0;
    private bool allowButtonEnable = false;
    private bool blockButtonEnable = false;
    private GateState state;
    #endregion

    #region Public Commnands
    public ICommand AllowCommand
    {
      get
      {
        return (allowCommand ?? (allowCommand =
          new RelayCommand((x) => { Allow.Set(); })));
      }
    }

    public ICommand BlockCommand
    {
      get
      {
        return (blockCommand ?? (blockCommand =
          new RelayCommand((x) => { Block.Set(); })));
      }
    }
    #endregion

    #region Public Properties (for Binding)
    public string Name
    {
      get { return (name); }
      set
      {
        name = value;
        RaisePropertyChanged(() => Name);
      }
    }
    public string Message
    {
      get { return (message); }
      set
      {
        message = value;
        RaisePropertyChanged(() => Message);
      }
    }

    public string UniqueId
    {
      get { return (uniqueId.UniqueId); }
      set
      {
        uniqueId.UniqueId = value;
        RaisePropertyChanged(() => UniqueId);
      }
    }
    public string UserData
    {
      get { return (userData.UserData); }
      set
      {
        userData.UserData = value;
        RaisePropertyChanged(() => UserData);
      }
    }

    public BitmapSource CameraImageBack
    {
      get { return (backImageCamera); }
      set
      {
        backImageCamera = value;
        RaisePropertyChanged(() => CameraImageBack);
      }
    }
    public BitmapSource CameraImageFront
    {
      get { return (frontImageCamera); }
      set
      {
        frontImageCamera = value;
        RaisePropertyChanged(() => CameraImageFront);
      }
    }
    public BitmapSource SavedImageBack
    {
      get { return (backImageSaved); }
      set
      {
        backImageSaved = value;
        RaisePropertyChanged(() => SavedImageBack);
      }
    }
    public BitmapSource SavedImageFront
    {
      get { return (frontImageSaved); }
      set
      {
        frontImageSaved = value;
        RaisePropertyChanged(() => SavedImageFront);
      }
    }

    public string AllowButtonText
    {
      get { return (allowButtonText); }
      set
      {
        allowButtonText = value;
        RaisePropertyChanged(() => AllowButtonText);
      }
    }
    public bool AllowButtonEnable
    {
      get { return (allowButtonEnable); }
      set
      {
        allowButtonEnable = value;
        RaisePropertyChanged(() => AllowButtonEnable);
      }
    }

    public string BlockButtonText
    {
      get { return (blockButtonText); }
      set
      {
        blockButtonText = value;
        RaisePropertyChanged(() => BlockButtonText);
      }
    }
    public bool BlockButtonEnable
    {
      get { return (blockButtonEnable); }
      set
      {
        blockButtonEnable = value;
        RaisePropertyChanged(() => BlockButtonEnable);
      }
    }
    #endregion

    #region Public Properties
    public GateState State
    {
      get { return (state); }
      set
      {
        state = value;
        RaisePropertyChanged(() => State);
      }
    }
    public Direction Direction { get; set; }

    public IDisposable Deregister { get; set; }

    public EventWaitHandle Allow { get; set; }
    public EventWaitHandle Block { get; set; }

    public ICapture CameraFirst
    {
      get { return (cameraFirst); }
      set
      {
        if (cameraFirst != null)
        {
          cameraFirst.NewFrame -= cameraFirst_NewFrame;
        }

        cameraFirst = value;

        if (cameraFirst != null)
        {
          cameraFirst.NewFrame += cameraFirst_NewFrame;
        }
      }
    }
    public ICapture CameraSecond
    {
      get { return (cameraSecond); }
      set
      {
        if (cameraSecond != null)
        {
          cameraSecond.NewFrame -= cameraSecond_NewFrame;
        }

        cameraSecond = value;

        if (cameraSecond != null)
        {
          cameraSecond.NewFrame += cameraSecond_NewFrame;
        }
      }
    }

    public IInputDevice Input { get; set; }

    public IPrinter Printer { get; set; }
    #endregion



    private void timer_Tick(object sender, EventArgs e)
    {
      if (secondsTimeout >= 0)
      {
        BlockButtonText = string.Format("{0} - Chặn lại", secondsTimeout);
        --secondsTimeout;
      }
      else
      {
        BlockButtonText = "Chặn lại";

        timer.Stop();
        DisableCommands();
      }
    }
    public LaneViewModel(IInputDevice input)
    {
      Requires.NotNull(input, "input");

      Input = input;
      Allow = new AutoResetEvent(false);
      Block = new AutoResetEvent(false);

      uniqueId = new UniqueId(string.Empty);
      userData = new UserData(string.Empty);

      Deregister = CenterUnit.Current.Reporter.Subscribe(this);

      AllowButtonText = "Cho phép";
      BlockButtonText = "Chặn lại";

      timer = new DispatcherTimer();
      timer.Interval = new TimeSpan(0, 0, 1);
      timer.Tick += new EventHandler(timer_Tick);
    }

    #region Public Methods
    public void OnError(Exception error)
    {
      /// Lọc ngoại lệ. (^_^)
      if (false)//error is SystemErrorException)
      {
        //Debug.WriteLine(
        //  "{Lane}: Có lỗi hệ thống xảy ra.\n{Message}\nVui lòng thông báo với quản trị."
        //  .NamedFormat(new { Lane = Name, Message = error.Message}));
      }
      else
      {
        NewMessage(error.Message);
      }

      timer.Stop();
      DisableCommands();
    }

    public void OnNext(EntryArgs value)
    {
      uniqueId = value.UniqueId;
      RaisePropertyChanged(() => UniqueId);

      userData = value.UserData;
      RaisePropertyChanged(() => UserData);

      var first = value.Images.First as BitmapImageHolder;
      SavedImageBack = (first == null || !first.Available) ? null : first.Image;

      var second = value.Images.Second as BitmapImageHolder;
      SavedImageFront = (second == null || !second.Available) ? null : second.Image;

      NewMessage("Đang chờ sự cho phép... ");

      timer.Stop();
      DisableCommands();
    }

    public void OnCompleted()
    {
      if (Direction == Qms.Direction.Export)
      {
        NewMessage("Mời phương tiện RA bãi");
      }
      else
      {
        NewMessage("Mời phương tiện VÀO bãi");
      }

      timer.Stop();
      DisableCommands();
    }
    #endregion

    #region Explicit Implementation of IGate
    void IGate.SavedImage(IFileStorage fileStorage, string first, string second)
    {
      if (fileStorage.Exists(first))
      {
        using (var stream = fileStorage.Open(first))
        {
          SavedImageBack = BitmapImageFromStream(stream);
        }
      }
      else
      {
        SavedImageBack = null;
      }

      if (fileStorage.Exists(second))
      {
        using (var stream = fileStorage.Open(second))
        {
          SavedImageFront = BitmapImageFromStream(stream);
        }
      }
      else
      {
        SavedImageFront = null;
      }
    }
    void IGate.ImportDisplay(IImport import)
    {
    }
    void IGate.ExportDisplay(IExport export)
    {
    }
    void IGate.TimeoutDisplay(int milisecondsTimeout)
    {
      secondsTimeout = milisecondsTimeout / 1000;

      timer.Start();
      EnableCommands();
    }
    #endregion

    #region Private Methods
    private void EnableCommands()
    {
      AllowButtonEnable = true;
      BlockButtonEnable = true;
    }
    private void DisableCommands()
    {
      AllowButtonText = "Cho phép";
      BlockButtonText = "Chặn lại";

      AllowButtonEnable = false;
      BlockButtonEnable = false;

      Allow.Reset();
      Block.Reset();
    }
    private void cameraFirst_NewFrame(object sender, EventArgs e)
    {
      var args = e as NewFrameEventArgs;
      var image = args.Image as BitmapImageHolder;
      if (image != null)
      {
        this.CameraImageBack = image.Image;
      }
    }

    private void cameraSecond_NewFrame(object sender, EventArgs e)
    {
      var args = e as NewFrameEventArgs;
      var image = args.Image as BitmapImageHolder;
      if (image != null)
      {
        this.CameraImageFront = image.Image;
      }
    }

    private void NewMessage(string message)
    {
      this.Message = "{Time:HH:mm:ss} - {Message}".NamedFormat(
        new { Time = DateTime.Now, Message = message });
    }

    private static BitmapImage BitmapImageFromStream(Stream stream)
    {
      BitmapImage image = new BitmapImage();
      image.BeginInit();
      image.StreamSource = stream;
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.EndInit();
      image.Freeze();

      return (image);
    }
    #endregion

    
  }
}