// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido
{
  using System.Text;

  public enum PrintResult
  {
    Success = 0
  }

  public interface IPrinter
  {
    #region Properties
    string Name { get; set; }
    #endregion

    #region Methods
    PrintResult Print(byte[] data);
    PrintResult Print(byte[] data, Encoding encoding);
    #endregion
  }
}
