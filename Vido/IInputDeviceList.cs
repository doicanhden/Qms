// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido
{
  using System;
  using System.Collections.Generic;

  public interface IInputDeviceList
  {
    #region Methods
    ICollection<IInputDevice> AllDevices();
    IDisposable Register(IInputDevice device);
    #endregion
  }
}