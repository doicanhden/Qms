namespace Vido.Media.Capture
{
  using System;
  using System.Diagnostics;
  using System.Drawing;
  using System.IO;
  using System.Net;
  using System.Text;
  using System.Threading;
  using Vido.Desktop;

  public class MJpegStream : ICapture
  {
    #region Data Members
    private const int readSize = 1024;
    private const int bufSize = 512 * 1024;

    private readonly ManualResetEvent stopEvent = new ManualResetEvent(false);
    private readonly ManualResetEvent reloadEvent = new ManualResetEvent(false);
    private Thread thread = null;

    private IImageHolder currentFrame = null;
    private int framesReceived = 0;
    private Configuration configs = null;
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

    #region Public Properties
    public Configuration Configuration
    {
      get { return (configs); }
      set
      {
        configs = value;

        if (thread != null)
        {
          reloadEvent.Set();
        }
      }
    }
    public int FramesReceived
    {
      get
      {
        int frames = framesReceived;
        framesReceived = 0;
        return frames;
      }
    }
    #endregion

    #region Public Constructors
    public MJpegStream()
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

        reloadEvent.Reset();
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
      byte[] buffer = new byte[bufSize];  // buffer to read stream

      while (true)
      {
        reloadEvent.Reset();

        HttpWebRequest request = null;
        WebResponse response = null;
        Stream stream = null;
        byte[] delimiter = null;
        byte[] delimiter2 = null;
        byte[] boundary = null;
        int boundaryLen, delimiterLen = 0, delimiter2Len = 0;
        int read, todo = 0, total = 0, pos = 0, align = 1;
        int start = 0, stop = 0;

        // align
        //  1 = searching for image start
        //  2 = searching for image end
        try
        {
          request = (HttpWebRequest) WebRequest.Create(Configuration.Source);

          if (!string.IsNullOrEmpty(Configuration.Username) && Configuration.Password != null)
            request.Credentials = new NetworkCredential(Configuration.Username, Configuration.Password);

          response = request.GetResponse();

          string ct = response.ContentType;
          if (ct.IndexOf("multipart/x-mixed-replace") == -1)
            throw new ApplicationException("Invalid URL");

          ASCIIEncoding encoding = new ASCIIEncoding();
          boundary = encoding.GetBytes(ct.Substring(ct.IndexOf("boundary=", 0) + 9));
          boundaryLen = boundary.Length;

          stream = response.GetResponseStream();

          while ((!stopEvent.WaitOne(0, true)) && (!reloadEvent.WaitOne(0, true)))
          {
            if (total > bufSize - readSize)
            {
              total = pos = todo = 0;
            }

            if ((read = stream.Read(buffer, total, readSize)) == 0)
              throw new ApplicationException();

            total += read;
            todo += read;

            if (delimiter == null)
            {
              // find boundary
              pos = ByteArrayUtils.Find(buffer, boundary, pos, todo);

              if (pos == -1)
              {
                // was not found
                todo = boundaryLen - 1;
                pos = total - todo;
                continue;
              }

              todo = total - pos;

              if (todo < 2)
                continue;

              // check new line delimiter type
              if (buffer[pos + boundaryLen] == 10)
              {
                delimiterLen = 2;
                delimiter = new byte[2] {10, 10};
                delimiter2Len = 1;
                delimiter2 = new byte[1] {10};
              }
              else
              {
                delimiterLen = 4;
                delimiter = new byte[4] {13, 10, 13, 10};
                delimiter2Len = 2;
                delimiter2 = new byte[2] {13, 10};
              }

              pos += boundaryLen + delimiter2Len;
              todo = total - pos;
            }

            // search for image
            if (align == 1)
            {
              start = ByteArrayUtils.Find(buffer, delimiter, pos, todo);
              if (start != -1)
              {
                // found delimiter
                start  += delimiterLen;
                pos    = start;
                todo  = total - pos;
                align  = 2;
              }
              else
              {
                // delimiter not found
                todo  = delimiterLen - 1;
                pos    = total - todo;
              }
            }

            // search for image end
            while ((align == 2) && (todo >= boundaryLen))
            {
              stop = ByteArrayUtils.Find(buffer, boundary, pos, todo);
              if (stop != -1)
              {
                pos  = stop;
                todo = total - pos;

                // increment frames counter
                ++framesReceived;

                using (Stream memoryStream = new MemoryStream(buffer, start, stop - start))
                {
                  currentFrame.Load(memoryStream);
                }

                if (NewFrame != null)
                {
                  NewFrame(this, new NewFrameEventArgs(currentFrame));
                }

                // shift array
                pos = stop + boundaryLen;
                todo = total - pos;
                Array.Copy(buffer, pos, buffer, 0, todo);

                pos = 0;
                total = todo;
                align = 1;
              }
              else
              {
                // delimiter not found
                todo = boundaryLen - 1;
                pos  = total - todo;
              }
            }
          }
        }
        catch (WebException ex)
        {
          Debug.WriteLine("MJpegStream: " + ex.Message);
          Thread.Sleep(250);
        }
        catch (ApplicationException ex)
        {
          Debug.WriteLine("MJpegStream: " + ex.Message);
          Thread.Sleep(250);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("MJpegStream: " + ex.Message);
        }
        finally
        {
          // abort request
          if (request != null)
          {
            request.Abort();
            request = null;
          }
          // close response stream
          if (stream != null)
          {
            stream.Close();
            stream = null;
          }
          // close response
          if (response != null)
          {
            response.Close();
            response = null;
          }
        }

        // need to stop ?
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
        reloadEvent.Dispose();
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