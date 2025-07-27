using TypeCatch.Net5.AsLink;
using TypingWpf.DbDataTran;

namespace TypingWpf.VMs;

public partial class MainVM : BindableBaseViewModel
{
  void onF2(object o)
  {
    //var tdy = DateTime.Today;
    //var gnd = (VoiceGender)(tdy.DayOfYear % 3 + 1);
    ////__speechSynth.Rate = 4;

    //// { Console.WriteLine("CULTURE ISO ISO WIN DISPLAYNAME                              ENGLISHNAME"); foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures)) { Console.Write("{0,-8}", ci.Name); Console.Write(" {0,-3}", ci.TwoLetterISOLanguageName); Console.Write(" {0,-3}", ci.ThreeLetterISOLanguageName); Console.Write(" {0,-3}", ci.ThreeLetterWindowsLanguageName); Console.Write(" {0,-40}", ci.DisplayName); Console.WriteLine(" {0,-40}", ci.EnglishName); } }

    //{
    //  var ci = new CultureInfo("zh-Hans");

    //  for (int va = 1; va < 3; va++)
    //  {
    //    foreach (InstalledVoice iv in __speechSynth.GetInstalledVoices())
    //    {
    //      __speechSynth.SelectVoice(iv.VoiceInfo.Name);
    //      __speechSynth.SpeakAsync($"{iv.VoiceInfo.Description} {gnd}. {ci.Name}.  voice Alternate = {va}.");

    //      __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Child,   /**/ va, ci); __speechSynth.SpeakAsync($"{VoiceAge.Child } {va}");
    //      __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Teen,    /**/ va, ci); __speechSynth.SpeakAsync($"{VoiceAge.Teen  } {va}");
    //      __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Adult,   /**/ va, ci); __speechSynth.SpeakAsync($"{VoiceAge.Adult } {va}");
    //      __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Senior,  /**/ va, ci); __speechSynth.SpeakAsync($"{VoiceAge.Senior} {va}");

    //      var pr = __speechSynth.GetCurrentlySpokenPrompt();
    //      Trace.WriteLine($"{pr}");
    //    }
    //  }

    //  promptSample();

    //  if (Debugger.IsAttached)
    //  {
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Child, 1, ci); __speechSynth.SpeakAsync($"{VoiceAge.Child }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Teen, 1, ci); __speechSynth.SpeakAsync($"{VoiceAge.Teen  }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Adult, 1, ci); __speechSynth.SpeakAsync($"{VoiceAge.Adult }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Senior, 1, ci); __speechSynth.SpeakAsync($"{VoiceAge.Senior}");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Child, 2, ci); __speechSynth.SpeakAsync($"{VoiceAge.Child }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Teen, 2, ci); __speechSynth.SpeakAsync($"{VoiceAge.Teen  }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Adult, 2, ci); __speechSynth.SpeakAsync($"{VoiceAge.Adult }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Senior, 2, ci); __speechSynth.SpeakAsync($"{VoiceAge.Senior}");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Child, 3, ci); __speechSynth.SpeakAsync($"{VoiceAge.Child }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Teen, 3, ci); __speechSynth.SpeakAsync($"{VoiceAge.Teen  }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Adult, 3, ci); __speechSynth.SpeakAsync($"{VoiceAge.Adult }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Senior, 3, ci); __speechSynth.SpeakAsync($"{VoiceAge.Senior}");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Child, 4, ci); __speechSynth.SpeakAsync($"{VoiceAge.Child }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Teen, 4, ci); __speechSynth.SpeakAsync($"{VoiceAge.Teen  }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Adult, 4, ci); __speechSynth.SpeakAsync($"{VoiceAge.Adult }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Senior, 4, ci); __speechSynth.SpeakAsync($"{VoiceAge.Senior}");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Child, 5, ci); __speechSynth.SpeakAsync($"{VoiceAge.Child }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Child, 6, ci); __speechSynth.SpeakAsync($"{VoiceAge.Child }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Child, 7, ci); __speechSynth.SpeakAsync($"{VoiceAge.Child }");
    //    __speechSynth.SelectVoiceByHints(gnd, VoiceAge.Child, 8, ci); __speechSynth.SpeakAsync($"{VoiceAge.Child }");
    //  }
    //}
  }
  void onF3(object o) => SoundPlayer.Test();
  async void onF4(object o)
  {
    //runTreatIfAny();
    await updateDoneTodo(SelectUser, __speechSynth, db);
    __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF($"Total done Since Day 0: {ExrzeRuns}, done Today, {DoneToday}, left for today: {TodoToday}");
  }
  void onF5(object o) => sessionLoad_Start_lazy();
  void onF6(object o) { __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF(string.IsNullOrEmpty(PreSelect) ? "Nothing is preselected" : $"I see there is something to play: {PreSelect}."); }
  void onF7(object o) => Process.Start("Explorer.exe", "IsoHelper.GetIsoFolder()");
  void onF8(object o)
  {
    if (System.Windows.Clipboard.ContainsFileDropList())
    {
      Trace.WriteLine("");
      var ff = System.Windows.Clipboard.GetFileDropList();
      if (ff.Count > 0)
      {
        __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF($"looks like an {(File.Exists(ff[0]) ? "" : "non-")}existing file with extension {Path.GetExtension(ff[0])}.");

        PreSelect = ff[0];
      }
      else
      {
        __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF($"Nothing found here.");
      }
    }
    else if (System.Windows.Clipboard.ContainsText())
    {
      PreSelect = System.Windows.Clipboard.GetText();
      __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF(PreSelect.StartsWith("http") ? $"looks like a URL." : $"Does not Look like URL.");
    }
  }
  void onF9(object o)
  {
    var (_, report, _) = new DataTransfer().CopyChunkyAzureSynch(A0DbMdl.GetA0DbMdlLocalDb, A0DbMdl.GetA0DbMdl); InfoMsg = $"{report}";
  }
  void onFA(object o) => ProLTgl = !ProLTgl;
  void onFB(object o) { /*Process.Start("Explorer.exe", Tracing.RemoteLogFolder);*/ }
  void onFC(object o) => Process.Start("Explorer.exe", OneDrive.Folder(LessonHelper.ExercizeDir));
}
