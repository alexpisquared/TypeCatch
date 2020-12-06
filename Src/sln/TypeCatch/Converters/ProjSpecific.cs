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
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return new SolidColorBrush(((bool)value) ? Colors.LimeGreen : Colors.HotPink); }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
    public override object ProvideValue(IServiceProvider serviceProvider) { return this; }
    public BoolToColorConverter() { }
  }

  public class CpmToColorConverter : MarkupExtension, IMultiValueConverter //Jul 2017
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(values[0] is int && values[1] is int)) return new SolidColorBrush(Colors.White);

      var crntCpm = (int)values[0];
      var rcrdCpm = (int)values[1];
      if (crntCpm > rcrdCpm && crntCpm != 0)
      {
        var ds = (byte)((225.0 * rcrdCpm / crntCpm));
        return new SolidColorBrush(Color.FromRgb(ds, 255, ds)); // Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"++ {CrntCpm,5} / {_recordCpm} ==> {(10.0 * CrntCpm / _recordCpm),6:N1} ==> {ds,4} ++");
      }
      else if (rcrdCpm != 0)
      {
        var ds = (byte)(255.0 * (100 + crntCpm) / (100 + rcrdCpm));
        return new SolidColorBrush(Color.FromRgb(255, ds, ds));
      }

      return new SolidColorBrush(Color.FromRgb(255, 255, 190));
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException("search for CpmToColorConverter");
    public override object ProvideValue(IServiceProvider serviceProvider) { return this; }
    public CpmToColorConverter() { }
  }
}
