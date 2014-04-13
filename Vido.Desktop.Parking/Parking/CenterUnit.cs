using System;
using Vido.Media.Capture;
using Vido.Qms;
namespace Vido.Parking
{
  public class CenterUnit
  {
    private DailyDirectory imageRoot = null;
    private EFEntryRecorder recorder = null;
    private EFUniqueIdStorage idStorage = null;
    private ControllerServices services = null;
    private InputDeviceList inputList = null;
    private GateController reporter = null;
    private CaptureList captureList = null;

    private static CenterUnit current = new CenterUnit();

    public static CenterUnit Current
    {
      get { return (current); }
    }

    private CenterUnit()
    {
    }

    public void RegisterDependencies(IntPtr mainWindowHandle, ICaptureFactory captureFactory)
    {
      imageRoot = new DailyDirectory();
      recorder  = new EFEntryRecorder();
      idStorage = new EFUniqueIdStorage();

      services  = new ControllerServices() { ImageRoot = imageRoot };
      inputList = new InputDeviceList(mainWindowHandle);
      reporter  = new GateController(services, inputList, idStorage, recorder);
      captureList = new CaptureList(captureFactory);
    }

    public DailyDirectory ImageRoot
    {
      get { return (imageRoot); }
    }
    public GateController Reporter
    {
      get { return (reporter); }
    }
    public ControllerServices ReporterServices
    {
      get { return (services); }
    }
    public EFEntryRecorder Recorder
    {
      get { return (recorder); }
    }
    public EFUniqueIdStorage IdStorage
    {
      get { return (idStorage); }
    }
    public CaptureList CaptureList
    {
      get { return (captureList); }
    }
  }
}