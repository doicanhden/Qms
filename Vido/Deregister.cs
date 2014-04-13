// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido
{
  using System;

  public class Deregister<T> : IDisposable
    where T: class
  {
    private readonly T obj;
    private readonly Action<T> deregister;

    public Deregister(T obj, Action<T> deregister)
    {
      this.obj = obj;
      this.deregister = deregister;
    }

    public void Dispose()
    {
      if (deregister != null)
      {
        deregister(obj);
      }
    }
  }
}
