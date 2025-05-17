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
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return new SolidColorBrush(((bool)value) ? Colors.DarkBlue : Colors.Red); }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return value; }
    public override object ProvideValue(IServiceProvider serviceProvider) { return this; }
    public BoolToColorConverter() { }
  }

  public class CpmToColorConverter : MarkupExtension, IMultiValueConverter //Jul 2017
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(values[0] is double && values[1] is int)) return new SolidColorBrush(Colors.Yellow);

      var crntCpm = (double)values[0];
      var rcrdCpm = (int)values[1];
      if (crntCpm > rcrdCpm && crntCpm != 0)
      {
        var ds = (byte)(64.0 * (100 + crntCpm) / (100 + rcrdCpm));
        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, ds, 0));
      }
      else if (rcrdCpm != 0)
      {
        var ds = (byte)(64.0 * (100 + crntCpm) / (100 + rcrdCpm));
        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(ds, 0, 0));
      }

      return new SolidColorBrush(System.Windows.Media.Color.FromRgb(32, 32, 32));
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException("search for CpmToColorConverter");
    public override object ProvideValue(IServiceProvider serviceProvider) { return this; }
    public CpmToColorConverter() { }
  }
}
