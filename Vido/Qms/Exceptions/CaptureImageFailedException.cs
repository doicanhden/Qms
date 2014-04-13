// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms.Exceptions
{
  public class CaptureImageFailedException : SystemErrorException
  {
    public CaptureImageFailedException()
      :base("Chụp ảnh thất bại")
    {
      /// TODO: Địa phương hóa chuỗi thông báo.
    }

    public CaptureImageFailedException(string message)
      :base(message)
    {
    }
  }
}