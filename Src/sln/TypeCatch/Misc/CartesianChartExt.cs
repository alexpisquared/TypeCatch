using AAV.Sys.Ext;
using AsLink;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using db = TypingWpf.DbMdl;

namespace TypingWpf
{
    public static class CartesianChartExt
    {
        public static void LoadDataToChart(this CartesianChart cc, IEnumerable<db.SessionResult> sr)
        {
            try
            {
                var dayConfig = Mappers.Xy<DateMdl>().X(datemdl => datemdl.DTime.ToOADate()).Y(datemdl => datemdl.Value); //you can also configure this type globally, so you don't need to configure every SeriesCollection instance using the type. more info at http://lvcharts.net/App/Index#/examples/v1/wpf/Types%20and%20Configuration

                cc.Series = new SeriesCollection(dayConfig) {
                    new LineSeries { Values = new ChartValues<DateMdl>(),
                        PointGeometry = DefaultGeometries.Cross,
                        PointGeometrySize = 6,
                        PointForeground = Brushes.Yellow,
                        LineSmoothness = .01
                    }
                };

                foreach (var l2 in sr)
                    cc.Series[0].Values.Add(new DateMdl { DTime = l2.DoneAt, Value = l2.CpM });

            }
            catch (Exception ex) { ex.Log(); }
        }
    }

    public class DateMdl
    {
        public DateTime DTime { get; set; }
        public double Value { get; set; }
    }
}
