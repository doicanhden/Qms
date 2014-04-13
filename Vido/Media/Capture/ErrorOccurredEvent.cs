// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Media.Capture
{
  public class ErrorOccurredEventArgs : System.EventArgs
  {
    #region Public Properties
    public string Message { get; private set; }
    public int Code { get; private set; }
    #endregion

    #region Public Constructors
    public ErrorOccurredEventArgs(int code)
    {
      this.Code = code;
      this.Message = string.Empty;
    }
    public ErrorOccurredEventArgs(string message)
    {
      this.Code = -1;
      this.Message = message;
    }
    public ErrorOccurredEventArgs(int code, string message)
    {
      this.Code = code;
      this.Message = message;
    }
    #endregion
  }
}
