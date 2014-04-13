// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms.Exceptions
{
  using System;

  public class SystemErrorException : Exception
  {
    public SystemErrorException()
      :base("Lỗi hệ thống")
    {
      /// TODO: Địa phương hóa chuỗi thông báo.
    }

    public SystemErrorException(Exception innerException)
      :base("Lỗi hệ thống", innerException)
    {
    }

    public SystemErrorException(string message)
      :base(message)
    {
    }

    public SystemErrorException(string message, Exception innerException)
      :base(message, innerException)
    {
    }
  }
}