namespace Vido.Parking.Ui.ViewModels
{
  using System.Windows.Controls;
  using System.Windows.Input;
  using Vido.Parking.Ui.Commands;
  using Vido.Parking.Ui.Models;

  public class SwitchShiftsViewModel : NotificationObject
  {
    private readonly SwitchShifts model = new SwitchShifts();
    private long numberOfVehiclesNotOut;
    private long numberOfUnusedCards;
    private ICommand printCommand;

    public long NumberOfVehiclesNotOut
    {
      get { return (numberOfVehiclesNotOut); }
      set
      {
        numberOfVehiclesNotOut = value;
        RaisePropertyChanged(() => NumberOfVehiclesNotOut);
      }
    }

    public long NumberOfUnusedCards
    {
      get { return (numberOfUnusedCards); }
      set
      {
        numberOfUnusedCards = value;
        RaisePropertyChanged(() => NumberOfUnusedCards);
      }
    }

    public ICommand PrintCommand
    {
      get
      {
        return (printCommand ?? (printCommand = new RelayCommand<Grid>((x) =>
        {
          PrintDialog dialog = new PrintDialog();

          if (dialog.ShowDialog() != true)
            return;

          dialog.PrintVisual(x, "A WPF printing");
        })));
      }
    }

    public SwitchShiftsViewModel()
    {
      NumberOfUnusedCards = model.NumberOfUnusedCards;
      NumberOfVehiclesNotOut = model.NumberOfVehiclesNotOut;
    }
  }
}
