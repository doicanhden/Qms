
namespace Vido
{
  using System;
  using System.Collections.Generic;

  public class InputDevice : IInputDevice
  {
    #region Data Members
    private readonly List<byte> buffer = new List<byte>();
    #endregion

    public event EventHandler DataIn;
    public string Name { get; set; }
    public byte EndKey { get; set; }
    public IDisposable Deregister { get; set; }

    internal void Push(byte data)
    {
      if (data == EndKey)
      {
        if (DataIn != null)
        {
          DataIn(this, new DataInEventArgs(buffer.ToArray(), true));
        }

        buffer.Clear();
      }
      else
      {
        buffer.Add(data);
      }
    }
  }
}