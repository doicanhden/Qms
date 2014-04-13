// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms.Exceptions
{
  using System;

  public class RecorderFullException : Exception
  {
    public RecorderFullException()
      :base("Nơi chứa đã đầy")
    {
      /// TODO: Địa phương hóa chuỗi thông báo.
    }

    public RecorderFullException(string message)
      :base(message)
    {
    }
  }
}
