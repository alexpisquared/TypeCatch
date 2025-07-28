using System.Windows.Input;
using TypingWpf.Resources;
using TypingWpf.VMs;

namespace TypingWpf.Vws;

public partial class JsonToSqlMigrator : Window
{
  Bpr Bpr = new();
  public JsonToSqlMigrator()
  {
    InitializeComponent(); MouseLeftButtonDown += new MouseButtonEventHandler((s, e) => DragMove()); //tu:
    tbInfo.Text = tbEror.Text = "";
  }

  async void onLoaded(object sender, RoutedEventArgs e) => await migrate(A0DbMdl.GetA0DbMdl);
  async Task migrate(A0DbMdl db)
  {
    Bpr.Beep1of2();
    if (JsonFileSerializer.Load<ObsMainVM>(OneDrive.Folder($@"Public\AppData\TypeCatch\{typeof(MainVM).Name}.json")) is not ObsMainVM obsVm)
      _ = System.Windows.MessageBox.Show("No Go");
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
        //Bpr.Click();

        int brandNew = 0, noMatchesInDb = 0, alreadySame = 0, updated = 0;

        foreach (var srjs in obsVm.SnRts)
        {
          var srdb0 = db.SessionResults.FirstOrDefault(d =>
                  d.UserId == srjs.UserId &&
                  d.Duration.ToTimeSpan() == srjs.Duration /*drn(srjs.Duration)*/ &&
                  d.ExcerciseName == srjs.ExcerciseName &&
                  d.PokedIn == srjs.PokedIn &&
                  Math.Abs((d.DoneAt - srjs.DoneAt.DateTime.AddMinutes(srjs.DoneAt.OffsetMinutes)).TotalSeconds) < 5);
          if (srdb0 == null)
          {
            _ = db.SessionResults.Add(new SessionResult
            {
              DoneAt = srjs.DoneAt.DateTime.AddMinutes(srjs.DoneAt.OffsetMinutes),
              UserId = srjs.UserId,
              Duration = TimeOnly.FromTimeSpan(srjs.Duration), // Updated to use ToTimeSpan()
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
                Duration = TimeOnly.FromTimeSpan(srjs.Duration )/*drn(srjs.Duration)*/ ,
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

        tbInfo.Text += $" brandNew {brandNew}, notFoundInDb {noMatchesInDb}, alreadySame {alreadySame}, updated {updated}\n";
        Debug.WriteLine($"::>{tbInfo.Text}");

        var rv = await db.SaveChangesAsync(); // TrySaveReportAsync();

        tbInfo.Text += $" {rv}";
        Bpr.Beep2of2();
      }
      catch (Exception ex) { tbEror.Text = ex.Log(); }
      finally { new DbExplorer2().Show(); }
    }
  }

  TimeSpan drn(string duration) => TimeSpan.TryParse(duration, out var rv) ? rv : TimeSpan.FromMinutes(10);

  void onClose(object sender, RoutedEventArgs e) => Close();
  void onReview(object sender, RoutedEventArgs e) => new DbExplorer2().ShowDialog();
}
