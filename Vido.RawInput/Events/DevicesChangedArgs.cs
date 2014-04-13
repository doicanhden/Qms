namespace Vido.RawInput.Events
{
  using System;
  using System.Collections.Generic;
  using Vido.RawInput;

  public class DevicesChangedEventArgs : EventArgs
  {
    public IList<IKeyboard> OldDevices { get; private set; }
    public IList<IKeyboard> NewDevices { get; private set; }

    public DevicesChangedEventArgs(IList<IKeyboard> oldDevices, IList<IKeyboard> newDevices)
    {
      this.OldDevices = oldDevices;
      this.NewDevices = newDevices;
    }
  }
}
