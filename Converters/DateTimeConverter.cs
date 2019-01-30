using System;
using System.Diagnostics;
using System.Globalization;
using QuinCalc.Helpers;
using Windows.UI.Xaml.Data;

namespace QuinCalc.Converters
{
  public class DateTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      try
      {
        var dto = (DateTimeOffset)value;
        return dto.DateTime.ToLongDateString();
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message, "Converters:DatetimeConverter:Convert");
        return DateTimeOffset.MinValue;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
