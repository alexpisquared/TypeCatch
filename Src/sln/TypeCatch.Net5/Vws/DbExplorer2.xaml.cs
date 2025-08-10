//using AAV.Sys.Helpers;
using AsLink;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TypeCatch.Net5.DbMdl;

namespace TypingWpf.Vws
{
  public partial class DbExplorer2 // : WindowBase
  {
    Bpr _dbeBpr = new();
    readonly A0DbMdl _db = A0DbMdl.GetA0DbMdl;
    public DbExplorer2() => InitializeComponent();

    async void Window_Loaded(object sender, RoutedEventArgs e)
    {
      tbInfo.Text = tbEror.Text = "";

      await _db.Users.LoadAsync();
      await _db.AppStngs.LoadAsync();
      await _db.SessionResults.LoadAsync();

      ((CollectionViewSource)(FindResource("userViewSource"))).Source = _db.Users.Local;
      ((CollectionViewSource)(FindResource("appStngViewSource"))).Source = _db.AppStngs.Local;
      ((CollectionViewSource)(FindResource("sessionResultViewSource"))).Source = _db.SessionResults.Local.OrderByDescending(r => r.DoneAt).ToList(); //?? .OrderByDescending(r => r.DoneAt); <= edititem is not allowed for this view 

      //ExzeFilter.Items.Clear(); foreach (var item in _db.SessionResults.Local.Select(r => r.ExcerciseName).Distinct().OrderBy(r => r)) ExzeFilter.Items.Add(item);

      ExzeFilter.ItemsSource = _db.SessionResults.Local
          .GroupBy(g => new { g.ExcerciseName, g.UserId }).ToList()
          .Select(r => new { Lesson = r.Key.ExcerciseName, UserId = r.Key.UserId, Times = r.Count(), Record = r.Max(y => y.CpM) })
          .OrderBy(r => r.Lesson)
          .ThenBy(r => r.UserId)
          .ThenByDescending(r => r.Record)
          .ThenByDescending(r => r.Times)
          .ToList();

      tbInfo.Text = $"{_db.SessionResults.Local.Count} total runs";
    }

    void onClose(object sender, RoutedEventArgs e) => Close();
    void onUpdateSsnRsltNotes(object sender, RoutedEventArgs e) => new JsonToSqlMigrator().Show();
    async void onDbSave(object sender, RoutedEventArgs e) { _dbeBpr.Beep1of2(); tbInfo.Text = (await _db.TrySaveReportAsync()).report; _dbeBpr.Beep2of2(); }


    void ExzeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count < 1) return;
      var filt = ((dynamic)e.AddedItems[0]);

      ((CollectionViewSource)(FindResource("sessionResultViewSource"))).Source = _db.SessionResults.Local
          .Where(r => r.ExcerciseName.Equals(filt.Lesson) && r.UserId.Equals(filt.UserId))
          .OrderByDescending(r => r.DoneAt);
    }
  }
}
