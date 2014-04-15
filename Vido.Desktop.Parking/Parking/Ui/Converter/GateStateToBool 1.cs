namespace Vido.Parking.Ui.Converter
{
  using System.Windows.Data;

  public class GateStateToBool : IValueConverter
  {

    public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return ((Qms.GateState)value == Qms.GateState.Opened);
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ((bool)value)
      {
        return (Qms.GateState.Opened);
      }

      return (Qms.GateState.Closed);
    }
  }
}
