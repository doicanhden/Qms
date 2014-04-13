// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  public enum UniqueIdState
  {
    Using = 0,
    Error = 1,
    Block = 2,
    NotExist = 3
  }

  public interface IUniqueIdStorage
  {
    #region Public Methods
    bool Insert(string uniqueId);
    bool Remove(string uniqueId);
    bool CanUse(string uniqueId);

    UniqueIdState Status(string uniqueId);
    #endregion
  }
}