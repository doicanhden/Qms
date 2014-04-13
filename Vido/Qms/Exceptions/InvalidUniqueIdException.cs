
// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms.Exceptions
{
  using System;

  public class InvalidUniqueIdException : Exception
  {
    public InvalidUniqueIdException()
      :base("Định danh duy nhất không hợp lệ")
    {
      /// TODO: Địa phương hóa chuỗi thông báo.
    }

    public InvalidUniqueIdException(string message)
      :base(message)
    {
    }
  }
}
