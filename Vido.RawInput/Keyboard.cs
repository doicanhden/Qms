namespace Vido.RawInput
{
  using System;
  using Vido.RawInput.Events;

  public class Keyboard : IKeyboard
  {
    #region Public Events
    public event EventHandler KeyUp;
    public event EventHandler KeyDown;
    #endregion

    #region Public Properties
    public IntPtr Handle { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    #endregion

    #region Public Constructors
    public Keyboard()
    {
    }
    #endregion

    #region Internal Methods
    internal void RaiseKeyDown(int keyValue)
    {
      if (KeyDown != null)
      {
        KeyDown(this, new KeyEventArgs(keyValue));
      }
    }
    internal void RaiseKeyUp(int keyValue)
    {
      if (KeyUp != null)
      {
        KeyUp(this, new KeyEventArgs(keyValue));
      }
    }
    #endregion
  }
}
