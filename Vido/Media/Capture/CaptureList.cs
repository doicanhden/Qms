// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Media.Capture
{
  using System;
  using System.Collections.Generic;

  public class CaptureList : ICaptureList, IDisposable
  {
    #region Data Members
    private readonly IList<ICapture> captures = new List<ICapture>();
    #endregion

    #region Public Constructors
    /// <summary>
    /// Khởi tạo danh sách các thiết bị Chụp ảnh
    /// </summary>
    /// <param name="factory">Đối tượng tạo các thiết bị Chụp ảnh</param>
    public CaptureList(ICaptureFactory factory)
    {
      if (factory == null)
      {
        throw new ArgumentNullException("captureFactory");
      }

      this.Factory = factory;
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Đối tượng sử dụng để tạo các thiết bị Chụp ảnh
    /// </summary>
    public ICaptureFactory Factory { get; private set; }
    #endregion

    #region Implementation of ICaptureList
    /// <summary>
    /// Danh sách thiết bị đã được tạo bởi phương thức Create()
    /// </summary>
    public ICollection<ICapture> Captures
    {
      get { return (captures); }
    }

    /// <summary>
    /// Tạo thiết bị Chụp ảnh và thêm vào danh sách thiết bị.
    /// </summary>
    /// <param name="configs">Cấu hình thiết bị Chụp ảnh</param>
    /// <returns></returns>
    public ICapture Create(Configuration configs)
    {
      var capture = Factory.Create(configs);
      if (capture != null)
      {
        captures.Add(capture);
      }

      return (capture);
    }
    #endregion

    #region Implementation of IDisposable
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        // dispose managed resources
        foreach (var capture in Captures)
        {
          capture.Stop();
          capture.Dispose();
        }
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion
  }
}