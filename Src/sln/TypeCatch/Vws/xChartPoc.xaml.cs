using System.Windows;
using System.Windows.Input;

namespace TypingWpf
{
    public partial class xChartPoc : Window
    {
        public xChartPoc() { InitializeComponent(); MouseLeftButtonDown += new MouseButtonEventHandler((s, e) => DragMove()); }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //cChart1.DrawChart();
        }
    }
}
