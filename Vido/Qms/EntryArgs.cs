// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  using Vido.Media;

  /// <summary>
  /// Bảng ghi thông tin vào Cổng. (tiền xử lý)
  /// </summary>
  public class EntryArgs
  {
    #region Public Properties
    /// <summary>
    /// Cổng
    /// </summary>
    public IGate Gate { get; set; }

    /// <summary>
    /// Định danh duy nhất
    /// </summary>
    public IUniqueId UniqueId { get; set; }

    /// <summary>
    /// Dữ liệu người dùng
    /// </summary>
    public IUserData UserData { get; set; }

    /// <summary>
    /// Ảnh chụp đối tượng vào cổng
    /// </summary>
    public ImagePair Images { get; set; }
    #endregion

    #region Public Constructors
    public EntryArgs()
    {
    }
    #endregion
  }
}
