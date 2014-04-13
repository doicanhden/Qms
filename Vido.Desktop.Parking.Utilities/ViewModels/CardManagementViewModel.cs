namespace Vido.Parking.Ms.Ui.Wpf.ViewModels
{
  using System.Data;
  using System.Windows;
  using System.Windows.Input;
  using VidoParkingDataSetTableAdapters;

  public class CardManagementViewModel : NotificationObject
  {
    private ICommand saveCommand = null;
    private VidoParkingDataSet.CardDataTable cards;

    public CardManagementViewModel()
    {
      cards = new VidoParkingDataSet.CardDataTable();

      CardTableAdapter cardTableAdapter = new CardTableAdapter();
      cardTableAdapter.Fill(cards);
    }

    public VidoParkingDataSet.CardDataTable Cards
    {
      get { return (cards); }
      set
      {
        cards = value;
        RaisePropertyChanged(() => Cards);
      }
    }

    public ICommand SaveCommand
    {
      get { return (saveCommand ?? (saveCommand = new Commands.RelayCommand(SaveExecute))); }
    }

    private void SaveExecute(object obj)
    {
      CardTableAdapter cardTableAdapter = new CardTableAdapter();
      cardTableAdapter.Update(cards);

      /// TODO: Địa phương hóa chuỗi thông báo
      MessageBox.Show("Thay đổi đã được lưu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
    }
  }
}
