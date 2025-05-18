using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace AsLink
{
  public class BoolToColorConverter : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return new SolidColorBrush(((bool)value) ? Colors.Green : Colors.DarkRed); }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return value; }
    public override object ProvideValue(IServiceProvider serviceProvider) { return this; }
    public BoolToColorConverter() { }
  }

  public class CpmToColorConverter : MarkupExtension, IMultiValueConverter //Jul 2017
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(values[0] is double && values[1] is int) || (100 + (int)values[1]) == 0) return new SolidColorBrush(Colors.Transparent);

      var recordCpm = 100 + (int)values[1];
      var curnetCpm = 100 + (double)values[0];
      var ds = (byte)(64.0 * (curnetCpm) / (recordCpm));

      return new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(curnetCpm > recordCpm ? 0 : (.7 * ds)), ds, 0));
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException("search for CpmToColorConverter");
    public override object ProvideValue(IServiceProvider serviceProvider) { return this; }
    public CpmToColorConverter() { }
  }
}
