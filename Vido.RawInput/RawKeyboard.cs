namespace Vido.RawInput
{
  using Microsoft.Win32;
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Globalization;
  using System.Runtime.InteropServices;
  using Vido.RawInput.Enums;
  using Vido.RawInput.Events;
  using Vido.RawInput.User32;

  [StructLayout(LayoutKind.Explicit)]
  internal struct RAWDATA
  {
    [FieldOffset(0)]
    internal RAWMOUSE Mouse;

    [FieldOffset(0)]
    internal RAWKEYBOARD Keyboard;

    [FieldOffset(0)]
    internal RAWHID Hid;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct InputData
  {
    internal RAWINPUTHEADER Header; // 64 bit header size is 24  32 bit the header size is 16
    internal RAWDATA Data;          // Creating the rest in a struct allows the header size to align correctly for 32 or 64 bit
  }

  public class RawKeyboard : IRawKeyboard
  {
    private readonly object objLock = new object();
    private List<IKeyboard> devices = new List<IKeyboard>();
    private static InputData rawBuffer;

    public event EventHandler DevicesChanged;

    public ICollection<IKeyboard> Devices
    {
      get { return (devices); }
    }

    public bool RegisterRawInput(IntPtr hwnd)
    {
      var rid = new RAWINPUTDEVICE[1];

      rid[0].UsagePage = HidUsagePage.GENERIC;
      rid[0].Usage = HidUsageId.Keyboard;
      rid[0].Flags = RawInputDeviceFlags.INPUTSINK;
      rid[0].Target = hwnd;

      return (NativeMethods.RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])));
    }

    public int EnumerateDevices()
    {
      List<IKeyboard> newDevices = new List<IKeyboard>();

      var globalDevice = new Keyboard
      {
        Name = "Global Keyboard",
        Handle = IntPtr.Zero,
        Type = GetDeviceType(NativeMethods.RIM_TYPEKEYBOARD),
        Description = "Fake Keyboard. Some keys (ZOOM, MUTE, VOLUMEUP, VOLUMEDOWN) are sent to rawinput with a handle of zero.",
      };

      newDevices.Add(globalDevice);

      uint deviceCount = 0;
      var dwSize = (Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));

      if (NativeMethods.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
      {
        var pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
        NativeMethods.GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

        for (var i = 0; i < deviceCount; i++)
        {
          // On Window 8 64bit when compiling against .Net > 3.5
          // using .ToInt32 you will generate an arithmetic overflow.
          // Leave as it is for 32bit/64bit applications
          var rid = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(
            new IntPtr((pRawInputDeviceList.ToInt64() + (dwSize * i))), typeof(RAWINPUTDEVICELIST));

          uint pcbSize = 0;
          NativeMethods.GetRawInputDeviceInfo(rid.hDevice, NativeMethods.RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

          if (pcbSize <= 0)
            continue;

          var pData = Marshal.AllocHGlobal((int)pcbSize);
          NativeMethods.GetRawInputDeviceInfo(rid.hDevice, NativeMethods.RIDI_DEVICENAME, pData, ref pcbSize);
          var deviceName = Marshal.PtrToStringAnsi(pData);

          if (rid.dwType == NativeMethods.RIM_TYPEKEYBOARD || rid.dwType == NativeMethods.RIM_TYPEHID)
          {
            if (!newDevices.Exists(x => x.Handle == rid.hDevice))
            {
              var dInfo = new Keyboard
              {
                Name = deviceName,
                Handle = rid.hDevice,
                Type = GetDeviceType(rid.dwType),
                Description = GetDeviceDescription(deviceName)
              };

              newDevices.Add(dInfo);
            }
          }

          Marshal.FreeHGlobal(pData);
        }

        Marshal.FreeHGlobal(pRawInputDeviceList);


        var oldDevices = devices;
        lock (objLock)
        {
          devices = newDevices;
        }

        if (DevicesChanged != null)
        {
          DevicesChanged(this, new DevicesChangedEventArgs(oldDevices, devices));
        }

        return (devices.Count);
      }

      return (0);
    }

    public bool ProcessRawInput(IntPtr hdevice)
    {
      if (devices.Count == 0)
        return (false);

//    if (CaptureOnlyIfTopMostWindow && !Win32.InputInForeground(_rawBuffer.header.wParam))
//      return (false);

      var dwSize = 0;
      NativeMethods.GetRawInputData(hdevice, NativeMethods.RID_INPUT, IntPtr.Zero, ref dwSize, Marshal.SizeOf(typeof(RAWINPUTHEADER)));

      if (dwSize != NativeMethods.GetRawInputData(hdevice, NativeMethods.RID_INPUT, out rawBuffer, ref dwSize, Marshal.SizeOf(typeof(RAWINPUTHEADER))))
      {
        return (false);
      }

      int virtualKey = rawBuffer.Data.Keyboard.VKey;
      int makeCode = rawBuffer.Data.Keyboard.MakeCode;
      int flags = rawBuffer.Data.Keyboard.Flags;

      if (virtualKey == NativeMethods.KEYBOARD_OVERRUN_MAKE_CODE)
        return (false);

      if (devices.Exists(x => x.Handle == rawBuffer.Header.hDevice))
      {
        Keyboard keyboard = null;
        lock (objLock)
        {
          keyboard = devices.Find(x => x.Handle == rawBuffer.Header.hDevice) as Keyboard;
        }

        var keyCode = VirtualKeyCorrection(virtualKey, (flags & NativeMethods.RI_KEY_E0) != 0, makeCode);

        if (rawBuffer.Data.Keyboard.Message == NativeMethods.WM_KEYDOWN)
        {
          keyboard.RaiseKeyDown(keyCode);
        }
        else if (rawBuffer.Data.Keyboard.Message == NativeMethods.WM_KEYUP)
        {
          keyboard.RaiseKeyUp(keyCode);
        }

        return (true);
      }

      return (false);
    }

    public static string GetDeviceDescription(string device)
    {
      var split = device.Substring(4).Split('#');

      var classCode = split[0];    // ACPI (Class code)
      var subClassCode = split[1]; // PNP0303 (SubClass code)
      var protocolCode = split[2]; // 3&13c0b0c5&0 (Protocol code)
      
      var deviceKey = Registry.LocalMachine.OpenSubKey(string.Format(@"System\CurrentControlSet\Enum\{0}\{1}\{2}", classCode, subClassCode, protocolCode));
      var deviceDesc = deviceKey.GetValue("DeviceDesc").ToString();
      deviceDesc = deviceDesc.Substring(deviceDesc.IndexOf(';') + 1);

      //var deviceClass = RegistryAccess.GetClassType(deviceKey.GetValue("ClassGUID").ToString());
      //isKeyboard = deviceClass.ToUpper().Equals( "KEYBOARD" );

      return deviceDesc;
    }
    private static string GetDeviceType(uint device)
    {
      string deviceType;
      switch (device)
      {
        case NativeMethods.RIM_TYPEMOUSE:
          deviceType = "MOUSE";
          break;
        case NativeMethods.RIM_TYPEKEYBOARD:
          deviceType = "KEYBOARD";
          break;
        case NativeMethods.RIM_TYPEHID:
          deviceType = "HID";
          break;
        default:
          deviceType = "UNKNOWN";
          break;
      }

      return (deviceType);
    }
    private static int VirtualKeyCorrection(int virtualKey, bool isE0BitSet, int makeCode)
    {
      var correctedVKey = virtualKey;

      if (rawBuffer.Header.hDevice == IntPtr.Zero)
      {
        // When hDevice is 0 and the vkey is VK_CONTROL indicates the ZOOM key
        if (rawBuffer.Data.Keyboard.VKey == NativeMethods.VK_CONTROL)
        {
          correctedVKey = NativeMethods.VK_ZOOM;
        }
      }
      else
      {
        switch (virtualKey)
        {
          // Right-hand CTRL and ALT have their e0 bit set 
          case NativeMethods.VK_CONTROL:
            correctedVKey = isE0BitSet ? NativeMethods.VK_RCONTROL : NativeMethods.VK_LCONTROL;
            break;
          case NativeMethods.VK_MENU:
            correctedVKey = isE0BitSet ? NativeMethods.VK_RMENU : NativeMethods.VK_LMENU;
            break;
          case NativeMethods.VK_SHIFT:
            correctedVKey = makeCode == NativeMethods.SC_SHIFT_R ? NativeMethods.VK_RSHIFT : NativeMethods.VK_LSHIFT;
            break;
          default:
            correctedVKey = virtualKey;
            break;
        }
      }

      return (correctedVKey);
    }

  }
}
