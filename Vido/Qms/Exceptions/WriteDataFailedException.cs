
// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms.Exceptions
{
  using System;

  public class WriteDataFailedException : SystemErrorException
  {
    public WriteDataFailedException()
      :base("Ghi dữ liệu thất bại")
    {
      /// TODO: Địa phương hóa chuỗi thông báo.
    }

    public WriteDataFailedException(string message)
      :base(message)
    {
    }
  }
}
