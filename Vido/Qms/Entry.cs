// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  using System;
  using System.Text;

  /// <summary>
  /// Bảng ghi thông tin vào Cổng
  /// </summary>
  public class Entry
  {
    #region Public Properties
    /// <summary>
    /// Thời gian vào Cổng
    /// </summary>
    public DateTime EntryTime { get; set; }

    /// <summary>
    /// Tên Cổng vào
    /// </summary>
    public string EntryGate { get; set; }

    /// <summary>
    /// Định danh duy nhất
    /// </summary>
    public string UniqueId { get; set; }

    /// <summary>
    /// Dữ liệu người dùng
    /// </summary>
    public string UserData { get; set; }

    /// <summary>
    /// Đường dẫn đến ảnh chụp thứ nhất
    /// </summary>
    public string FirstImage { get; set; }

    /// <summary>
    /// Đường dẫn đến ảnh chụp thứ hai
    /// </summary>
    public string SecondImage { get; set; }
    #endregion

    #region Public Constructors
    public Entry()
    {
    }
    #endregion
  }
}