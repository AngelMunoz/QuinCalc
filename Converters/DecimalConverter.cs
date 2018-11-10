using System;
using Windows.UI.Xaml.Data;

namespace QuinCalc.Converters
{
  public class DecimalConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      decimal.TryParse(value as string, out decimal result);
      return result;
    }

  }
}
