// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  public interface IDailyDirectory : IFileStorage
  {
    /// <summary>
    /// Đường dẫn thư mục gốc
    /// </summary>
    string RootDirectoryName { get; set; }

    /// <summary>
    /// Lấy đường dẫn tệp
    /// </summary>
    /// <param name="time">Thời gian</param>
    /// <param name="fileName">Tên tệp tin</param>
    /// <returns></returns>
    string GetPath(System.DateTime time, string fileName);
  }
}