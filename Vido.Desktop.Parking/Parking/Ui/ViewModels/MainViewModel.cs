namespace Vido.Parking.Ui.ViewModels
{
  using System;
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Interop;
  using Vido.Media.Capture;
  using Vido.Parking.Ui.Commands;
  using Vido.Parking.Ui.Utilities;
  using Vido.Parking.Ui.Views;
  using Vido.Qms;

  public class MainViewModel : NotificationObject
  {
    #region Data Members
    private readonly Settings settings = new Settings();
    private readonly Window mainWindow;
    private readonly CaptureFactory capFactory;
    private readonly ObservableCollection<LaneViewModel> laneViewModels;
    private string status;
    private ICommand showLaneConfigsCommand;
    private ICommand showConnectionStringEditorCommand;
    private ICommand showSwitchShiftsDialogCommand;
    #endregion

    #region Public Properties
    public Settings Settings
    {
      get { return (settings); }
    }

    public Window View
    {
      get { return (mainWindow); }
    }
    #endregion

    #region Public Properties (for Binding)
    /// <summary>
    /// Danh sách các ViewModel của lane.
    /// </summary>
    public ObservableCollection<LaneViewModel> Lanes
    {
      get { return (laneViewModels); }
    }

    /// <summary>
    /// Trạng thái.
    /// </summary>
    public string Status
    {
      get { return (status); }
      set
      {
        status = value;
        RaisePropertyChanged(() => Status);
      }
    }

    public ICommand ShowLaneConfigsCommand
    {
      get
      {
        return (showLaneConfigsCommand ?? (showLaneConfigsCommand =
          new RelayCommand(ShowLaneConfigsExecute)));
      }
    }

    public ICommand ShowConnectionStringEditorCommand
    {
      get
      {
        return (showConnectionStringEditorCommand ?? (showConnectionStringEditorCommand =
          new RelayCommand(ShowConnectionStringEditExecute)));
      }
    }

    public ICommand ShowSwitchShiftsDialogCommand
    {
      get
      {
        return (showSwitchShiftsDialogCommand ?? (showSwitchShiftsDialogCommand =
          new RelayCommand(ShowSwitchShiftsDialogExecute)));
      }
    }
    #endregion

    #region Public Constructors
    public MainViewModel(Window mainWindow)
    {
      this.mainWindow = mainWindow;
      this.capFactory = new CaptureFactory();
      this.laneViewModels = new ObservableCollection<LaneViewModel>();

      TestDatabaseConnection();

      CenterUnit.Current.RegisterDependencies(GetHandle(mainWindow), capFactory);
      CenterUnit.Current.Recorder.NewMessage += UpdateStatus;
      CenterUnit.Current.IdStorage.NewMessage += UpdateStatus;

      LoadSettings();

      
    }

    #endregion

    #region Private Methods
    private void LoadSettings()
    {
      foreach (Datasets.Settings.ParkingConfigsRow cfg in settings.Parking.Rows)
      {
        if (cfg.Level == 0)
        {
          CenterUnit.Current.Recorder.MaximumSlots = cfg.MaximumSlots;
          CenterUnit.Current.Recorder.MinimumSlots = cfg.MinimumSlots;
          break;
        }
      }

      foreach (Datasets.Settings.ControllerConfigsRow cfg in settings.Controller.Rows)
      {
        if (cfg.Id == 0)
        {
          CenterUnit.Current.ImageRoot.RootDirectoryName = cfg.ImageRootDirectoryName;
          break;
        }
      }

      foreach (Datasets.Settings.LaneConfigsRow cfg in settings.LaneConfigs.Rows)
      {
        Lanes.Add(new LaneViewModel(new Vido.InputDevice()
        {
          Name = cfg.UIdDeviceName,
          EndKey = 13
        })
        {
          Name = cfg.Code,
          Direction = (Direction)cfg.Direction,
          State = (GateState)cfg.State,

          CameraFirst = CenterUnit.Current.CaptureList.Create(new Configuration()
          {
            Source = cfg.BackCamSource,
            Coding = (Coding)cfg.BackCamCoding,
            Username = cfg.BackCamUsername,
            Password = cfg.BackCamPassword,
            FrameInterval = 100
          }),

          CameraSecond = CenterUnit.Current.CaptureList.Create(new Configuration()
          {
            Source = cfg.FrontCamSource,
            Coding = (Coding)cfg.FrontCamCoding,
            Username = cfg.FrontCamUsername,
            Password = cfg.FrontCamPassword,
            FrameInterval = 100
          })
        });
      }

      foreach (var cap in CenterUnit.Current.CaptureList.Captures)
      {
        cap.Start();
      }
    }

    private void UpdateStatus(object sender, EventArgs e)
    {
      var args = e as NewMessageEventArgs;
      Status = args.Message;
    }

    private void ShowConnectionStringEditExecute(object obj)
    {
      new Views.ConnectionStringEditorView()
      {
        Owner = mainWindow,
        DataContext = new ConnectionStringEditorViewModel()
      }.ShowDialog();
    }

    private void ShowLaneConfigsExecute(object obj)
    {
      new Views.LaneManagementView()
      {
        Owner = mainWindow,
        DataContext = new LaneManagementViewModel(this)
      }.ShowDialog();
    }

    private void ShowSwitchShiftsDialogExecute(object obj)
    {
      new Views.SwitchShiftsView()
      {
        Owner = mainWindow
      }.ShowDialog();
    }

    private void TestDatabaseConnection()
    {
      bool ret;
      do
      {
        try
        {
          ret = Database.TestConnection();
        }
        catch (Exception ex)
        {
          ret = false;
          MessageBox.Show(ex.Message);
        }

        if (!ret)
        {
          if (false == new ConnectionStringEditorView()
          {
            Owner = mainWindow,
            DataContext = new ConnectionStringEditorViewModel(false)
          }.ShowDialog())
          {
            Application.Current.Shutdown();
            break;
          }
        }
      } while (!ret);
    }
    #endregion

    private static IntPtr GetHandle(Window window)
    {
      if (window == null)
      {
        return (IntPtr.Zero);
      }

      return (new WindowInteropHelper(window).Handle);
    }
  }
}