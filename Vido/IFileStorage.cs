// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido
{
  using System.IO;

  public interface IFileStorage
  {
    Stream Open(string fileName);
    bool Exists(string fileName);
    void Delete(string fileName);
  }
}
