using Microsoft.EntityFrameworkCore;
using OneBase.Db.PowerTools.Models;
using TypeCatch.Net5.AsLink;
using db_ = TypingWpf.DbMdl;
namespace TypingWpf.VMs;
public partial class MainVM : BindableBaseViewModel
{
  const int _t333ms = 333;
  DateTime _nextMeasureTime;
  TimeSpan _measurePeriod = TimeSpan.FromSeconds(2.5);
  public static readonly TimeSpan _shortView = TimeSpan.FromDays(28);
  readonly Stopwatch _swMain = new Stopwatch();
  DispatcherTimer _dt = null;
  readonly SpeechSynth __speechSynth;
  OneBaseContext _dbx;

  public MainVM()
  {
    var key = new ConfigurationBuilder().AddUserSecrets<App>().Build()["AppSecrets:MagicSpeech"] ?? "no key"; //tu: adhoc usersecrets

    __speechSynth = new SpeechSynth(key, true, lgr: null);

    _dbx = CreateOneBaseContext();

    if (Debugger.IsAttached) return;
  }

  private OneBaseContext CreateOneBaseContext()
  {
    var optionsBuilder = new DbContextOptionsBuilder<OneBaseContext>();
    optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;initial catalog=OneBase;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework;Encrypt=False;");
    return new OneBaseContext(optionsBuilder.Options);
  }

  protected override void AutoExec() { }
  protected override async Task AutoExecAsync()
  {
    try
    {
      base.AutoExec();
      VersioInfo = "123233412312"; // VerHelper.CurVerStr(".Net5");


      {
        await LoadFromDbAsync();

        SelectSnRt = null;

        //sessionLoad_Start_lazy();
        IsInSsn = true;
        IsAdmin = true; // VerHelper.IsVIP;

        //var gnd = (VoiceGender)((AppRunCount++) % 3 + 1);
        //__speechSynth.Rate = 3;
        //Feb 2020: seems to hang on this one: __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Senior);

        _dt = new DispatcherTimer(TimeSpan.FromMilliseconds(_t333ms), DispatcherPriority.Background, new EventHandler(async (s, e) => await tick333ms()), Dispatcher.CurrentDispatcher); //tu: one-line timer
        _dt.Start();
        MainVis = Visibility.Visible;
        //SpeedClr = new SolidColorBrush(Colors.White);
        await updateDoneTodo(SelectUser, __speechSynth, _dbx);
      }

      CurInfo = $"{(LesnTyp)} - {SubLesnId:N0}  ";// ({DashName})";
    }
    catch (Exception ex) { ex.Log(); __speechSynth.SpeakAsyncCancelAll(); await __speechSynth.SpeakAsync($"Something is not right: {ex.Message}. Talk to you later"); }

    Trace.TraceInformation($"{DateTime.Now:HH:mm:ss.fff} AutoExec: \t.");
  }
  protected override async Task ClosingVM()
  {
    _cancelClosing = IsInSsn;
    if (IsInSsn) // if in session: finish it and cancel closing and show results.
    {
      LessonText = PupilInput = string.Empty;
      await finishTheSession();
      return;
    }

    _cancelClosing = true; //////////////////////////////////////////////////////////////// May 2025
    return;                //////////////////////////////////////////////////////////////// May 2025

    MainVis = Visibility.Hidden;
    //refreshUiSynch();
    //await refreshUi();

    await Task.Run(() => SoundPlayer.PlayByeByeSound());
    await base.ClosingVM();
  }

