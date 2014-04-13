namespace Vido.Parking.Ui
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Vido.Media.Capture;
  using Vido.Qms;

  public class Settings
  {
    private readonly Datasets.Settings.ParkingConfigsDataTable parkingCfgs = null;
    private readonly Datasets.Settings.ControllerConfigsDataTable controllerCfgs = null;
    private readonly Datasets.Settings.LaneConfigsDataTable laneCfgs = null;

    public static string SettingsDirectoryName
    {
      get { return (Environment.CurrentDirectory + @"\Settings"); }
    }
    public static string ParkingFileName
    {
      get { return (SettingsDirectoryName + @"\Parkings.xml"); }
    }
    public static string ControllerFileName
    {
      get { return (SettingsDirectoryName + @"\Controllers.xml"); }
    }
    public static string LaneFileName
    {
      get { return (SettingsDirectoryName + @"\Lanes.xml"); }
    }

    public Settings()
    {
      parkingCfgs = new Datasets.Settings.ParkingConfigsDataTable();
      controllerCfgs = new Datasets.Settings.ControllerConfigsDataTable();
      laneCfgs = new Datasets.Settings.LaneConfigsDataTable();

      if (!Directory.Exists(SettingsDirectoryName))
      {
        Directory.CreateDirectory(SettingsDirectoryName);
      }

      Load();
    }

    public void Save()
    {
      parkingCfgs.WriteXml(ParkingFileName);
      controllerCfgs.WriteXml(ControllerFileName);
      laneCfgs.WriteXml(LaneFileName);
    }
    public void Load()
    {
      if (!File.Exists(ParkingFileName))
      {
        DefaultParkingConfigs();
        parkingCfgs.WriteXml(ParkingFileName);
      }
      else
      {
        parkingCfgs.ReadXml(ParkingFileName);
      }

      if (!File.Exists(ControllerFileName))
      {
        DefaultControllerConfigs();
        controllerCfgs.WriteXml(ControllerFileName);
      }
      else
      {
        controllerCfgs.ReadXml(ControllerFileName);
      }

      if (!File.Exists(LaneFileName))
      {
        DefaultLaneConfigs();
        laneCfgs.WriteXml(LaneFileName);
      }
      else
      {
        laneCfgs.ReadXml(LaneFileName);
      }
    }
    public Datasets.Settings.ParkingConfigsDataTable Parking
    {
      get { return (parkingCfgs); }
    }
    public Datasets.Settings.LaneConfigsDataTable LaneConfigs
    {
      get { return (laneCfgs); }
    }
    public Datasets.Settings.ControllerConfigsDataTable Controller
    {
      get { return (controllerCfgs); }
    }

    //public void SetParking(IParking parking, int level = 0)
    //{
    //  foreach (Datasets.Settings.ParkingConfigsRow cfg in parkingCfgs.Rows)
    //  {
    //    if (cfg.Level == level)
    //    {
    //      parking.MaximumSlots = cfg.MaximumSlots;
    //      break;
    //    }
    //  }
    //}
    //public void SetController(Controller controller, int id = 0)
    //{
    //  foreach (Datasets.Settings.ControllerConfigsRow cfg in controllerCfgs.Rows)
    //  {
    //    if (cfg.Id == id)
    //    {
    //      controller.RootImageDirectoryName = cfg.RootImageDirectoryName;
    //      controller.DailyDirectoryFormat = cfg.DailyDirectoryFormat;

    //      controller.BackImageNameFormat = cfg.BackImageNameFormat;
    //      controller.FrontImageNameFormat = cfg.FrontImageNameFormat;

    //      controller.InFormat = cfg.InFormat;
    //      controller.OutFormat = cfg.OutFormat;
    //      controller.EntryRequestTimeout = cfg.EntryRequestTimeout;
    //      controller.LaneConfigs = new List<LaneConfigs>();
    //      break;
    //    }
    //  }
    //}
    //public void SetLaneConfigs(ICollection<LaneConfigs> lanes)
    //{
    //  foreach (Datasets.Settings.LaneConfigsRow cfg in laneCfgs.Rows)
    //  {
    //    lanes.Add(new LaneConfigs()
    //    {
    //      Code = cfg.Code,
    //      Direction = (Enums.Direction)cfg.Direction,
    //      State = (Enums.LaneState)cfg.State,
    //      NumberOfRetries = cfg.NumberOfRetries,
    //      UidDeviceName = cfg.UIdDeviceName,

    //      BackCamera = new Configs()
    //      {
    //        Source = cfg.BackCamSource,
    //        Coding = (Capture.Enums.Coding)cfg.BackCamCoding,
    //        Username = cfg.BackCamUsername,
    //        Password = cfg.BackCamPassword,
    //        FrameInterval = 100
    //      },
    //      FrontCamera = new Configs()
    //      {
    //        Source = cfg.FrontCamSource,
    //        Coding = (Capture.Enums.Coding)cfg.FrontCamCoding,
    //        Username = cfg.FrontCamUsername,
    //        Password = cfg.FrontCamPassword,
    //        FrameInterval = 100
    //      }
    //    });
    //  }
    //}

    private void DefaultLaneConfigs()
    {
      var newRow1 = laneCfgs.NewRow() as Datasets.Settings.LaneConfigsRow;

      newRow1.Code = "LANE1";
      newRow1.State = (int)(GateState.Opened);
      newRow1.Direction = (int)(Direction.Import);
      newRow1.UIdDeviceName = @"VID_0E6A&PID_030B";

      newRow1.BackCamSource = "http://camera1.mairie-brest.fr/mjpg/video.mjpg?resolution=320x240";
      newRow1.BackCamCoding = (int)(Coding.MJpeg);
      newRow1.BackCamUsername = "admin";
      newRow1.BackCamPassword = "admin";

      newRow1.FrontCamSource = "http://camera1.mairie-brest.fr/mjpg/video.mjpg?resolution=320x240";
      newRow1.FrontCamCoding = (int)(Coding.MJpeg);
      newRow1.FrontCamUsername = "admin";
      newRow1.FrontCamPassword = "admin";

      laneCfgs.Rows.Add(newRow1);


      var newRow2 = laneCfgs.NewRow() as Datasets.Settings.LaneConfigsRow;

      newRow2.Code = "LANE2";
      newRow2.State = (int)(GateState.Opened);
      newRow2.Direction = (int)(Direction.Export);
      newRow2.UIdDeviceName = @"PNP0303";

      newRow2.BackCamSource = "http://camera1.mairie-brest.fr/mjpg/video.mjpg?resolution=320x240";
      newRow2.BackCamCoding = (int)(Coding.MJpeg);
      newRow2.BackCamUsername = "admin";
      newRow2.BackCamPassword = "admin";

      newRow2.FrontCamSource = "http://camera1.mairie-brest.fr/mjpg/video.mjpg?resolution=320x240";
      newRow2.FrontCamCoding = (int)(Coding.MJpeg);
      newRow2.FrontCamUsername = "admin";
      newRow2.FrontCamPassword = "admin";

      laneCfgs.Rows.Add(newRow2);
    }
    private void DefaultControllerConfigs()
    {
      var newRow = controllerCfgs.NewRow() as Datasets.Settings.ControllerConfigsRow;

      newRow.Id = 0;
      newRow.ImageRootDirectoryName = Environment.CurrentDirectory + @"\Images";
      newRow.DailyDirectoryFormat = @"yyyy\MM\dd";
      newRow.ImageNameFormat = "IMG_{EntryTime:HHmmss}{EntryGate}{Direction}_{UniqueId}_{UserData}{Index}.jpg";
      newRow.ImportFormat = "IM";
      newRow.ExportFormat = "EX";

      controllerCfgs.Rows.Add(newRow);
    }
    private void DefaultParkingConfigs()
    {
      var newRow = parkingCfgs.NewRow() as Datasets.Settings.ParkingConfigsRow;

      newRow.Level = 0;
      newRow.MaximumSlots = 100000;
      newRow.MinimumSlots = 0;

      parkingCfgs.Rows.Add(newRow);
    }
  }
}
