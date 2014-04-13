namespace Vido.Parking.Ui.ViewModels
{
  using System.Configuration;
  using System.Diagnostics;
  using System.Windows;
  using System.Windows.Input;


  public class ConnectionStringEditorViewModel : NotificationObject
  {
    private string connectionString = null;
    private bool exitOnSave = true;
    private ICommand saveCommand = null;
    public string ConnectionString
    {
      get { return (connectionString); }
      set
      {
        connectionString = value;
        RaisePropertyChanged(() => ConnectionString);
      }
    }
    public ConnectionStringEditorViewModel(bool exitOnSave = true)
    {
      this.ConnectionString = ConfigurationManager.ConnectionStrings["VidoParkingEntities"].ConnectionString;
      this.exitOnSave = exitOnSave;
    }

    public ICommand SaveCommand
    {
      get
      {
        return (saveCommand ?? (saveCommand = new Commands.RelayCommand(
          (x) =>
          {
            AddAndSaveOneConnectionStringSettings(
              ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location),
              new ConnectionStringSettings("VidoParkingEntities", ConnectionString, "System.Data.EntityClient"));

            var view = x as Window;
            if (view != null)
            {
              view.DialogResult = true;
              view.Close();
            }

            if (exitOnSave)
            {
              /// TODO: Địa phương hóa chuỗi thông báo.
              MessageBox.Show("Khởi động lại ứng dụng để áp dụng thay đổi.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

              /// TODO: Khởi động lại ứng dụng.
              Process.Start(Application.ResourceAssembly.Location);
              Application.Current.Shutdown();
            }
          },
          (x) => (!string.IsNullOrWhiteSpace(ConnectionString))
        )));
      }
    }
    /// <summary>
    /// Adds a connection string settings entry & saves it to the associated config file.
    ///
    /// This may be app.config, or an auxiliary file that app.config points to or some
    /// other xml file.
    /// ConnectionStringSettings is the confusing type name of one entry including: 
    /// name + connection string + provider entry
    /// </summary>
    /// <param name="configuration">Pass in ConfigurationManager.OpenMachineConfiguration, 
    /// ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None) etc. </param>
    /// <param name="connectionStringSettings">The entry to add</param>

    public static void AddAndSaveOneConnectionStringSettings(
      Configuration configuration,
      ConnectionStringSettings connectionStringSettings)
    {
      // You cannot add to ConfigurationManager.ConnectionStrings using
      // ConfigurationManager.ConnectionStrings.Add
      // (connectionStringSettings) -- This fails.

      // But you can add to the configuration section and refresh the ConfigurationManager.

      // Get the connection strings section; Even if it is in another file.
      ConnectionStringsSection connectionStringsSection = configuration.ConnectionStrings;

      // Add the new element to the section.
      connectionStringsSection.ConnectionStrings.Clear();
      connectionStringsSection.ConnectionStrings.Add(connectionStringSettings);

      // Save the configuration file.
      configuration.Save(ConfigurationSaveMode.Full);

      // This is this needed. Otherwise the connection string does not show up in
      // ConfigurationManager
      ConfigurationManager.RefreshSection("connectionStrings");
    }
  }
}
