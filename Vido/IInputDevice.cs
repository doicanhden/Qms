// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido
{
  using System;

  public interface IInputDevice
  {
    #region Events
    event EventHandler DataIn;
    #endregion

    #region Properties
    string Name { get; set; }
    IDisposable Deregister { get; set; }
    #endregion
  }
}