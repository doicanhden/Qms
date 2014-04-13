// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms.Exceptions
{
  using System;

  public class SaveImageFailedException : SystemErrorException
  {
    public SaveImageFailedException()
      : base("Lưu hình ảnh thất bại")
    {
      /// TODO: Địa phương hóa chuỗi thông báo.
    }

    public SaveImageFailedException(string message)
      :base(message)
    {
    }
  }
}
