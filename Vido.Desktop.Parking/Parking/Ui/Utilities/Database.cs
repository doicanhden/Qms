namespace Vido.Parking.Ui.Utilities
{
  using System;
  using System.Diagnostics;

  public static class Database
  {
    public static bool TestConnection()
    {
      VidoParkingEntities entities = new VidoParkingEntities();

      return (entities.Database.Exists());
    }
  }
}
