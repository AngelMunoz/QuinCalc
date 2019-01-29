using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace QuinCalc.Converters
{
  public class CurrencyConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      string currency = ((decimal)value).ToString("C", CultureInfo.CurrentCulture);
      return currency;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      var culture = new CultureInfo(language);
      var preval = ((string)value).ToString(CultureInfo.CurrentCulture);
      if (decimal.TryParse(preval, out decimal result)) { return result; }
      return value;
    }

  }
}
