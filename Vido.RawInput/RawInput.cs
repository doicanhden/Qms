
namespace Vido.RawInput
{
  using System;
  using System.Runtime.InteropServices;
  using System.Windows.Forms;
  using Vido.RawInput.User32;

  public class RawInput : NativeWindow
  {
    #region Data Members
    private static readonly Guid DeviceInterfaceHid = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
    private static RawKeyboard keyboard;
    private readonly IntPtr devNotifyHandle;
    private PreMessageFilter filter;
    #endregion

    #region Properties
    public IRawKeyboard Keyboard
    {
      get { return (keyboard); }
    }
    #endregion

    #region Constructors
    public RawInput(IntPtr parentHandle)
    {
      AssignHandle(parentHandle);
      keyboard = new RawKeyboard();
      devNotifyHandle = RegisterForDeviceNotifications(parentHandle);
      keyboard.RegisterRawInput(parentHandle);

    }

    ~RawInput()
    {
      NativeMethods.UnregisterDeviceNotification(devNotifyHandle);
    }
    #endregion

    #region Methods
    public void AddMessageFilter()
    {
      if (null != filter)
        return;

      filter = new PreMessageFilter();
      Application.AddMessageFilter(filter);
    }

    public void RemoveMessageFilter()
    {
      if (null == filter)
        return;

      Application.RemoveMessageFilter(filter);
    }
    #endregion

    #region Private Methods
    private static IntPtr RegisterForDeviceNotifications(IntPtr parent)
    {
      var bdi = new DEV_BROADCAST_DEVICEINTERFACE();
      bdi.dbcc_size = Marshal.SizeOf(bdi);
      bdi.BroadcastDeviceType = NativeMethods.DBT_DEVTYP_DEVICEINTERFACE;
      bdi.dbcc_classguid = DeviceInterfaceHid;

      var usbNotifyHandle = IntPtr.Zero;
      var mem = IntPtr.Zero;
      try
      {
        mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE)));
        Marshal.StructureToPtr(bdi, mem, false);
        usbNotifyHandle = NativeMethods.RegisterDeviceNotification(parent, mem, NativeMethods.DEVICE_NOTIFY_WINDOW_HANDLE);
      }
//    catch (Exception e)
//    {
//      Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
//      Debug.Print(e.StackTrace);
//    }
      finally
      {
        Marshal.FreeHGlobal(mem);
      }

      if (usbNotifyHandle == IntPtr.Zero)
      {
//      Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
      }

      return (usbNotifyHandle);
    }

    protected override void WndProc(ref Message message)
    {
      switch (message.Msg)
      {
        case NativeMethods.WM_INPUT:
          {
            // Should never get here if you are using PreMessageFiltering
            keyboard.ProcessRawInput(message.LParam);
          }
          break;

        case NativeMethods.WM_DEVICECHANGE:
          {
//          Debug.WriteLine("USB Device Arrival / Removal");
            keyboard.EnumerateDevices();
          }
          break;
      }

      base.WndProc(ref message);
    }

    private class PreMessageFilter : IMessageFilter
    {
      public bool PreFilterMessage(ref Message message)
      {
        if (message.Msg != NativeMethods.WM_INPUT)
        {
          // Allow any non WM_INPUT message to pass through
          return (false);
        }

        return (keyboard.ProcessRawInput(message.LParam));
      }
    }
    #endregion
  }

}
