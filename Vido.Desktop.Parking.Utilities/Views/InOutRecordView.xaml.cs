using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vido.Parking.Ms.Ui.Wpf.Views
{
  /// <summary>
  /// Interaction logic for InOutRecordView.xaml
  /// </summary>
  public partial class InOutRecordView : Window
  {
    public InOutRecordView()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {

      VidoParkingDataSet vidoParkingDataSet = ((VidoParkingDataSet)(this.FindResource("vidoParkingDataSet")));
      // Load data into the table InOutRecord. You can modify this code as needed.
      VidoParkingDataSetTableAdapters.InOutRecordTableAdapter vidoParkingDataSetInOutRecordTableAdapter = new VidoParkingDataSetTableAdapters.InOutRecordTableAdapter();
      vidoParkingDataSetInOutRecordTableAdapter.Fill(vidoParkingDataSet.InOutRecord);
      System.Windows.Data.CollectionViewSource inOutRecordViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inOutRecordViewSource")));
      inOutRecordViewSource.View.MoveCurrentToFirst();
    }
  }
}
