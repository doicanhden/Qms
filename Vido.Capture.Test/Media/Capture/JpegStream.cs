namespace Vido.Media.Capture
{
  using System;
  using System.Diagnostics;
  using System.Drawing;
  using System.IO;
  using System.Net;
  using System.Threading;

  public class JpegStream : ICapture
  {
    #region Data Members
    private const int readSize = 1024;
    private const int bufSize = 512 * 1024;

    private readonly ManualResetEvent stopEvent = new ManualResetEvent(false);
    private Thread thread = null;

    private IImageHolder currentFrame = null;
    private int framesReceived = 0;
    #endregion

    #region Public Events
    /// <summary>
    /// Sự kiện kích hoạt khi có khung hình mới từ thiết bị.
    /// </summary>
    public event EventHandler NewFrame;

    /// <summary>
    /// Sự kiện kích hoại khi có lỗi xảy ra với thiết bị.
    /// </summary>
    public event EventHandler ErrorOccurred;
    #endregion

    #region Properties
    public Configuration Configuration { get; set; }
    public int FramesReceived
    {
      get { return (framesReceived); }
    }
    #endregion

    #region Constructors
    public JpegStream()
    {
      currentFrame = new BitmapImageHolder();
    }
    #endregion

    #region Public Methods
    public bool Start()
    {
      if (Configuration != null && thread == null)
      {
        framesReceived = 0;

        stopEvent.Reset();

        thread = new Thread(new ThreadStart(WorkerThread));
        thread.Name = Configuration.Source;
        thread.IsBackground = true;
        thread.Start();

        return (true);
      }

      return (false);
    }

    public void Stop()
    {
      if (thread != null)
      {
        if (!thread.Join(0))
        {
          stopEvent.Set();
          thread.Join();
        }

        thread = null;
      }
    }
    
    public IImageHolder Take()
    {
      return (currentFrame.Copy());
    }
    #endregion

    #region Private Methods
    private void RaiseErrorOccurred(int code, string message)
    {
      if (ErrorOccurred != null)
      {
        ErrorOccurred(this, new ErrorOccurredEventArgs(code, message));
      }
    }

    private void WorkerThread()
    {
      byte[] buffer = new byte[bufSize];

      HttpWebRequest request = null;
      WebResponse response = null;
      Stream stream = null;
      Random rnd = new Random((int) DateTime.Now.Ticks);
      DateTime start;

      while (true)
      {
        int  read, total = 0;

        try
        {
          start = DateTime.Now;

          request = (HttpWebRequest)WebRequest.Create(
            Configuration.Source + ((Configuration.Source.IndexOf('?') == -1) ? '?' : '&') + "fake=" + rnd.Next().ToString());

          if (!string.IsNullOrEmpty(Configuration.Username) && Configuration.Password != null)
            request.Credentials = new NetworkCredential(Configuration.Username, Configuration.Password);

          response = request.GetResponse();

          stream = response.GetResponseStream();
          while (!stopEvent.WaitOne(0, true))
          {
            if (total > bufSize - readSize)
            {
              total = 0;
            }

            if ((read = stream.Read(buffer, total, readSize)) == 0)
              break;

            total += read;
          }

          if (!stopEvent.WaitOne(0, true))
          {
            ++framesReceived;

            using (Stream memoryStream = new MemoryStream(buffer, 0, total))
            {
              currentFrame.Load(memoryStream);
            }

            if (NewFrame != null)
            {
              NewFrame(this, new NewFrameEventArgs(currentFrame));
            }
          }

          if (Configuration.FrameInterval > 0)
          {
            int msec = Configuration.FrameInterval - (int)DateTime.Now.Subtract(start).TotalMilliseconds;

            while ((msec > 0) && (stopEvent.WaitOne(0, true) == false))
            {
              Thread.Sleep((msec < 100) ? msec : 100);
              msec -= 100;
            }
          }
        }
        catch (WebException ex)
        {
          Debug.WriteLine("JpegStream: " + ex.Message);
          Thread.Sleep(250);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("JpegStream: " + ex.Message);
        }
        finally
        {
          if (request != null)
          {
            request.Abort();
            request = null;
          }

          if (stream != null)
          {
            stream.Close();
            stream = null;
          }

          if (response != null)
          {
            response.Close();
            response = null;
          }
        }

        if (stopEvent.WaitOne(0, true))
          break;
      }
    }
    #endregion

    #region Implementation of IDisposable
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        // dispose managed resources
        stopEvent.Dispose();
      }
      // free native resources
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion
  }
}