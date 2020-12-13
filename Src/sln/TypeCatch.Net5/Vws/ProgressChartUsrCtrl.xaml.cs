using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TypingWpf.Vws
{
  public partial class ProgressChartUsrCtrl : UserControl
  {
    public ProgressChartUsrCtrl()
    {
      InitializeComponent();
    }

    void f1()
    {
      var now = DateTime.Now;

      //chartuc.Chartuc.Series.Clear();
      //var s = chartuc.Chartuc.Series.Add("New Series");
      //s.Points.Clear();
      //for (int i = 0; i < 20; i++)
      //{
      //  s.Points.AddXY(now.AddDays(i).ToOADate(), Math.Sin(i * Math.PI / 20));
      //}
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      f1();
    }
  }
}
