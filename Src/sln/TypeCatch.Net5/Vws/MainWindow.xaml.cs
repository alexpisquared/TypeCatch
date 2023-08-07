using AAV.Sys.Helpers;
using AsLink;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TypeCatch.Net5.DbMdl;

namespace TypingWpf
{
  public partial class MainWindow : WindowBase
  {
    public MainWindow() : base()
    {
      InitializeComponent();

      _ignoreEscape = true;

      VersioInfo.Text = $"{VerHelper.CurVerStr(".Net5")}\n{A0DbMdl.GetA0DbMdl.ServerDatabase()}";

      Topmost = Debugger.IsAttached;
    }

    void onWindowMinimize(object s, RoutedEventArgs e) => WindowState = System.Windows.WindowState.Minimized;
    void onWindowRestoree(object s, RoutedEventArgs e) { wr.Visibility = Visibility.Collapsed; wm.Visibility = Visibility.Visible; WindowState = System.Windows.WindowState.Normal; }
    void onWindowMaximize(object s, RoutedEventArgs e) { wm.Visibility = Visibility.Collapsed; wr.Visibility = Visibility.Visible; WindowState = System.Windows.WindowState.Maximized; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      const string accelkeys = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ`~!@#$%^&*()_+";
      int i = 0;
      var rr = LessonHelper.LoadDrillDataArray();
      foreach (var dr in rr)
        mniFileDrills.Items.Add(new MenuItem
        {
          Header = $"_{accelkeys[i++]} \t{dr.Header}",
          CommandParameter = dr.SqlExcerciseName,
          //Command = .. used through Style.
        });
    }
  }
}