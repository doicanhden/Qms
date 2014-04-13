namespace Vido.Qms
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Text;

  public class DailyDirectory : IDailyDirectory
  {
    #region Data Members
    private string formatString;
    #endregion

    #region Public Constructors
    public DailyDirectory(string formatString = @"yyyy\MM\dd")
    {
      if (Path.DirectorySeparatorChar == '\\')
      {
        this.formatString = formatString.Replace(@"\", @"\\");
      }
      else
      {
        this.formatString = formatString;
      }
    }
    #endregion

    #region Implementation of IDailyDirectory
    public string RootDirectoryName { get; set; }

    public string GetPath(DateTime time, string fileName)
    {
      var directoryName = time.ToString(formatString);

      CreateDirectory(Path.Combine(RootDirectoryName, directoryName));

      return (Path.Combine(directoryName, fileName));
    }
    #endregion

    #region Implementation of IFileStorage
    Stream IFileStorage.Open(string fileName)
    {
      if (string.IsNullOrWhiteSpace(fileName))
        return (null);

      var path = Path.Combine(RootDirectoryName, fileName);
      return (File.Open(path, FileMode.OpenOrCreate));
    }

    bool IFileStorage.Exists(string fileName)
    {
      if (string.IsNullOrWhiteSpace(fileName))
        return (false);

      var path = Path.Combine(RootDirectoryName, fileName);
      return (File.Exists(path));
    }

    void IFileStorage.Delete(string fileName)
    {
      if (string.IsNullOrWhiteSpace(fileName))
        return;

      var path = Path.Combine(RootDirectoryName, fileName);
      File.Delete(path);
    }
    #endregion

    #region Private Methods
    private bool CreateDirectory(string directoryName)
    {
      try
      {
        if (!Directory.Exists(directoryName))
        {
          Directory.CreateDirectory(directoryName);
        }

        return (true);
      }
      catch (Exception ex)
      {
        Debug.WriteLine("CreateDirectory" + ex.Message);
        return (false);
      }
    }
    #endregion
  }
}