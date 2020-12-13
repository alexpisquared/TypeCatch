using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingWpf.Resources
{
    public class ObsMainVM
    {
        public int AppRunCount { get; set; }
        public bool Audible { get; set; }
        public int CrntCpm { get; set; }
        public string CurInfo { get; set; }
        public int DoneToday { get; set; }
        public int ExrzeRuns { get; set; }
        public string InfoMsg { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBusy { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsFocusedPI { get; set; }
        public bool IsFocusedSB { get; set; }
        public bool IsInSsn { get; set; }
        public bool IsRecord { get; set; }
        //public bool IsSHort { get; set; }
        public int LesnTyp { get; set; }
        public int LessonLen { get; set; }
        public string LessonText { get; set; }
        public int MainVis { get; set; }
        public int MaxCpm { get; set; }
        public int Opcty { get; set; }
        public object PreSelect { get; set; }
        public object[] PrgsChart { get; set; }
        public bool ProLTgl { get; set; }
        public int ProLvl { get; set; }
        public string PupilInput { get; set; }
        public int RcrdCpm { get; set; }
        public object SelectSnRt { get; set; }
        public string SelectUser { get; set; }
        public Snrt[] SnRts { get; set; }
        public Soundplayer SoundPlayer { get; set; }
        public string SubLesnId { get; set; }
        public int TodoToday { get; set; }
        public Userlessonlst[] CurUserCurExcrsRsltLst { get; set; }
        public string[] Users { get; set; }
        public string VersioInfo { get; set; }
        public Snrt1[] snrts { get; set; }
    }

    public class Soundplayer
    {
        public int Volume { get; set; }
    }

    public class Snrt
    {
        public Doneat DoneAt { get; set; }
        public TimeSpan Duration { get; set; }
        public string ExcerciseName { get; set; }
        public string Notes { get; set; }
        public int PokedIn { get; set; }
        public string UserId { get; set; }
    }

    public class Doneat
    {
        public DateTime DateTime { get; set; }
        public int OffsetMinutes { get; set; }
    }

    public class Userlessonlst
    {
        public Doneat1 DoneAt { get; set; }
        public TimeSpan Duration { get; set; }
        public string ExcerciseName { get; set; }
        public string Notes { get; set; }
        public int PokedIn { get; set; }
        public string UserId { get; set; }
    }

    public class Doneat1
    {
        public DateTime DateTime { get; set; }
        public int OffsetMinutes { get; set; }
    }

    public class Snrt1
    {
        public Doneat2 DoneAt { get; set; }
        public TimeSpan Duration { get; set; }
        public string ExcerciseName { get; set; }
        public string Notes { get; set; }
        public int PokedIn { get; set; }
        public string UserId { get; set; }
    }

    public class Doneat2
    {
        public DateTime DateTime { get; set; }
        public int OffsetMinutes { get; set; }
    }
}
