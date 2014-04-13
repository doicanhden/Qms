// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Media.Capture
{
  public interface ICaptureFactory
  {
    #region Public Methods
    /// <summary>
    /// Tạo thiết bị Chụp ảnh mới dựa vào Cấu hình thiết bị
    /// </summary>
    /// <param name="configs">Cấu hình thiết bị Chụp ảnh</param>
    /// <returns>Đối tượng Chụp ảnh được tạo; nếu không thể tạo thiết bị: null/returns>
    ICapture Create(Configuration configs);
    #endregion
  }
}