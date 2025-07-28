using System.ComponentModel;
using System.Windows.Data;
using static AmbienceLib.SpeechSynth;

namespace TypingWpf.VMs;

public partial class MainVM //: BindableBaseViewModel
{
  internal void onJsonToDb_Suspended() => __speechSynth.SpeakFAF("Migrating JSON to local DB is suspended till further notice.");

  internal async Task LoadFromDbAsync()
  {
    LessonText = "\r\n\n\t  W A I T !    \r\n\n\t\t Loading \r\n\n\t\t\t from DB Async ... ";

    IsBusy = true;
    try
    {
      db.SessionResults.Load();

      SessionResultCvs = CollectionViewSource.GetDefaultView(db.SessionResults.Local.ToObservableCollection());
      SessionResultCvs.SortDescriptions.Add(new SortDescription(nameof(SessionResult.DoneAt), ListSortDirection.Descending));
      SessionResultCvs.Filter = obj => obj is not SessionResult r || r is null
        || string.IsNullOrEmpty(SearchText) || r.Note.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true;

      LessonText =
      InfoMsg = $"{((ListCollectionView)SessionResultCvs).Count} matches so far.";

      SelectUser = tlaFromCurEnvtUser();
      var appSetngCountBefore = db.AppStngs.Count();

      //var rmv = db.AppStngs.Find(5); if (rmv != null)  db.AppStngs.Remove(rmv);                    db.TrySave Report();

      var appStngUsr = await getCurUserSettings(db);

      var appSetngCount_After = db.AppStngs.Count();

      foreach (var aps in db.AppStngs) { Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"    {aps.Id,5} {aps.UserId,5} {aps.FullName,-20} {aps.Note}"); }

      SubLesnId /**/ = appStngUsr.SubLesnId;
      ProLTgl   /**/ = appStngUsr.ProLtgl;
      Audible   /**/ = appStngUsr.Audible;
      LesnTyp   /**/ = (LessonType)appStngUsr.LesnTyp;


      __speechSynth.SpeakAsyncCancelAll();
      __speechSynth.SpeakFAF("Ready, player one.", voice: CC.Xiaomo, style: CC.ZhcnXiaomoNeural.Styles[new Random(DateTime.Now.Microsecond).Next(CC.ZhcnXiaomoNeural.Styles.Length)], role: CC.Girl);
    }
    catch (SqlException ex)
    {
      InfoMsg = ex.Log();
      __speechSynth.SpeakAsyncCancelAll();
      await __speechSynth.SpeakAsync("Get the right 'AzureSqlCredentials.json' to the right place. Everything is your friend. BTW, resetting password on Azure portal is easier than other options.");
      await Task.Delay(2000); // SpeakAsync does not wait long enough to finish the sentence.
    }
    catch (Exception ex) { InfoMsg = ex.Log(); }
    finally { IsBusy = false; }
  }

  async Task<AppStng> getCurUserSettings(OneBaseContext db)
  {
    var appStngUsr = db.AppStngs.FirstOrDefault(r => r.UserId == SelectUser);
    if (appStngUsr == null)
    {
      var anybody = await getAnybody_CreateIfnobody(db);
      appStngUsr = db.AppStngs.Add(new AppStng { LesnTyp = anybody.LesnTyp, SubLesnId = anybody.SubLesnId, CreatedAt = DateTime.Now, UserId = SelectUser, Note = $"Auto pre-copied from {anybody.UserId}." }).Entity;

      _ = await db.SaveChangesAsync(); // .TrySaveReportAsync();
    }

    return appStngUsr;
  }

  static async Task<AppStng> getAnybody_CreateIfnobody(OneBaseContext db)
  {
    var anybody = db.AppStngs.FirstOrDefault();
    if (anybody == null)
    {
      anybody = db.AppStngs.Add(new AppStng { LesnTyp = (int)LessonType.PhrasesRandm, SubLesnId = "0", CreatedAt = DateTime.Now, Id = 1, UserId = "Dflt", Note = "Auto pre-loaded [Default]." }).Entity;
      _ = await db.SaveChangesAsync(); // .TrySaveReportAsync();
    }

    return anybody;
  }

  string tlaFromCurEnvtUser()
  {
    var curuser = Environment.UserName.ToLower();
    switch (curuser)
    {
      case "alex":
      case "alexp": return "Alx";
      case "mei":
      case "jingm": return "Mei";
      case "zoepi": return "Zoe";
      case "nadine": return "Ndn";
      default:
        if (curuser.Contains("ale")) return "Alx";
        if (curuser.Contains("zoe")) return "Zoe";
        if (curuser.Contains("nad")) return "Ndn";
        if (curuser.Contains("mei")) return "Mei";
        if (curuser.Contains("jing")) return "Mei";
        return "Alx";
    }
  }

  async Task updateSettings(OneBaseContext db)
  {
    var stg = await getCurUserSettings(db) ?? db.AppStngs.Add(new AppStng { LesnTyp = (int)LessonType.PhrasesRandm, SubLesnId = "0", CreatedAt = DateTime.Now, Id = 1, UserId = "Plr1", Note = "Auto pre-loaded." }).Entity;

    stg.SubLesnId = SubLesnId;
    stg.LesnTyp = (int)LesnTyp;
    stg.Audible = Audible;
    stg.ProLtgl = ProLTgl;
    stg.UserId = SelectUser;
    stg.ModifiedAt = DateTime.Now;
  }
  internal void DeleteSaveSsnRsltToDb(db.SessionResult sr, OneBaseContext db)
  {
    IsBusy = true;
    _ = DateTime.Now;
    try
    {

      {
        //if (Debugger.IsAttached) Debugger.Break(); // >>> DoneAt = sr.DoneAt.DateTime, //todo: ?? .AddMinutes(sr.DoneAt.OffsetMinutes)

        var srfromdb = db.SessionResults.Find(sr.Id);
        _ = db.SessionResults.Remove(srfromdb);

        var rv = db.SaveChanges(); //.TrySaveReportAsync(); //Trace.TraceInformation($"{rv}");

        InfoMsg = $" GLobal/Total runs {db.SessionResults.Count(),4} (SessionResults rows). \r\n {LesnTyp}-{SubLesnId}\r\n SelectUser: '{SelectUser}'";
      }
    }
    catch (Exception ex) { InfoMsg = ex.Log(); }
    finally { IsBusy = false; }
  }
}