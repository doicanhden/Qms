namespace Vido.Parking.Ui.Models
{
  using System;
  using System.Diagnostics;
  using System.Linq;

  public class SwitchShifts
  {
    private readonly VidoParkingEntities entities = new VidoParkingEntities();

    public int NumberOfUnusedCards
    {
      get
      {
        try
        {
          var usedCards = (from InOut in entities.InOutRecord
                          where InOut.OutTime == null &&
                                InOut.OutLaneCode == null &&
                                InOut.OutBackImg == null &&
                                InOut.OutFrontImg == null &&
                                InOut.OutLaneCode == null &&
                                InOut.OutEmployeeId == null
                          select InOut.CardId).Distinct();

          return (entities.Card.Count((x) => !usedCards.Contains(x.CardId)));
        }
        catch (Exception ex)
        {
          Debug.WriteLine("SwitchShifts.NumberOfUnusedCards: " + ex.Message);
          return (-1);
        }
      }
    }

    public int NumberOfVehiclesNotOut
    {
      get
      {
        try
        {
          return (entities.InOutRecord.Count((io) =>
            io.OutTime == null &&
            io.OutLaneCode == null &&
            io.OutBackImg == null &&
            io.OutFrontImg == null &&
            io.OutLaneCode == null &&
            io.OutEmployeeId == null));
        }
        catch (Exception ex)
        {
          Debug.WriteLine("SwitchShifts.NumberOfVehiclesNotOut: " + ex.Message);
          return (-1);
        }
      }
    }
  }
}
