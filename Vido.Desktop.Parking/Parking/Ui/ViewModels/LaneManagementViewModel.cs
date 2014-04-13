namespace Vido.Parking.Ui.ViewModels
{
  using System.Configuration;
  using System.Data;
  using System.Diagnostics;
  using System.Windows;
  using System.Windows.Input;


  public class LaneManagementViewModel
  {
    private MainViewModel mainViewModel;
    private ICommand saveCommand = null;
    private ICommand exitCommand = null;

    public LaneManagementViewModel(MainViewModel mainViewModel)
    {
      this.mainViewModel = mainViewModel;
      this.LaneConfigs = mainViewModel.Settings.LaneConfigs;
    }

    public DataTable LaneConfigs { get; set; }

    public ICommand SaveCommand
    {
      get
      {
        return (saveCommand ?? (saveCommand =
          new Commands.RelayCommand(SaveExecute, SaveCanExecute)));
      }
    }
    private void SaveExecute(object obj)
    {
      this.mainViewModel.Settings.Save();

      var view = obj as Window;
      if (view != null)
      {
        view.Close();
      }
      /// TODO: Địa phương hóa chuỗi thông báo.
      MessageBox.Show("Khởi động lại ứng dụng để áp dụng thay đổi.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

      /// TODO: Khởi động lại ứng dụng.
      Process.Start(Application.ResourceAssembly.Location);
      Application.Current.Shutdown();
    }

    private bool SaveCanExecute(object obj)
    {
      foreach (DataRow dr in LaneConfigs.Rows)
      {
        foreach (DataColumn col in LaneConfigs.Columns)
        {
          if (string.IsNullOrWhiteSpace(dr[col.ColumnName].ToString()))
          {
            return (false);
          }
        }
      }
      return (true);
    }

    public ICommand ExitCommand
    {
      get
      {
        return (exitCommand ?? (exitCommand =
          new Commands.RelayCommand(ExitExecute)));
      }
    }

    private void ExitExecute(object obj)
    {
      var view = obj as Window;
      view.Close();
    }
  }
}