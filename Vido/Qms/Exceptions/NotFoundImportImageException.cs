
// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms.Exceptions
{
  using System;
  public class NotFoundImportImageException : Exception
  {
    public NotFoundImportImageException()
      :base("Không tìm thấy ảnh đối tượng Vào")
    {
      /// TODO: Địa phương hóa chuỗi thông báo.
    }

    public NotFoundImportImageException(string message)
      :base(message)
    {
    }
  }
}