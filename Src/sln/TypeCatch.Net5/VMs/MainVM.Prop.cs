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
  [ObservableProperty] string searchText = ""; partial void OnSearchTextChanged(string value) { SessionResultCvs.Refresh(); Console.Beep(222, 222); InfoMsg = $"{((ListCollectionView)SessionResultCvs).Count} matches so far."; }
  [ObservableProperty] string _PupilInput = ""; partial void OnPupilInputChanged(string value) => OnUserInput();
  [ObservableProperty] int _RcrdCpm = 100; partial void OnRcrdCpmChanged(int value) { if (value > _MaxCpm) MaxCpm = 2 * value; } //   if (_recordCpm > _MaxCpm) MaxCpm = 2 * RcrdCpm; } } }
  [ObservableProperty] User _SlctUser; partial void OnSlctUserChanged(User value) => SelectUser = value.UserId;
  [ObservableProperty] string _SelectUser; partial void OnSelectUserChanged(string value) => loadListsFromDB(getTheLatestLessonTypeTheUserWorksOn(_dbx), value, _dbx);
  [ObservableProperty] LessonType _LesnTyp; partial void OnLesnTypChanged(LessonType oldValue, LessonType newValue) { if (SubLesnId != null) { sessionLoad_Start_lazy(); } }
  [ObservableProperty] string _LessonText = "Loading...";
  [ObservableProperty] int _ExrzeRuns = 300;
  [ObservableProperty] int _DoneToday = 300;
  [ObservableProperty] int _TodoToday = 300;
  [ObservableProperty] int _MaxCpm = 300;
  [ObservableProperty] double _CrntCpm = 00;
  [ObservableProperty] string _SubLesnId = "1";
  [ObservableProperty] int _AppRunCount = 00;
  [ObservableProperty] string _CurInfo;
  [ObservableProperty] string _InfoMsg;
  [ObservableProperty] string _PreSelect;
  [ObservableProperty] string _VersioInfo;
  [ObservableProperty] SessionResult _SelectSnRt;
  [ObservableProperty] bool _ProLTgl;                            //  if (Set(ref _ProLTgl, value)) { ProLvl = value ? 1 : .7; } } }
  [ObservableProperty] double _ProLvl = .7;
  [ObservableProperty] bool _IsInSsn = true;
  [ObservableProperty] bool _Audible = false;                    //  if (Set(ref _Audible, value)) { SoundPlayer.SystemVolume = value ? ushort.MaxValue : (ushort)0; } } }
  [ObservableProperty] bool _IsFocusedPI = true;                 //  if (_IsFocusedPI == value) { _ = Set(ref _IsFocusedPI, !value); } _ = Set(ref _IsFocusedPI, value); } }
  [ObservableProperty] bool _IsFocusedSB = true;                 //  if (_IsFocusedSB == value) { _ = Set(ref _IsFocusedSB, !value); } _ = Set(ref _IsFocusedSB, value); } }
  [ObservableProperty] bool _IsCorrect = true;
  [ObservableProperty] bool _IsRecord = false;
  [ObservableProperty] bool _IsAdmin = false;
  [ObservableProperty] bool _IsBusy = false;
  [ObservableProperty] Visibility _MainVis = Visibility.Visible; 
  public ObservableCollection<VeloMeasure> PrgsChart { get; set; } = [];
  public int LessonLen;// => (LessonText.Length - LessonHelper.PaddingLen) > 8 ? LessonText.Length - LessonHelper.PaddingLen : LessonText.Length;
  public string DashName => $"{LesnTyp.ToString()[0]}-{SubLesnId}";
  const int _planPerDay = 3;
  public ResourcePlayer SoundPlayer { get; set; } = new ResourcePlayer();
}