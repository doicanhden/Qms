// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Media.Capture
{
  using System;

  /// <summary>
  /// Tham số cho sự kiện Khung hình mới trên thiết bị Chụp ảnh
  /// </summary>
  public class NewFrameEventArgs : EventArgs
  {
    #region Public Properties
    /// <summary>
    /// Khung hình mới từ thiết bị Chụp ảnh
    /// </summary>
    public IImageHolder Image { get; private set; }
    #endregion

    #region Public Constructors
    public NewFrameEventArgs(IImageHolder image)
    {
      this.Image = image;
    }
    #endregion
  }
}