// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido
{
  using System;

  public class DataInEventArgs : EventArgs
  {
    #region Public Properties
    public byte[] Data { get; set; }

    public bool Printable { get; set; }
    #endregion

    #region Public Constructors
    public DataInEventArgs(byte[] data, bool printable = false)
    {
      this.Data = data;
      this.Printable = printable;
    }
    #endregion
  }
}