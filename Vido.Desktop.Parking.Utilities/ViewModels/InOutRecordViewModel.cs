using System.Windows;
using System.Windows.Input;
using Vido.VidoParkingDataSetTableAdapters;
namespace Vido.Parking.Ms.Ui.Wpf.ViewModels
{
  public class InOutRecordViewModel : NotificationObject
  {
    private ICommand saveCommand = null;
    private VidoParkingDataSet.InOutRecordDataTable inOutRecords;

    public InOutRecordViewModel()
    {
      inOutRecords = new VidoParkingDataSet.InOutRecordDataTable();

      InOutRecordTableAdapter inOutRecordTableAdapter = new InOutRecordTableAdapter();
      inOutRecordTableAdapter.Fill(inOutRecords);
    }

    public VidoParkingDataSet.InOutRecordDataTable InOutRecords
    {
      get { return (inOutRecords); }
      set
      {
        inOutRecords = value;
        RaisePropertyChanged(() => InOutRecords);
      }
    }

    public ICommand SaveCommand
    {
      get { return (saveCommand ?? (saveCommand = new Commands.RelayCommand(SaveExecute))); }
    }

    private void SaveExecute(object obj)
    {
      InOutRecordTableAdapter inOutRecordTableAdapter = new InOutRecordTableAdapter();
      inOutRecordTableAdapter.Update(inOutRecords);

      /// TODO: Địa phương hóa chuỗi thông báo
      MessageBox.Show("Thay đổi đã được lưu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
    }
  }
}