  void loadListsFromDB(string dashName, string selectUser)
  {
    if (string.IsNullOrEmpty(selectUser) || string.IsNullOrEmpty(dashName) /*|| SelectUser.Equals(selectUser)*/)
      return;

    if (!SelectUser.Equals(selectUser))
      SelectUser = selectUser;

    TypeCatch.Net5.Properties.Settings.Default.LastUser = SelectUser;
    TypeCatch.Net5.Properties.Settings.Default.Save();

    try
    {
      var dbsrlst = _dbx.SessionResults.Local.Where(r => r.UserId == SelectUser && r.ExcerciseName == dashName
      //&& r.Duration.ToTimeSpan() > TimeSpan.Zero
      ).OrderByDescending(r => r.DoneAt).ToList();

      RcrdCpm = dbsrlst?.Count() > 0 ? (int)(dbsrlst?.Max(r => r.PokedIn / r.Duration.ToTimeSpan().TotalMinutes) ?? 110) : 99;

      MaxCpm = RcrdCpm;

      Debug.WriteLine($"~~~ C: {_dbx.SessionResults.Local.Count()} - {SessionResultObs.Count}");

      SessionResultObs.ClearAddRangeAuto(dbsrlst);

      Debug.WriteLine($"~~~ D: {_dbx.SessionResults.Local.Count()} - {SessionResultObs.Count}");
    }
    catch (Exception ex) { ex.Log(); __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF($"Something is not right: {ex.Message}. Talk to you later"); }
  }
  string getTheLatestLessonTypeTheUserWorksOn(OneBaseContext db)
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
      case "Full": f = "y-M-d";    /**/ _chartUC.LoadDataToChart(SessionResultObs.OrderByDescending(r => r.DoneAt)); break;
      case "Year": f = "MMM d";    /**/ _chartUC.LoadDataToChart(SessionResultObs.Where(r => r.DoneAt > DateTime.Now.AddYears(-1)).OrderByDescending(r => r.DoneAt)); break;
      case "3Mon": f = "ddd d";    /**/ _chartUC.LoadDataToChart(SessionResultObs.Where(r => r.DoneAt > DateTime.Now.AddMonths(-3)).OrderByDescending(r => r.DoneAt)); break;
      case "Mont": f = "ddd d";    /**/ _chartUC.LoadDataToChart(SessionResultObs.Where(r => r.DoneAt > DateTime.Now.AddMonths(-1)).OrderByDescending(r => r.DoneAt)); break;
      case "Week": f = "ddd H:mm"; /**/ _chartUC.LoadDataToChart(SessionResultObs.Where(r => r.DoneAt > DateTime.Now.AddDays(-7)).OrderByDescending(r => r.DoneAt)); break;
      case "PreX": f = "ddd H:mm"; /**/ _chartUC.LoadDataToChart(SessionResultObs.OrderByDescending(r => r.DoneAt).Take(10)); break;
      case "Pre5": f = "H:mm";     /**/ _chartUC.LoadDataToChart(SessionResultObs.OrderByDescending(r => r.DoneAt).Take(05)); break;
      case "24hr": f = "H:mm";     /**/ _chartUC.LoadDataToChart(SessionResultObs.Where(r => r.DoneAt > DateTime.Now.AddDays(-1)).OrderByDescending(r => r.DoneAt)); break;
      case "1Day": f = "H:mm";     /**/ _chartUC.LoadDataToChart(SessionResultObs.Where(r => r.DoneAt > DateTime.Today).OrderByDescending(r => r.DoneAt)); break;
      case null:
      default: f = "MMM-dd"; break;
    }

    _chartUC.AxisX.First().LabelFormatter = value => DateTime.FromOADate(value).ToString(f);
  }

  void onDeleteSR(object x)
  {
    if (SelectSnRt == null) return;

    __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF($"Are you sure?");
    if (System.Windows.MessageBox.Show($"{SelectSnRt.DoneAt:MMM-dd HH:mm} \r\n\n{SelectSnRt.CpM} cpm\r\n\n{(SelectSnRt.IsRecord == true ? "It's a Record!!" : "")}", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
    {
      DeleteSaveSsnRsltToDb(SelectSnRt, _dbx);
    }
  }
  void promptSample()
  {
    /*
    var ps = new PromptStyle
    {
      Emphasis = PromptEmphasis.Strong,
      SystemVolume = PromptVolume.ExtraLoud
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
    __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF(pb);
    */
  }
  async void prepLessonType(string x, bool doF1 = true)
  {
    SubLesnId = x.Split('-').Last();

    switch (x[0])
    {
      case 'A':
      case 'D': LesnTyp = LessonType.DrillsInFile; break;
      case 'B': LesnTyp = LessonType.BasicLessons; break;
      case 'C': LesnTyp = LessonType.Combinations; break;
      case 'N': LesnTyp = LessonType.NumerSymbols; break;
      case 'E': LesnTyp = LessonType.EditableFile; break;
      case 'S': LesnTyp = LessonType.SpecialDrill; break;
      case 'P': LesnTyp = LessonType.PhrasesRandm; break;
      case 'X': LesnTyp = LessonType.Experimental; break;
      default: break;
    }

    var (lessonTxt, lessonLen) = LessonHelper.GetLesson(LesnTyp, SubLesnId);
    LessonText = lessonTxt;
    LessonLen = lessonLen;

    if (doF1)
    {

      {
        loadListsFromDB(DashName, SelectUser);
        await updateDoneTodo(SelectUser, __speechSynth, _dbx);
      }
    }

    CurInfo = $"{(LesnTyp)} - {SubLesnId:N0}  ";// ({DashName})";
  }
  async Task updateDoneTodo(string selectUser, SpeechSynth synth, OneBaseContext db)
  {
    int doneToday = -1, sinceRcrd = -1, todoToday = -1;
    try
    {
      doneToday = db.SessionResults.Count(r => r.UserId.ToLower() == selectUser.ToLower() && r.DoneAt > DateTime.Today);

      //if (!_dbx.SessionResults.Local.Any(r => r.UserId.Equals(selectUser, StringComparison.OrdinalIgnoreCase)))
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
    catch (Exception ex) { ex.Log(); synth.SpeakAsyncCancelAll(); await synth.SpeakAsync($"Something is not right: {ex.Message}. Talk to you later"); }

    DoneToday = doneToday;
    ExrzeRuns = sinceRcrd;
    TodoToday = todoToday;
  }

  static async Task<DateTime> getLatestGlobalRecordDate(string selectUser, OneBaseContext dbdbld) // not just the highest score!
  {
    var sw = Stopwatch.StartNew();
    var latestGlobalRecord = DateTime.Now.AddYears(-100);

    //await dbdbld.SessionResults.Where(r => r.UserId == selectUser).LoadAsync();

    try
    {
      var allExrczNames = dbdbld.SessionResults.Where(r => r.UserId == selectUser).GroupBy(r => r.ExcerciseName);
      foreach (var exrsName in allExrczNames)
      {
        if (dbdbld.SessionResults.Any(r => r.UserId == selectUser && r.ExcerciseName == exrsName.Key))
        {
          var record4userAndExrz = dbdbld.SessionResults.Where(r => r.UserId == selectUser && r.ExcerciseName == exrsName.Key).ToList().Max(r => r.CpM);
          var lastExrzRecordDate = dbdbld.SessionResults.ToList().Where(r => r.UserId == selectUser && r.ExcerciseName == exrsName.Key && r.CpM == record4userAndExrz).Max(r => r.DoneAt);
          if (latestGlobalRecord < lastExrzRecordDate)
          {
            latestGlobalRecord = lastExrzRecordDate;
            Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $" {exrsName.Key,-8}{dbdbld.SessionResults.Count(r => r.UserId == selectUser && r.ExcerciseName == exrsName.Key),5} times    max cpm {record4userAndExrz,3}   {lastExrzRecordDate}  ->  {latestGlobalRecord}");
          }
        }
      }
    }
    catch (Exception ex) { ex.Log(); }

    Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"  ... => latestGlobalRecord: {latestGlobalRecord}");

    Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceWarning, $"{DateTime.Now:yy.MM.dd-HH:mm:ss.f} +{(DateTime.Now - App.StartedAt):mm\\:ss\\.ff}    *** getLatestGlobalRecordDate() done in {sw.Elapsed.TotalSeconds:N2} sec");

    return latestGlobalRecord;
  }
}
