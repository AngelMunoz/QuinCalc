using System;
using Windows.UI.Xaml.Data;

namespace QuinCalc.Converters
{
  public class DateTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      DateTime.TryParse((value ?? "").ToString(), out DateTime date);
      return date.ToShortDateString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      DateTime.TryParse((value ?? "").ToString(), out DateTime date);
      return date;
    }
  }
}
