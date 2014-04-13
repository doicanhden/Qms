namespace Vido.Parking.Ms.Ui.Wpf.ViewModels
{
  using System.Windows;
  using System.Windows.Input;

  public class MainWindowViewModel
  {
    private ICommand showCardTableCommand;
    private ICommand showInOutRecordTableCommand;

    public MainWindowViewModel(MainWindow mainWindow)
    {
      // TODO: Complete member initialization
      this.View = mainWindow;
    }

    public MainWindow View { get; set; }

    public ICommand ShowCardTableCommand
    {
      get { return (showCardTableCommand ?? (showCardTableCommand =
        new Commands.RelayCommand(ShowCardTableExecute))); }
    }

    private void ShowCardTableExecute(object obj)
    {
      new Views.CardManagementView()
      {
        Owner = View
      }.ShowDialog();
    }

    public ICommand ShowInOutRecordTableCommand
    {
      get { return (showInOutRecordTableCommand ?? (showInOutRecordTableCommand =
        new Commands.RelayCommand(ShowInOutRecordTableExecute))); }
    }

    private void ShowInOutRecordTableExecute(object obj)
    {
      new Views.InOutRecordView()
      {
        Owner = View
      }.ShowDialog();
    }

  }
}
