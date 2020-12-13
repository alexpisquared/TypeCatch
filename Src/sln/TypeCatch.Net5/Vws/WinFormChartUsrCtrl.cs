using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.ObjectModel;
using TypingWpf.Mdl;
using AsLink;
using AAV.Sys.Ext;
using LiveCharts.Wpf.Charts.Base;

namespace TypingWpf.Vws
{
    public partial class WinFormChartUsrCtrl : UserControl
  {
    public WinFormChartUsrCtrl() { InitializeComponent(); }

    void f2()
    {
      var now = DateTime.Now;

      //chart1.Series.Clear();
      //var s = chart1.Series.Add("New Series");
      var s = chart1.Series[0];
      s.Points.Clear();
      for (int i = 0; i < 20; i++)
      {
        s.Points.AddXY(now.AddDays(i).ToOADate(), Math.Sin(i * Math.PI / 20));
      }
    }

    ObservableCollection<SessionResult> _Srl = new ObservableCollection<SessionResult>();
    public ObservableCollection<SessionResult> LoadDataToChart_Prop { get { return _Srl; } set { _Srl = value; DrawChart(value); } }

    public void DrawChart(IEnumerable<SessionResult> lst)
    {
      try
      {
        var s = chart1.Series[0];
        s.Points.Clear();

        foreach (SessionResult sr in lst) { s.Points.AddXY(sr.DoneAt.DateTime, sr.CpM); }

        if (lst.Count() > 1)
          chart1.ChartAreas.First().AxisY.Minimum = lst.Min(r => r.CpM);
      }
      catch (Exception ex) { ex.Log(); }
    }


    public Chart Chartuc { get { return chart1; } set { chart1 = value; } }

    void WinFormChartUsrCtrl_Load(object sender, EventArgs e) { /*f2(); */}

    internal void CurrentSR(SessionResult sr)
    {
      try
      {
        var s = chart1.Series[1];
        s.Points.Clear();

        s.Points.AddXY(sr.DoneAt.DateTime, sr.CpM);
      }
      catch (Exception ex) { ex.Log(); }
    }
  }
}
