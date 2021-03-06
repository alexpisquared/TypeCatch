﻿using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using MVVM.Common;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TypingWpf.DbMdl;
using xMvvmMin;
using db = TypingWpf.DbMdl;

namespace TypingWpf.VMs
{
  public partial class MainVM : BindableBaseViewModel
  {
    const int _t333 = 333;
    DateTime _nextMeasureTime;
    TimeSpan _measurePeriod = TimeSpan.FromSeconds(2.5);
    public static readonly TimeSpan _shortView = TimeSpan.FromDays(28);
    readonly Stopwatch _swMain = new Stopwatch();
    ResourcePlayer _soundPlayer = new ResourcePlayer();
    DispatcherTimer _dt = null;
    readonly SpeechSynthesizer synth = new SpeechSynthesizer();
    //Brush _errorBrush = new SolidColorBrush(Colors.Red);
    //Brush _greenBrush = new SolidColorBrush(Colors.Green);

    protected override void AutoExec() { }
    protected override async Task AutoExecAsync()
    {
      try
      {
        base.AutoExec();
        VersioInfo = VerHelper.CurVerStr(".NET 4.8");

        using (var db = A0DbMdl.GetA0DbMdlAzureDb)
        {
          await LoadFromDbAsync(db);

          SelectSnRt = null;

          sessionLoad_Start_lazy();
          IsInSsn = true;
          IsAdmin = VerHelper.IsVIP;

          var gnd = (VoiceGender)((AppRunCount++) % 3 + 1);
          synth.Rate = 3;
          //Feb 2020: seems to hang on this one: synth.SelectVoiceByHints(gnd, VoiceAge.Senior);

          _dt = new DispatcherTimer(TimeSpan.FromMilliseconds(_t333), DispatcherPriority.Background, new EventHandler(async (s, e) => await tick333ms()), Dispatcher.CurrentDispatcher); //tu: one-line timer
          _dt.Start();
          MainVis = Visibility.Visible;
          Opcty = 1;
          //SpeedClr = new SolidColorBrush(Colors.White);
          await updateDoneTodo(SelectUser, synth, db);
        }

        CurInfo = $"{(LesnTyp)} - {SubLesnId:N0}  ";// ({DashName})";
      }
      catch (Exception ex) { ex.Log(); synth.SpeakAsyncCancelAll(); synth.SpeakAsync($"Something is not right: {ex.Message}. Talk to you later"); }

      Trace.TraceInformation($"{DateTime.Now:HH:mm:ss.fff} AutoExec: \t.");
    }
    protected override async Task ClosingVM()
    {
      Bpr.BeepClk();

      if (_cancelClosing = IsInSsn) // if in session: finish it and cancel closing and show results.
      {
        await finishTheSession();
        return;
      }

      //redundant: SaveToDb_SettingsMostly();  <== redundant Jun 2019

      Opcty = 0;
      IsInSsn = true; // just to show responciveness
      MainVis = Visibility.Hidden;
      refreshUiSynch();
      await refreshUi();

      await Task.Run(() => SoundPlayer.PlayByeByeSound());
      await base.ClosingVM();
    }

    //async void loadListsFromDB_(string dashName, string selectUser, A0DbMdl db)    {      await loadListsFromDB(dashName, selectUser, db);    }

    void loadListsFromDB(string dashName, string selectUser, A0DbMdl db)
    {
      if (string.IsNullOrEmpty(selectUser) || string.IsNullOrEmpty(dashName) /*|| SelectUser.Equals(selectUser)*/)
        return;

      if (!SelectUser.Equals(selectUser))
        SelectUser = selectUser;

      Properties.Settings.Default.LastUser = SelectUser;
      Properties.Settings.Default.Save();

      try
      {
        var dbsrlst = db.SessionResults.Where(r => r.UserId == SelectUser && r.ExcerciseName == dashName && r.Duration > TimeSpan.Zero).OrderByDescending(r => r.DoneAt).ToList();

        RcrdCpm = dbsrlst?.Count() > 0 ? (int)(dbsrlst?.Max(r => r.PokedIn / r.Duration.TotalMinutes) ?? 110) : 99;

        MaxCpm = 0 + 2 * RcrdCpm;

        Debug.WriteLine($"~~~ C: {db.SessionResults.Count()} - {CurUserCurExcrsRsltLst.Count}");

        CurUserCurExcrsRsltLst.ClearAddRangeAuto(dbsrlst);

        Debug.WriteLine($"~~~ D: {db.SessionResults.Count()} - {CurUserCurExcrsRsltLst.Count}");
      }
      catch (Exception ex) { ex.Log(); synth.SpeakAsyncCancelAll(); synth.SpeakAsync($"Something is not right: {ex.Message}. Talk to you later"); }
    }
    string getTheLatestLessonTypeTheUserWorksOn(A0DbMdl db)
    {
      var dashName = db.SessionResults.Where(r => r.UserId == SelectUser).OrderByDescending(x => x.DoneAt).FirstOrDefault()?.ExcerciseName;

      if (string.IsNullOrEmpty(dashName))
        dashName = DashName;

      prepLessonType(dashName/*.Replace("-", "")*/, false);

      return dashName;
    }

