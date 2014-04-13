namespace Vido.Media.Capture
{
  public class CaptureFactory : ICaptureFactory
  {
    #region Public Methods
    /// <summary>
    /// Tạo đối tượng Capture và set Config.
    /// </summary>
    /// <param name="configs"></param>
    /// <returns></returns>
    public virtual ICapture Create(Configuration configs)
    {
      ICapture capture = null;
      if (configs != null)
      {
        switch (configs.Coding)
        {
          case Coding.Jpeg:
            capture = new JpegStream();
            break;
          case Coding.MJpeg:
            capture = new MJpegStream();
            break;
        }

        if (capture != null)
        {
          capture.Configuration = configs;
        }
      }

      return (capture);
    }
    #endregion
  }
}