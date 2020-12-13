using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AAV.Sys.Ext;
using AsLink;
using TypeCatch.Net5.Properties;
using TypingWpf.DbMdl;
using db = TypingWpf.DbMdl;

namespace TypingWpf.VMs
{
  public partial class MainVM //: BindableBaseViewModel
  {
    internal void onJsonToDb_Suspended() => synth.SpeakAsync("Migrating JSON to local DB is suspended till further notice.");

    async internal Task LoadFromDbAsync(A0DbMdl db)
    {
      LessonText = "\r\n\n\t  W A I T !    \r\n\n\t\t Loading \r\n\n\t\t\t from DB Async ... ";
      //synth.SpeakAsyncCancelAll(); synth.SpeakAsync("Loading from DB.");

      if (Settings.Default.ReadOnlyUsr=="haha")
      {
        synth.SpeakAsync("Change the credentials in CFG, search for haha.");
        return;
      }

      IsBusy = true;
      try
      {
        // using (var db = A0DbMdl.GetA0DbMdlAzureDb)
        {
          //Users.Clear();
          ////.await db.Users.LoadAsync(); 
          ////.foreach (var user in db.Users) Users.Add(user/*.UserId*/); //SnRts.Clear(); await db.SessionResults.LoadAsync(); foreach (var sr in db.SessionResults) { SnRts.Add(new js.SessionResult { DoneAt = sr.DoneAt, UserId = sr.UserId, Duration = sr.Duration, ExcerciseName = sr.ExcerciseName, PokedIn = sr.PokedIn }); }
          //foreach (var user in db.Users) Users.Add(user/*.UserId*/);

          SelectUser = tlaFromCurEnvtUser();
          var appSetngCountBefore = db.AppStngs.Count();

          //var rmv = db.AppStngs.Find(5); if (rmv != null)  db.AppStngs.Remove(rmv);                    db.TrySave Report();

          var appStngUsr = await getCurUserSettings(db);

          var appSetngCount_After = db.AppStngs.Count();

          foreach (var aps in db.AppStngs) { Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"    {aps.Id,5} {aps.UserId,5} {aps.FullName,-20} {aps.Note}"); }

          SubLesnId /**/ = appStngUsr.SubLesnId;
          ProLTgl   /**/ = appStngUsr.ProLTgl;
          Audible   /**/ = appStngUsr.Audible;
          LesnTyp   /**/ = appStngUsr.LesnTyp;

          InfoMsg = $" Total {db.SessionResults.Count(),4} runs (SesRslt rows). \r\n {LesnTyp} - {SubLesnId}\r\n User: '{SelectUser}' (Stngs: {appStngUsr.UserId},  bfr-aft: {appSetngCountBefore}-{appSetngCount_After}) ";
        }
      }
      catch (Exception ex) { InfoMsg = ex.Log(); }
      finally { IsBusy = false; synth.SpeakAsyncCancelAll(); synth.SpeakAsync("Ready, player one."); }
    }

    async Task<AppStng> getCurUserSettings(A0DbMdl db)
    {
      var appStngUsr = db.AppStngs.FirstOrDefault(r => r.UserId == SelectUser);
      if (appStngUsr == null)
      {
        var anybody = await getAnybody_CreateIfnobody(db);
        appStngUsr = db.AppStngs.Add(new AppStng
        {
          LesnTyp = anybody.LesnTyp,
          SubLesnId = anybody.SubLesnId,
          CreatedAt = DateTime.Now,
          UserId = SelectUser,
          Note = $"Auto pre-copied from {anybody.UserId}."
        });

        await db.TrySaveReportAsync();
      }

      return appStngUsr;
    }

    static async Task<AppStng> getAnybody_CreateIfnobody(A0DbMdl db)
    {
      var anybody = db.AppStngs.FirstOrDefault();
      if (anybody == null)
      {
        anybody = db.AppStngs.Add(new AppStng { LesnTyp = LessonType.PhrasesRandm, SubLesnId = "0", CreatedAt = DateTime.Now, Id = 1, UserId = "Dflt", Note = "Auto pre-loaded [Default]." });
        await db.TrySaveReportAsync();
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

    async Task updateSettings(A0DbMdl db)
    {
      var stg = await getCurUserSettings(db) ?? db.AppStngs.Add(new AppStng { LesnTyp = LessonType.PhrasesRandm, SubLesnId = "0", CreatedAt = DateTime.Now, Id = 1, UserId = "Plr1", Note = "Auto pre-loaded." });

      stg.SubLesnId = SubLesnId;
      stg.LesnTyp = LesnTyp;
      stg.Audible = Audible;
      stg.ProLTgl = ProLTgl;
      stg.UserId = SelectUser;
      stg.ModifiedAt = DateTime.Now;
    }
    internal void DeleteSaveSsnRsltToDb(db.SessionResult sr, A0DbMdl db)
    {
      IsBusy = true;
      var now = DateTime.Now;
      try
      {
        // using (var db = A0DbMdl.GetA0DbMdlAzureDb)
        {
          //if (Debugger.IsAttached) Debugger.Break(); // >>> DoneAt = sr.DoneAt.DateTime, //todo: ?? .AddMinutes(sr.DoneAt.OffsetMinutes)

          var srfromdb = db.SessionResults.Find(sr.Id);
          db.SessionResults.Remove(srfromdb);

          var rv = db.TrySaveReportAsync(); //Trace.TraceInformation($"{rv}");

          InfoMsg = $" Total {db.SessionResults.Count(),4} SesRslt rows. \r\n {LesnTyp} - {SubLesnId}\r\n SelectUser: '{SelectUser}'";
        }
      }
      catch (Exception ex) { InfoMsg = ex.Log(); }
      finally { IsBusy = false; }
    }
  }
}