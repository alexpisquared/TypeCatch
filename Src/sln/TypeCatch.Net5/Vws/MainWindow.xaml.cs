using AAV.Sys.Helpers;
using AsLink;
using System.Diagnostics;
using System.Windows;

namespace TypingWpf
{
  public partial class MainWindow : WindowBase
  {
    public MainWindow() : base()
    {
      InitializeComponent();

      _ignoreEscape = true;

      VersioInfo.Text = $"{VerHelper.CurVerStr(".Net5")}\n{DbMdl.A0DbMdl.GetA0DbMdlAzureDb.ServerDatabase()}";

      Topmost = Debugger.IsAttached;
    }

    void onWindowMinimize(object s, RoutedEventArgs e) => WindowState = System.Windows.WindowState.Minimized;
    void onWindowRestoree(object s, RoutedEventArgs e) { wr.Visibility = Visibility.Collapsed; wm.Visibility = Visibility.Visible; WindowState = System.Windows.WindowState.Normal; }
    void onWindowMaximize(object s, RoutedEventArgs e) { wm.Visibility = Visibility.Collapsed; wr.Visibility = Visibility.Visible; WindowState = System.Windows.WindowState.Maximized; }

    private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {

    }
  }
}