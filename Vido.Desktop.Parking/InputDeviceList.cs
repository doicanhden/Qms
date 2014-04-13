namespace Vido
{
  using System;
  using System.Collections.Generic;

  public class InputDeviceList : IInputDeviceList
  {
    #region Data Members
    private readonly object objLock = new object();
    private readonly List<IInputDevice> registered = null;
    private readonly RawInput.RawInput rawInput = null;
    #endregion

    #region Public Constructors
    public InputDeviceList(IntPtr handle)
    {
      registered = new List<IInputDevice>();

      rawInput = new Vido.RawInput.RawInput(handle);
      rawInput.AddMessageFilter();
      rawInput.Keyboard.DevicesChanged += Keyboard_DevicesChanged;
      rawInput.Keyboard.EnumerateDevices();
    }
    #endregion

    public ICollection<IInputDevice> AllDevices()
    {
      throw new NotImplementedException();
    }

    public IDisposable Register(IInputDevice device)
    {
      if (!registered.Contains(device))
      {
        registered.Add(device);
      }

      return (new Deregister<IInputDevice>(device, (x) =>
      {
        if (registered.Contains(x))
        {
          registered.Remove(x);
        }
      }));
    }

    #region Event Handlers
    private void Keyboard_DevicesChanged(object sender, EventArgs e)
    {
      var args = e as RawInput.Events.DevicesChangedEventArgs;

      if (args.OldDevices != null)
      {
        foreach (var keyboard in args.OldDevices)
        {
          keyboard.KeyDown -= keyboard_KeyDown;
        }
      }

      if (args.NewDevices != null)
      {
        foreach (var keyboard in args.NewDevices)
        {
          keyboard.KeyDown += keyboard_KeyDown;
        }
      }
    }

    private void keyboard_KeyDown(object sender, EventArgs e)
    {
      var args = e as Vido.RawInput.Events.KeyEventArgs;
      var keyboard = sender as Vido.RawInput.IKeyboard;

      foreach (var reg in registered)
      {
        if (keyboard.Name.Contains(reg.Name))
        {
          (reg as InputDevice).Push((byte)args.KeyValue);
        }
      }
    }
    #endregion
  }
}
