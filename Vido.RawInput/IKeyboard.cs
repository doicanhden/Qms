namespace Vido.RawInput
{
  using System;
  using Vido.RawInput.Events;

  public interface IKeyboard
  {
    event EventHandler KeyUp;
    event EventHandler KeyDown;

    string Description { get; set; }
    string Name { get; set; }
    string Type { get; set; }
    IntPtr Handle { get; set; }
  }
}
