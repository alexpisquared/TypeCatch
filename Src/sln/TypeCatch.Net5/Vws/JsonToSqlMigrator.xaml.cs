using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using AsLink;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
using System.Windows.Shapes;
using TypeCatch.Net5.DbMdl;
using TypingWpf.DbMdl;
using TypingWpf.Resources;
using TypingWpf.VMs;

namespace TypingWpf.Vws
{
  public partial class JsonToSqlMigrator : Window
  {
    public JsonToSqlMigrator()
    {
      InitializeComponent(); MouseLeftButtonDown += new MouseButtonEventHandler((s, e) => DragMove()); //tu:
      tbInfo.Text = tbEror.Text = "";
    }

    async void onLoaded(object sender, RoutedEventArgs e) { await migrate(A0DbMdl.GetA0DbMdlAzureDb); }
    async Task migrate(A0DbMdl db)
    {
      Bpr.Beep1of2();
      var obsVm = JsonFileSerializer.Load<ObsMainVM>(MainVM.MainVmJsonFile) as ObsMainVM;
      if (obsVm == null)
        MessageBox.Show("No Go");
      else
      {
        var srFromJsonWithNote = obsVm.SnRts.Where(r => !string.IsNullOrEmpty(r.Notes));

        try
        {

          if (db.SessionResults.Any(r => r.Note != null && (r.Note == "zz" || r.Note.StartsWith("from fs at"))))
            db.SessionResults.Where(r => r.Note != null && (r.Note == "zz" || r.Note.StartsWith("from fs at"))).ToList().ForEach(r => r.Note = "");

          tbInfo.Text = $"Json: \t{obsVm.SnRts.Length} total runs incl-g {srFromJsonWithNote.Count()} with note;\r\nDB: \t{db.SessionResults.Count()} \r\n";
          Debug.WriteLine($"::>{tbInfo.Text}");

          await Task.Yield();
          Bpr.BeepClk();

          int brandNew = 0, noMatchesInDb = 0, alreadySame = 0, updated = 0;

          foreach (var srjs in obsVm.SnRts)
          {
            var srdb0 = db.SessionResults.FirstOrDefault(d =>
                    d.UserId == srjs.UserId &&
                    d.Duration == srjs.Duration /*drn(srjs.Duration)*/ &&
                    d.ExcerciseName == srjs.ExcerciseName &&
                    d.PokedIn == srjs.PokedIn &&
                    Math.Abs((d.DoneAt - srjs.DoneAt.DateTime.AddMinutes(srjs.DoneAt.OffsetMinutes)).TotalSeconds) < 5);
            if (srdb0 == null)
            {
              db.SessionResults.Add(new SessionResult
              {
                DoneAt = srjs.DoneAt.DateTime.AddMinutes(srjs.DoneAt.OffsetMinutes),
                UserId = srjs.UserId,
                Duration = srjs.Duration /*drn(srjs.Duration)*/ ,
                ExcerciseName = srjs.ExcerciseName,
                PokedIn = srjs.PokedIn,
                Note = srjs.Notes
              });
              brandNew++;
            }
          }

          try
          {
            foreach (var srjs in srFromJsonWithNote)
            {
              var srdb2 = db.SessionResults.FirstOrDefault(d => Math.Abs((d.DoneAt - srjs.DoneAt.DateTime.AddMinutes(srjs.DoneAt.OffsetMinutes)).TotalSeconds) < 5);
              if (srdb2 == null)
              {
                db.SessionResults.Add(new SessionResult
                {
                  DoneAt = srjs.DoneAt.DateTime.AddMinutes(srjs.DoneAt.OffsetMinutes),
                  UserId = srjs.UserId,
                  Duration = srjs.Duration /*drn(srjs.Duration)*/ ,
                  ExcerciseName = srjs.ExcerciseName,
                  PokedIn = srjs.PokedIn,
                  Note = srjs.Notes
                });
                noMatchesInDb++;
              }
              else
              {
                if (srjs.Notes.Equals(srdb2.Note))
                  alreadySame++;
                else
                {
                  updated++;
                  srdb2.Note = srjs.Notes;
                }
              }
            }
          }
          catch (Exception ex) { Debug.WriteLine(ex); }


          tbInfo.Text += $" brandNew {brandNew}, notFoundInDb {noMatchesInDb }, alreadySame {alreadySame }, updated {updated}\n";
          Debug.WriteLine($"::>{tbInfo.Text}");

          var rv = await db.TrySaveReportAsync();

          tbInfo.Text += $" {rv}";
          Bpr.Beep2of2();
        }
        catch (Exception ex) { tbEror.Text = ex.Log(); }
        finally { new DbExplorer2().Show(); }
      }
    }

    TimeSpan drn(string duration)
    {
      if (TimeSpan.TryParse(duration, out TimeSpan rv)) return rv;
      return TimeSpan.FromMinutes(10);
    }

    void onClose(object sender, RoutedEventArgs e) => Close();
    void onReview(object sender, RoutedEventArgs e) => new DbExplorer2().ShowDialog();
  }
}
