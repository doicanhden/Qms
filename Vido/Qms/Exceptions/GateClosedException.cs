// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms.Exceptions
{
  using System;

  public class GateClosedException : Exception
  {
    public GateClosedException()
      :base("Cổng đã đóng")
    {
      /// TODO: Địa phương hóa chuỗi thông báo.
    }

    public GateClosedException(string message)
      :base(message)
    {
    }
  }
}