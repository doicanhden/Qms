namespace Vido.Parking.Ms.Ui.Wpf
{
  using System;
  using System.Windows.Data;

  class IntToCardState : IValueConverter
  {
    public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var str = value.ToString();

      if (string.IsNullOrWhiteSpace(str))
        return (null);

      return (Enum.Parse(typeof(CardState), str));;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
      {
        return (null);
      }

      return (int)(value);
    }
  }
}
