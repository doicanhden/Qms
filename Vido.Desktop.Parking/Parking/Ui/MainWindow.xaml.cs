using System.Windows;
using Vido.Parking.Ui.ViewModels;
namespace Vido.Desktop.Parking.Ui
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      this.DataContext = new MainViewModel(this);
    }
  }
}