    void onShowChart(object mode)
    {
      string f;
      switch ((string)mode)
      {
        case "Full": f = "MMM-dd";  /**/ _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.OrderByDescending(r => r.DoneAt)); break;
        case "Mont": f = "MMM-dd";  /**/ _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.Where(r => r.DoneAt > DateTime.Now.AddMonths(-1)).OrderByDescending(r => r.DoneAt)); break;
        case "Week": f = "MMM-dd";  /**/ _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.Where(r => r.DoneAt > DateTime.Now.AddDays(-7)).OrderByDescending(r => r.DoneAt)); break;
        case "1Day": f = "H:mm";    /**/ _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.Where(r => r.DoneAt > DateTime.Now.AddDays(-1)).OrderByDescending(r => r.DoneAt)); break;
        case "PreX": f = "H:mm";    /**/ _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.OrderByDescending(r => r.DoneAt).Take(10)); break;
        case "Pre5": f = "h:mm:ss"; /**/ _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.OrderByDescending(r => r.DoneAt).Take(05)); break;
        case null:
        default: f = "MMM-dd"; break;
      }

      _chartUC.AxisX.First().LabelFormatter = value => DateTime.FromOADate(value).ToString(f);
    }

    void onDeleteSR(object x)
    {
      if (SelectSnRt == null) return;

      synth.SpeakAsyncCancelAll(); synth.SpeakAsync($"Are you sure?");
      if (MessageBox.Show($"{SelectSnRt.DoneAt:MMM-dd HH:mm} \r\n\n{SelectSnRt.CpM} cpm\r\n\n{(SelectSnRt.IsRecord == true ? "It's a Record!!" : "")}", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
      {
        DeleteSaveSsnRsltToDb(SelectSnRt, A0DbMdl.GetA0DbMdlAzureDb);
      }
    }
    void promptSample()
    {
      var ps = new PromptStyle
      {
        Emphasis = PromptEmphasis.Strong,
        Volume = PromptVolume.ExtraLoud
      };

      var pb = new PromptBuilder();
      pb.StartStyle(ps);
      pb.StartParagraph();

      pb.StartVoice(VoiceGender.Female, VoiceAge.Child);
      pb.StartSentence();
      pb.AppendText($"Female Child", PromptRate.Medium);
      pb.AppendBreak(TimeSpan.FromSeconds(.3));
      pb.EndSentence();
      pb.EndVoice();

      pb.StartVoice(VoiceGender.Female, VoiceAge.Senior);
      pb.StartSentence();
      pb.AppendText($"Female Senior", PromptRate.Medium);
      pb.AppendBreak(TimeSpan.FromSeconds(.3));
      pb.EndSentence();
      pb.EndVoice();

      pb.StartVoice(VoiceGender.Male, VoiceAge.Senior);
      pb.StartSentence();
      pb.AppendText($"Male Senior", PromptRate.Medium);
      pb.AppendBreak(TimeSpan.FromSeconds(.3));
      pb.EndSentence();
      pb.EndVoice();

      pb.StartVoice(VoiceGender.Male, VoiceAge.Child);
      pb.StartSentence();
      pb.AppendText($"Male Child", PromptRate.Medium);
      pb.AppendBreak(TimeSpan.FromSeconds(.3));
      pb.EndSentence();
      pb.EndVoice();

      pb.EndParagraph();
      pb.EndStyle();
      synth.SpeakAsyncCancelAll(); synth.SpeakAsync(pb);
    }
    async void prepLessonType(string x, bool doF1 = true)
    {
      SubLesnId = x.Split('-').Last();

      switch (x[0])
      {
        case 'B': LesnTyp = LessonType.BasicLessons; break;
        case 'C': LesnTyp = LessonType.Combinations; break;
        case 'D': LesnTyp = LessonType.DigitSymbols; break;
        case 'F': LesnTyp = LessonType.EditableFile; break;
        case 'S': LesnTyp = LessonType.SpecialDrill; break;
        case 'P': LesnTyp = LessonType.PhrasesRandm; break;
        case 'X': LesnTyp = LessonType.Experimental; break;
        default: break;
      }

      LessonText = LessonHelper.GetLesson(LesnTyp, SubLesnId);
      LessonLen = LesnTyp == LessonType.PhrasesRandm ? LessonText.Length - LessonHelper.PaddingLen : LessonText.Length;

      if (doF1)
      {
        using (var db = A0DbMdl.GetA0DbMdlAzureDb)
        {
          loadListsFromDB(DashName, SelectUser, db);
          await updateDoneTodo(SelectUser, synth, db);
        }
      }

      CurInfo = $"{(LesnTyp)} - {SubLesnId:N0}  ";// ({DashName})";
    }
    async Task updateDoneTodo(string selectUser, SpeechSynthesizer synth, A0DbMdl db)
    {
      int doneToday = -1, sinceRcrd = -1, todoToday = -1;
      try
      {
        doneToday = db.SessionResults.Count(r => r.UserId.Equals(selectUser, StringComparison.OrdinalIgnoreCase) && r.DoneAt > DateTime.Today);

        //if (!db.SessionResults.Any(r => r.UserId.Equals(selectUser, StringComparison.OrdinalIgnoreCase)))
        //{
        //  sinceRcrd = 0;
        //  todoToday = _planPerDay - doneToday;
        //}
        //else
        {
          var latestGlobalRecordDate = await getLatestGlobalRecordDate(selectUser, db);

          sinceRcrd = db.SessionResults.Count(r => r.UserId == selectUser && r.DoneAt > latestGlobalRecordDate);
          todoToday =
            ((DateTime.Today < latestGlobalRecordDate)) ? 0 : // if done a record today ==> 0 left
            ((DateTime.Today - latestGlobalRecordDate).Days + 1) * _planPerDay - sinceRcrd;
        }

        if (todoToday < 0)
          todoToday = _planPerDay - doneToday;
      }
      catch (Exception ex) { ex.Log(); synth.SpeakAsyncCancelAll(); synth.SpeakAsync($"Something is not right: {ex.Message}. Talk to you later"); }

      DoneToday = doneToday;
      ExrzeRuns = sinceRcrd;
      TodoToday = todoToday;
    }

    static async Task<DateTime> getLatestGlobalRecordDate(string selectUser, A0DbMdl dbdbld) // not just the highest score!
    {
      var sw = Stopwatch.StartNew();
      var latestGlobalRecord = DateTime.Now.AddYears(-100);

      await dbdbld.SessionResults.Where(r => r.UserId == selectUser).LoadAsync();

      try
      {
        var allExrczNames = dbdbld.SessionResults.Local.GroupBy(r => r.ExcerciseName);
        foreach (var exrsName in allExrczNames)
        {
          if (dbdbld.SessionResults.Local.Any(r => r.UserId == selectUser && r.ExcerciseName == exrsName.Key))
          {
            var record4userAndExrz = dbdbld.SessionResults.Local.Where(r => r.UserId == selectUser && r.ExcerciseName == exrsName.Key).ToList().Max(r => r.CpM);
            var lastExrzRecordDate = dbdbld.SessionResults.Local.ToList().Where(r => r.UserId == selectUser && r.ExcerciseName == exrsName.Key && r.CpM == record4userAndExrz).Max(r => r.DoneAt);
            if (latestGlobalRecord < lastExrzRecordDate)
            {
              latestGlobalRecord = lastExrzRecordDate;
              Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $" {exrsName.Key,-8}{dbdbld.SessionResults.Local.Count(r => r.UserId == selectUser && r.ExcerciseName == exrsName.Key),5} times    max cpm {record4userAndExrz,3}   {lastExrzRecordDate}  ->  {latestGlobalRecord}");
            }
          }
        }
      }
      catch (Exception ex) { ex.Log(); }

      Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"  ... => latestGlobalRecord: {latestGlobalRecord}");

      Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceWarning, $"{DateTime.Now:yy.MM.dd-HH:mm:ss.f} +{(DateTime.Now - App.Started):mm\\:ss\\.ff}    *** getLatestGlobalRecordDate() done in {sw.Elapsed.TotalSeconds:N2} sec");

      return latestGlobalRecord;
    }
  }
}
