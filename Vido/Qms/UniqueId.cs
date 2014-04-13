// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  public class UniqueId : IUniqueId
  {
    string IUniqueId.UniqueId { get; set; }

    public UniqueId(string uniqueId)
    {
      (this as IUniqueId).UniqueId = uniqueId;
    }
  }
}
