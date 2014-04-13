// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Media.Capture
{
  public class Configuration
  {
    #region Public Properties
    /// <summary>
    /// Tên đăng nhập thiết bị
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Mật khẩu đăng nhập thiết bị.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Nguồn cung cấp khung hình.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Định dạng nén dữ liệu Video.
    /// </summary>
    public Coding Coding { get; set; }

    /// <summary>
    /// Thời gian giữa mỗi khung hình.
    /// </summary>
    public int FrameInterval { get; set;}
    #endregion
  }
}