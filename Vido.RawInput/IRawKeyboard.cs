namespace Vido.RawInput
{
  using System;
  using System.Collections.Generic;
  using Vido.RawInput.Events;

  public interface IRawKeyboard
  {
    event EventHandler DevicesChanged;
    ICollection<IKeyboard> Devices { get; }

    int EnumerateDevices();
  }
}
