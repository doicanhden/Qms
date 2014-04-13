// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Media
{
  using System;
  using System.IO;

  public interface IImageHolder
  {
    bool Available { get; }

    bool Load(IFileStorage storage, string fileName);
    bool Save(IFileStorage storage, string fileName);
    bool Load(Stream stream);
    bool Save(Stream stream);

    IImageHolder Copy();
  }
}
