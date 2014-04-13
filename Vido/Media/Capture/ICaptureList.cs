// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Media.Capture
{
  using System.Collections.Generic;

  public interface ICaptureList : ICaptureFactory
  {
    ICollection<ICapture> Captures { get; }
  }
}