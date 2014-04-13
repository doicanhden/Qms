namespace Vido.RawInput.User32
{
  using System;
  using System.Runtime.InteropServices;
  using Vido.RawInput.Enums;

  #region Raw Input Structures

  [StructLayout(LayoutKind.Sequential)]
  internal struct RAWHID
  {
    internal uint dwSizHid;
    internal uint dwCount;
    internal byte bRawData;
  }

  [StructLayout(LayoutKind.Explicit)]
  internal struct RAWMOUSE
  {
    [FieldOffset(0)]
    internal ushort usFlags;

    [FieldOffset(4)]
    internal uint ulButtons;

    [FieldOffset(4)]
    internal ushort usButtonFlags;

    [FieldOffset(6)]
    internal ushort usButtonData;

    [FieldOffset(8)]
    internal uint ulRawButtons;

    [FieldOffset(12)]
    internal int lLastX;

    [FieldOffset(16)]
    internal int lLastY;

    [FieldOffset(20)]
    internal uint ulExtraInformation;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct RAWKEYBOARD
  {
    internal ushort MakeCode;       // Scan code from the key depression
    internal ushort Flags;          // One or more of RI_KEY_MAKE, RI_KEY_BREAK, RI_KEY_E0, RI_KEY_E1
    internal ushort Reserved;       // Always 0
    internal ushort VKey;           // Virtual Key Code
    internal uint Message;          // Corresponding Windows message for exmaple (WM_KEYDOWN, WM_SYASKEYDOWN etc)
    internal uint ExtraInformation; // The device-specific addition information for the event (seems to always be zero for keyboards)
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct RAWINPUTDEVICE
  {
    internal HidUsagePage UsagePage;
    internal HidUsageId Usage;
    internal RawInputDeviceFlags Flags;
    internal IntPtr Target;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct RAWINPUTDEVICELIST
  {
    internal IntPtr hDevice;
    internal uint dwType;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct RAWINPUTHEADER
  {
    internal uint dwType;    // Type of raw input (RIM_TYPEHID 2, RIM_TYPEKEYBOARD 1, RIM_TYPEMOUSE 0)
    internal uint dwSize;    // Size in bytes of the entire input packet of data. This includes RAWINPUT plus possible extra input reports in the RAWHID variable length array. 
    internal IntPtr hDevice; // A handle to the device generating the raw input data. 
    internal IntPtr wParam;  // RIM_INPUT 0 if input occurred while application was in the foreground else RIM_INPUTSINK 1 if it was not.
  }
/*
  internal struct RID_DEVICE_INFO_MOUSE
  {
    internal uint Id;                 // Identifier of the mouse device
    internal uint NumberOfButtons;    // Number of buttons for the mouse
    internal uint SampleRate;         // Number of data points per second.
    internal bool HasHorizontalWheel; // True is mouse has wheel for horizontal scrolling else false.
  }

  internal struct RID_DEVICE_INFO_KEYBOARD
  {
    internal uint Type;                       // Type of the keyboard
    internal uint SubType;                    // Subtype of the keyboard
    internal uint KeyboardMode;               // The scan code mode
    internal uint NumberOfFunctionKeys;       // Number of function keys on the keyboard
    internal uint NumberOfIndicators;         // Number of LED indicators on the keyboard
    internal uint NumberOfKeysTotal;          // Total number of keys on the keyboard
  }

  internal struct RID_DEVICE_INFO_HID
  {
    internal uint VendorID;      // Vendor identifier for the HID
    internal uint ProductID;     // Product identifier for the HID
    internal uint VersionNumber; // Version number for the device
    internal ushort UsagePage;   // Top-level collection Usage page for the device
    internal ushort Usage;       // Top-level collection Usage for the device
  }

  [StructLayout(LayoutKind.Explicit)]
  internal struct RID_DEVICE_INFO
  {
    [FieldOffset(0)]
    internal int Size;

    [FieldOffset(4)]
    internal int Type;

    [FieldOffset(8)]
    internal RID_DEVICE_INFO_MOUSE MouseInfo;

    [FieldOffset(8)]
    internal RID_DEVICE_INFO_KEYBOARD KeyboardInfo;

    [FieldOffset(8)]
    internal RID_DEVICE_INFO_HID HIDInfo;
  }
*/
  internal struct DEV_BROADCAST_DEVICEINTERFACE
  {
    internal Int32 dbcc_size;
    internal Int32 BroadcastDeviceType;
    private  Int32 dbcc_reserved;
    internal Guid dbcc_classguid;
    private  char dbcc_name;
  }
  #endregion

  internal static class NativeMethods
  {
    #region Constants
    internal const int WM_KEYDOWN = 0x0100;
    internal const int WM_KEYUP = 0x0101;

    internal const uint RID_HEADER = 0x10000005; // Get the header information from the RAWINPUT structure.
    internal const uint RID_INPUT = 0x10000003;  // Get the raw data from the RAWINPUT structure.

    internal const uint RIDI_DEVICENAME = 0x20000007;
    internal const uint RIDI_DEVICEINFO = 0x2000000b;
    internal const uint RIDI_PREPARSEDDATA = 0x20000005;

    internal const int RIM_TYPEMOUSE = 0;
    internal const int RIM_TYPEKEYBOARD = 1;
    internal const int RIM_TYPEHID = 2;

    internal const int KEYBOARD_OVERRUN_MAKE_CODE = 0xFF;

    internal const int RI_KEY_MAKE = 0x00;      // Key Down
    internal const int RI_KEY_BREAK = 0x01;     // Key Up
    internal const int RI_KEY_E0 = 0x02;        // Left version of the key
    internal const int RI_KEY_E1 = 0x04;        // Right version of the key. Only seems to be set for the Pause/Break key.

    internal const int VK_CONTROL = 0x11;
    internal const int VK_MENU = 0x12;
    internal const int VK_ZOOM = 0xFB;
    internal const int VK_LSHIFT = 0xA0;
    internal const int VK_RSHIFT = 0xA1;
    internal const int VK_LCONTROL = 0xA2;
    internal const int VK_RCONTROL = 0xA3;
    internal const int VK_LMENU = 0xA4;
    internal const int VK_RMENU = 0xA5;

    internal const int VK_SHIFT = 0x10;
    internal const int SC_SHIFT_R = 0x36;
    internal const int SC_SHIFT_L = 0x2a;

    internal const int WM_INPUT = 0x00FF;
    internal const int WM_DEVICECHANGE = 0x0219;

    internal const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
    internal const int DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001;
    internal const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004;


    internal const int DBT_DEVTYP_OEM = 0;
    internal const int DBT_DEVTYP_DEVNODE = 1;
    internal const int DBT_DEVTYP_VOLUME = 2;
    internal const int DBT_DEVTYP_PORT = 3;
    internal const int DBT_DEVTYP_NET = 4;
    internal const int DBT_DEVTYP_DEVICEINTERFACE = 5;
    internal const int DBT_DEVTYP_HANDLE = 6;
    #endregion

    #region Raw Input Functions

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern int GetRawInputData(IntPtr hRawInput, uint command, [Out] out InputData buffer, [In, Out] ref int size, int cbSizeHeader);

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern int GetRawInputData(IntPtr hRawInput, uint command, [Out] IntPtr pData, [In, Out] ref int size, int sizeHeader);

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint command, IntPtr pData, ref uint size);

//  [DllImport("User32.dll")]
//  internal static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint command, ref RID_DEVICE_INFO data, ref uint dataSize);

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint numberDevices, uint size);

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint numberDevices, uint size);

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, int flags);

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern bool UnregisterDeviceNotification(IntPtr handle);

    #endregion
  }
}
