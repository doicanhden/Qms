// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms.Exceptions
{
  using System;

  public class EntryBlockedException : Exception
  {
    public EntryBlockedException()
      :base("Truy cập bị từ chối")
    {
      /// TODO: Địa phương hóa chuỗi thông báo.
    }

    public EntryBlockedException(string message)
      :base(message)
    {
    }
  }
}