using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;

namespace TypeCatch.Net5.Misc;

public static class CartesianChartExt
{
  public static void LoadDataToChart(this CartesianChart cc, IEnumerable<SessionResult> sr)
  {
    try
    {
      var dayConfig = Mappers.Xy<DateMdl>().X(datemdl => datemdl.DTime.ToOADate()).Y(datemdl => datemdl.Value); //you can also configure this type globally, so you don't need to configure every SeriesCollection instance using the type. more info at http://lvcharts.net/App/Index#/examples/v1/wpf/Types%20and%20Configuration

      cc.Series = new SeriesCollection(dayConfig) { new LineSeries { Values = new ChartValues<DateMdl>(), PointGeometry = DefaultGeometries.Circle, PointGeometrySize = 4, PointForeground = Brushes.Yellow, LineSmoothness = .01 } };

      foreach (var l2 in sr)
        _ = cc.Series[0].Values.Add(new DateMdl(l2.DoneAt, l2.CpM));
    }
    catch (Exception ex) { _ = ex.Log(); }
  }
}

public record DateMdl(DateTime DTime, double Value);