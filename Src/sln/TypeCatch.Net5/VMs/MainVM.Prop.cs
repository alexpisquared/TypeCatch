using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Data;
using TypeCatch.Net5.AsLink;
using TypeCatch.Net5.Mdl;

namespace TypingWpf.VMs;

public partial class MainVM : BindableBaseViewModel
{
  [ObservableProperty] ObservableCollection<SessionResult> curUserCurExcrsRsltLst = [];
  [ObservableProperty] ICollectionView sessionResultCvs = CollectionViewSource.GetDefaultView(new List<SessionResult>());
  [ObservableProperty] string searchText = ""; partial void OnSearchTextChanged(string value)  {    SessionResultCvs.Refresh(); Console.Beep(222, 222);    InfoMsg = $"{((ListCollectionView)SessionResultCvs).Count} matches so far.";  }

  public ObservableCollection<VeloMeasure> PrgsChart { get; set; } = [];

  public int LessonLen;// => (LessonText.Length - LessonHelper.PaddingLen) > 8 ? LessonText.Length - LessonHelper.PaddingLen : LessonText.Length;

  public string DashName => $"{LesnTyp.ToString()[0]}-{SubLesnId}";

  int _DoneAtAll = 300;           /**/ public int ExrzeRuns { get => _DoneAtAll; set { if (Set(ref _DoneAtAll, value)) {; } } }
  int _DoneToday = 300;           /**/ public int DoneToday { get => _DoneToday; set { if (Set(ref _DoneToday, value)) {; } } }
  int _TodoToday = 300;           /**/ public int TodoToday { get => _TodoToday; set { if (Set(ref _TodoToday, value)) {; } } }

  int _MaxCpm = 300;              /**/ public int MaxCpm { get => _MaxCpm; set { if (Set(ref _MaxCpm, value)) {; } } }
  double _cc = 00;                /**/ public double CrntCpm { get => _cc; set => Set(ref _cc, value); }
  string _SubLesnId = "1";        /**/ public string SubLesnId { get => _SubLesnId; set => Set(ref _SubLesnId, value); }
  int _AppRunCount = 00;          /**/ public int AppRunCount { get => _AppRunCount; set => Set(ref _AppRunCount, value); }
  int _recordCpm = 100;           /**/ public int RcrdCpm { get => _recordCpm; set { if (Set(ref _recordCpm, value)) { if (_recordCpm > _MaxCpm) MaxCpm = 2 * RcrdCpm; } } }
  string _CurentInfo;             /**/ public string CurInfo { get => _CurentInfo; set { if (Set(ref _CurentInfo, value)) {; } } }
  string _InfoMsg;                /**/ public string InfoMsg { get => _InfoMsg; set => Set(ref _InfoMsg, value); }
  string _PreSelect;              /**/ public string PreSelect { get => _PreSelect; set { if (Set(ref _PreSelect, value)) {; } } }
  string _LessonText = "\r\n\n\t  W A I T !    \r\n\n\t\t Loading ... "; public string LessonText { get => _LessonText; set { if (Set(ref _LessonText, value)) {; } } }
  string _PupilInput;             /**/ public string PupilInput { get => _PupilInput; set { if (Set(ref _PupilInput, value)) { OnUserInput(); } } }
  string _VersioInfo;             /**/ public string VersioInfo { get => _VersioInfo; set { if (Set(ref _VersioInfo, value)) {; } } }
  SessionResult _SelectSnRt;   /**/ public SessionResult SelectSnRt { get => _SelectSnRt; set { if (Set(ref _SelectSnRt, value)) {; } } }//..Trace.WriteLine($"SelSsnRt: {value}"); } }
  User _SlctUser;              /**/ public User SlctUser { get => _SlctUser; set { if (Set(ref _SlctUser, value)) SelectUser = SlctUser.UserId; } }
  string _SelectUser;             /**/ public string SelectUser { get => _SelectUser; set { if (Set(ref _SelectUser, value) /*&& _isLoaded*/) { { loadListsFromDB(getTheLatestLessonTypeTheUserWorksOn(db), value, db); } } } }
  bool _ProLTgl;                  /**/ public bool ProLTgl { get => _ProLTgl; set { if (Set(ref _ProLTgl, value)) { ProLvl = value ? 1 : .7; } } }
  double _ProLvl = .7;            /**/ public double ProLvl { get => _ProLvl; set => Set(ref _ProLvl, value); }
  double _Opcty = 1;              /**/ public double Opcty { get => _Opcty; set => Set(ref _Opcty, value); }
  bool _IsInSsn = true;           /**/ public bool IsInSsn { get => _IsInSsn; set { if (Set(ref _IsInSsn, value)) {; } } }
  bool _Audible = false;          /**/ public bool Audible { get => _Audible; set { if (Set(ref _Audible, value)) { SoundPlayer.SystemVolume = value ? ushort.MaxValue : (ushort)0; } } }
  bool _IsFocusedPI = true;       /**/ public bool IsFocusedPI { get => _IsFocusedPI; set { if (_IsFocusedPI == value) { _ = Set(ref _IsFocusedPI, !value); } _ = Set(ref _IsFocusedPI, value); } }
  bool _IsFocusedSB = true;       /**/ public bool IsFocusedSB { get => _IsFocusedSB; set { if (_IsFocusedSB == value) { _ = Set(ref _IsFocusedSB, !value); } _ = Set(ref _IsFocusedSB, value); } }
  bool _IsCorrect = true;         /**/ public bool IsCorrect { get => _IsCorrect; set { if (Set(ref _IsCorrect, value)) {; } } }
  bool _IsRecord = false;         /**/ public bool IsRecord { get => _IsRecord; set { if (Set(ref _IsRecord, value)) {; } } }
  bool _IsAdmin = false;          /**/ public bool IsAdmin { get => _IsAdmin; set { if (Set(ref _IsAdmin, value)) {; } } }
  bool _IsBusy = false;           /**/ public bool IsBusy { get => _IsBusy; set { if (Set(ref _IsBusy, value) && value) { /*InfoMsg = "StartedAt ..."; */ ; } } }
  LessonType _LesnTyp;            /**/ public LessonType LesnTyp { get => _LesnTyp; set { if (Set(ref _LesnTyp, value) && SubLesnId != null) { sessionLoad_Start_lazy(); } } }
  Visibility _vis = Visibility.Visible; public Visibility MainVis { get => _vis; set { if (Set(ref _vis, value)) { Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, ""); ; } } }

  readonly DateTime _Date0 = new(2017, 3, 27);
  const int _planPerDay = 2;

  public ResourcePlayer SoundPlayer { get; set; } = new ResourcePlayer();

  //[JsonIgnore]
  //[XmlIgnore]
  //[NonSerialized]
  //Brush _SpeedClr;                /**/ public Brush SpeedClr { get { return _SpeedClr; } set { if (Set(ref _SpeedClr, value)) {; } } }

  //[XmlIgnore]
  //public Func<double, string> XFormatter_ProperWay { get; set; }
  //[XmlIgnore]
  //internal SeriesCollection Series_TODO_TU { get; set; }
}
