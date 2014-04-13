namespace Vido.Parking.Ui.Utilities
{
  using System;
  using System.Diagnostics;

  public static class Database
  {
    public static bool TestConnection()
    {
      try
      {
        VidoParkingEntities entities = new VidoParkingEntities();

        return (entities.Database.Exists());
      }
      catch (Exception ex)
      {
        Debug.WriteLine("TestConnection(): " + ex.Message);
        return (false);
      }
    }
  }
}
