// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  public class RandomUniqueIdStorage : IUniqueIdStorage
  {
    public bool Insert(string uniqueId)
    {
      return (true);
    }

    public bool Remove(string uniqueId)
    {
      return (true);
    }

    public bool CanUse(string uniqueId)
    {
      return (true);
    }

    public UniqueIdState Status(string uniqueId)
    {
      return (UniqueIdState.Using);
    }
  }
}
