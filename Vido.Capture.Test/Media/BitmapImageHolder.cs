namespace Vido.Media
{
  using System;
  using System.Diagnostics;
  using System.Drawing;
  using System.IO;

  public class BitmapImageHolder : IImageHolder
  {
    #region Data Members
    private readonly object locker = new object();
    private Image image = null;
    #endregion

    #region Public Constructors
    public BitmapImageHolder()
    {
      this.image = null;
    }

    public BitmapImageHolder(Image image)
    {
      this.image = image;
    }
    #endregion

    #region Public Properties
    public bool Available
    {
      get
      {
        lock (locker)
        {
          return (this.image != null);
        }
      }
    }

    public Image Image
    {
      get
      {
        lock (locker)
        {
          return (image);
        }
      }
    }
    #endregion

    #region Public Methods
    public IImageHolder Copy()
    {
      lock (locker)
      {
        if (image != null)
        {
          var stream = new MemoryStream();
          this.Save(stream);

          var copy = new BitmapImageHolder();
          copy.Load(stream);

          return (copy);
        }

        return (null);
      }
    }

    public bool Load(IFileStorage storage, string fileName)
    {
      using (var stream = storage.Open(fileName))
      {
        return (Load(stream));
      }
    }
    public bool Save(IFileStorage storage, string fileName)
    {
      using (var stream = storage.Open(fileName))
      {
        return (Save(stream));
      }
    }

    public bool Load(Stream stream)
    {
      lock (locker)
      {
        try
        {
          image = Bitmap.FromStream(stream);

          return (true);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("BitmapImageHolder.Load(Stream): " + ex.Message);
          image = null;
          return (false);
        }
      }
    }
    public bool Save(Stream stream)
    {
      lock (locker)
      {
        try
        {
          if (image != null)
          {
            image.Save(stream, image.RawFormat);
            return (true);
          }

          return (false);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("BitmapImageHolder.Save(Stream): " + ex.Message);
          return (false);
        }
      }
    }
    #endregion
  }
}
