using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using AsLink;
using TypeCatch.Net5;
using TypingWpf.Mdl;
using dbMdl = TypingWpf.DbMdl;

namespace TypingWpf.VMs
{
  public partial class MainVM
  {
    public string VpcExe => new[] { @"C:\Users\apigida\OneDrive\Public\bin\VPC.exe", @"C:\1\bin\VPC.exe", @"C:\c\MP\VPC\VPC\bin\x86\Release\VPC.exe", @"O:\OneDrive\Public\bin\VPC.exe" }.FirstOrDefault(f => File.Exists(f));

    async void onUserInput(object o)
    {
      if (!IsInSsn || PupilInput.Length == 0) return;

      IsCorrect = LessonText.Contains(PupilInput);

      if (PupilInput.Length == 1) //!_swMain.IsRunning) // messed up with autoPause.
      {
        _swMain.Start();
        _nextMeasureTime = DateTime.Now.Add(_measurePeriod);
        Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $">>> {_swMain.ElapsedMilliseconds,9:N0} - StartedAt!    <<< onUserInput");
      }

      if (!IsCorrect)
        SoundPlayer.PlayОw();
      else if (PupilInput.Length >= LessonLen)
        await finishTheSession();
    }
    void autoPause()
    {
      Debug.Write($"*** {_swMain.ElapsedMilliseconds,9:N0} -- {_swMain.IsRunning,9} -> ");
      if (_prevCharLenAt333 == PupilInput.Length)
      {
        if (_swMain.IsRunning)
        {
          Debug.Write($" ^^ Stopping ---------------- ... ");
          _swMain.Stop();
          synth.SpeakFaF("Stopped.");
        }
      }
      else
      {
        _prevCharLenAt333 = PupilInput.Length;
        if (!_swMain.IsRunning)
        {
          Debug.Write($" vv Starting +++++++++++++++++ ... ");
          _swMain.Start();
          synth.SpeakFaF("StartedAt.");
        }
      }
      Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $" - {_swMain.IsRunning,9}");
    }

    int _prevCharLen = 0, _prevCharLenAt333 = 0;
    double _prevMinutes = 0;
    async Task tick333ms()
    {
      if (_swMain.Elapsed.TotalMinutes == 0) return;

      //autoPause();

      CrntCpm = (int)(PupilInput.Length / _swMain.Elapsed.TotalMinutes); //CurInfo = $" Typed {(PupilInput.Length),3:N0}\t Elapsed {_sw.Elapsed:mm\\:ss}\t Speed {CrntCpm:N0}";
      IsRecord = CrntCpm > _recordCpm;

      var now = DateTime.Now;
      if (_swMain.IsRunning && PupilInput.Length > 0 && now > _nextMeasureTime)
      {
        var dLen = PupilInput.Length - _prevCharLen;
        var dMin = _swMain.Elapsed.TotalMinutes - _prevMinutes;
        if (dMin > 0)
          PrgsChart.Add(new VeloMeasure { SppedCpm = (int)(dLen / dMin), PrevRcrd = _recordCpm });

        _prevMinutes = _swMain.Elapsed.TotalMinutes;
        _prevCharLen = PupilInput.Length;
        _nextMeasureTime = now.Add(_measurePeriod);
      }

      await Task.Yield();
    }


    async void sessionLoad_Start_lazy() // timing will start on the first keystroke.
    {
      LessonText = LessonHelper.GetLesson(LesnTyp, SubLesnId);
      LessonLen = LesnTyp == LessonType.PhrasesRandm ? LessonText.Length - LessonHelper.PaddingLen : LessonText.Length;

      PupilInput = "";
      _swMain.Reset();
      /*updateUserLessonLst(false)*/
      ;
      PrgsChart.Clear();
      IsInSsn = IsCorrect = true;

      IsFocusedSB = false; await Task.Delay(9); refreshUiSynch(); await refreshUi();
      IsFocusedPI = false; await Task.Delay(9); refreshUiSynch(); await refreshUi();
      IsFocusedPI = true; await Task.Delay(99); refreshUiSynch(); await refreshUi(); //this acrobatics seems to bring focus to the textbox.
    }
    async Task finishTheSession()
    {
      var sw = Stopwatch.StartNew();
      try
      {
        IsInSsn = false;    //todo: why so slow in reflecting in the UI???
        await Task.Yield();
        Bpr.BeepClk();

        if (!_swMain.IsRunning || _swMain.ElapsedTicks == 0) // if has not been started yet.
          return;

        _swMain.Stop();

        if (PupilInput.Length == 0)
          return;

        if (PupilInput.Trim().Length < LessonLen - 3)
        {
#if !DEBUG
          synth.SpeakAsyncCancelAll(); await synth.Speak($"Does not count: Too short!");
#endif
          return;
        }

        if (!IsCorrect) { synth.SpeakAsyncCancelAll(); synth.SpeakFaF($"Does not count: Mistyped! Always finish typing till the last letter. Pressing Escape button ruins/discards the lesson."); return; }

        var prevRcrdCpm = RcrdCpm;
        var thisResult = new dbMdl.SessionResult { Duration = _swMain.Elapsed, ExcerciseName = DashName, PokedIn = PupilInput.Length, UserId = SelectUser, Note = $"{Environment.MachineName.Substring(0, 2).ToLower()}{Environment.MachineName.Substring(Environment.MachineName.Length - 1)}", DoneAt = DateTime.Now };
        if (thisResult.CpM < .333 * prevRcrdCpm)
        {
          synth.SpeakAsyncCancelAll(); synth.SpeakFaF($"Does not count: Very-very-very- V. E. R. Y. slow!");
          return;
        }

        //var pb = new PromptBuilder();        for (int i = 0; i < 10; i++) { pb.AppendText($"{1 + i} mississippi ", i % 2 > 0 ? PromptEmphasis.Strong : PromptEmphasis.Reduced); }        synth.SpeakFaF(pb); // synth.SpeakFaF("The end. Storing the results... 1 mississippi 2 mississippi 3 mississippi 4 mississippi 5 mississippi 6 mississippi 7 mississippi 8 mississippi 9 mississippi 10 mississippi 11 mississippi 12 mississippi 13 mississippi 14 mississippi 15 mississippi 16 mississippi 17 mississippi 18 mississippi 19 ");        await Task.Delay(3333);
        
        var swStoring = Stopwatch.StartNew();
        
        if (thisResult.CpM > prevRcrdCpm)
          thisResult.IsRecord = true;

        using (var db = dbMdl.A0DbMdl.GetA0DbMdlAzureDb)
        {
          //2019-12/           _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.OrderByDescending(r => r.DoneAt).Take(10).ToList());
          //2019-12/           await Task.Delay(50);

          synth.SpeakAsyncCancelAll(); await synth.Speak("adding");    /**/ db.SessionResults.Add(SelectSnRt = thisResult);                                                           /**/       //      synth.SpeakAsyncCancelAll(); await synth.Speak("added");
          synth.SpeakAsyncCancelAll(); await synth.Speak("seting");    /**/ await updateSettings(db);                                                                                 /**/       //      synth.SpeakAsyncCancelAll(); await synth.Speak("setingsed");
          synth.SpeakAsyncCancelAll(); await synth.Speak("saving");    /**/ await db.TrySaveReportAsync();                                                                            /**/       //      synth.SpeakAsyncCancelAll(); await synth.Speak("saved");
          synth.SpeakAsyncCancelAll(); await synth.Speak("loading");   /**/ loadListsFromDB(DashName, SelectUser, db);                                                                /**/       //      synth.SpeakAsyncCancelAll(); await synth.Speak("loaded");
          synth.SpeakAsyncCancelAll(); await synth.Speak("todoing");   /**/ await updateDoneTodo(SelectUser, synth, db);                                                              /**/       //      synth.SpeakAsyncCancelAll(); await synth.Speak("todoed");
          synth.SpeakAsyncCancelAll(); await synth.Speak("charting");  /**/ _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.OrderByDescending(r => r.DoneAt).Take(10).ToList());      /**/       //      synth.SpeakAsyncCancelAll(); await synth.Speak("charted");
        }

        synth.SpeakAsyncCancelAll(); await synth.Speak($"took {swStoring.Elapsed.TotalSeconds:N0} seconds.");

        IsFocusedPI = false;
        IsFocusedSB = true;
        IsFocusedSB = false;
        IsFocusedSB = true;

        //Task.Run(() =>
        {
          if (thisResult.CpM > prevRcrdCpm)
          {
            SoundPlayer.PlaySessionFinish_Good();
            synth.SpeakAsyncCancelAll(); await synth.Speak($"Wow! Congratulations! That is actually a record! {thisResult.CpM - prevRcrdCpm} characters per minute faster or {100 * (thisResult.CpM - prevRcrdCpm) / prevRcrdCpm} percent improvement!");
          }
          else
          {
            //Jun 2019: too old: SoundPlayer.PlaySessionFinish_Baad();

            synth.SpeakAsyncCancelAll(); await synth.Speak($"{thisResult.CpM}");

            if (TodoToday > 0)
            {
              var t1 = (
                        thisResult.CpM > .99 * prevRcrdCpm ? $"OMG! That was really-really R-E-A-L-L-Y close. I am so excited! New record is coming up!" :
                        thisResult.CpM > .98 * prevRcrdCpm ? $"OMG! That was really REALLY close. New record is coming." :
                        thisResult.CpM > .97 * prevRcrdCpm ? $"Oh my god! That was awesome!" :
                        thisResult.CpM > .96 * prevRcrdCpm ? $"Oh my god! That was amazing - almost a brand new record!" :
                        thisResult.CpM > .95 * prevRcrdCpm ? $"Brilliant!" :
                        thisResult.CpM > .94 * prevRcrdCpm ? $"Perfect! Almost ..." :
                        thisResult.CpM > .93 * prevRcrdCpm ? $"Well done! Keep it up! " :
                        thisResult.CpM > .92 * prevRcrdCpm ? $"Fantastic! " :
                        thisResult.CpM > .91 * prevRcrdCpm ? $"Exciting!" :
                        thisResult.CpM > .90 * prevRcrdCpm ? $"Looks promising..." :
                        thisResult.CpM > .89 * prevRcrdCpm ? $"Looking pretty." :
                        thisResult.CpM > .88 * prevRcrdCpm ? $"Looking good..." :
                        thisResult.CpM > .87 * prevRcrdCpm ? $"Even nicer!" :
                        thisResult.CpM > .86 * prevRcrdCpm ? $"N. I. C. E. Nice!" :
                        thisResult.CpM > .85 * prevRcrdCpm ? $"Getting there..." :
                        thisResult.CpM > .84 * prevRcrdCpm ? $"Nice! But could be nicer..." :
                        thisResult.CpM > .83 * prevRcrdCpm ? $"Well executed!" :
                        thisResult.CpM > .82 * prevRcrdCpm ? $"Well done!" :
                        thisResult.CpM > .81 * prevRcrdCpm ? $"Better-ish!" :
                        thisResult.CpM > .80 * prevRcrdCpm ? $"Better!" :
                        thisResult.CpM > .69 * prevRcrdCpm ? $"Take a deep breath, and - double the effort!" :
                        thisResult.CpM > .78 * prevRcrdCpm ? $"Not bad!" :
                        thisResult.CpM > .77 * prevRcrdCpm ? $"Good! But needs improvement..." :
                        thisResult.CpM > .76 * prevRcrdCpm ? $"Good! But could be better..." :
                        thisResult.CpM > .75 * prevRcrdCpm ? $"Good! But could be much better..." :
                        thisResult.CpM > .74 * prevRcrdCpm ? $"Good! But could be much-much better..." :
                        thisResult.CpM > .73 * prevRcrdCpm ? $"Remember: making an effort is all what counts." :
                        thisResult.CpM > .72 * prevRcrdCpm ? $"Good! But not good..." :
                        thisResult.CpM > .71 * prevRcrdCpm ? $"Try to make an effort! " :
                        thisResult.CpM > .70 * prevRcrdCpm ? $"Try a tiny bit harder! " :
                        thisResult.CpM > .69 * prevRcrdCpm ? $"A deep breath, and - buckle up!" :
                        thisResult.CpM > .68 * prevRcrdCpm ? $"Kind of getting there..." :
                        thisResult.CpM > .67 * prevRcrdCpm ? $"Still too slow..." :
                        thisResult.CpM > .66 * prevRcrdCpm ? $"Kind of slow..." :
                        thisResult.CpM > .65 * prevRcrdCpm ? $"Kind of sluggish..." :
                        thisResult.CpM > .64 * prevRcrdCpm ? $"That was a snail pace, if you ask me." :
                        thisResult.CpM > .63 * prevRcrdCpm ? $"Warm up or not, but move it, will you!" :
                        thisResult.CpM > .62 * prevRcrdCpm ? $"Warm up or not, but it must be faster!" :
                        thisResult.CpM > .61 * prevRcrdCpm ? $"I'll take it as a joke." :
                        thisResult.CpM > .60 * prevRcrdCpm ? $"I'll take it as a warm up." :
                        thisResult.CpM > .59 * prevRcrdCpm ? $"Hey you! Be serious!" :
                        thisResult.CpM > .58 * prevRcrdCpm ? $"That was not even interesting..." :
                        thisResult.CpM > .57 * prevRcrdCpm ? $"Is anybody awake up there?" :
                        thisResult.CpM > .56 * prevRcrdCpm ? $"You typed faster a year ago." :
                        thisResult.CpM > .55 * prevRcrdCpm ? $"Hey you! Wake up already!" :
                        thisResult.CpM > .54 * prevRcrdCpm ? $"You typed faster when you were 3." :
                        thisResult.CpM > .53 * prevRcrdCpm ? $"Hello-o-o-o-o!" :
                        thisResult.CpM > .52 * prevRcrdCpm ? $"That was just silly!" :
                        thisResult.CpM > .51 * prevRcrdCpm ? $"Remember: slower than this does not count." :
                        thisResult.CpM > .50 * prevRcrdCpm ? $"Just barely made it: a bit slower - and it would not count. " :
                        thisResult.CpM > .47 * prevRcrdCpm ? $"Za-ra-za." :
                        thisResult.CpM > .45 * prevRcrdCpm ? $"AFAIK, Your grandpa types faster." :
                        thisResult.CpM > .40 * prevRcrdCpm ? $"Your grandma types faster." :
                        thisResult.CpM > .33 * prevRcrdCpm ? $"OK, monkey, I will count in this disgracefully slow run this time, but you'd better do not repeat such a horrible performance. " :
                        $"This message never played, right? Are you kidding me! This is too slow and is not counted.");

              synth.SpeakAsyncCancelAll(); await synth.Speak($"{t1} {DoneToday} down; {TodoToday} to go.");
              return;
            }
          }

          runTreatIfAny();
          Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceWarning, $"{DateTime.Now:yy.MM.dd-HH:mm:ss.f} +{(DateTime.Now - App.StartedAt):mm\\:ss\\.ff}    *** finishTheSession() done in {sw.Elapsed.TotalSeconds:N1} sec.");

        }//);//.ContinueWith(_ => onF1_UpdateCpmRecord(), TaskScheduler.FromCurrentSynchronizationContext());
      }
      finally { }
    }

    public static List<dbMdl.SessionResult> GetLatestCurLessnons(IEnumerable<dbMdl.SessionResult> curLessons) => new List<dbMdl.SessionResult>(curLessons.OrderByDescending(r => r.DoneAt).Take(10).ToList());

    async void runTreatIfAny()
    {
      if (string.IsNullOrEmpty(PreSelect))
      {
        return; //synth.SpeakAsyncCancelAll(); await synth.Speak("Nothing in the treat section!");
      }
      try
      {
        Process process;
        if (Directory.Exists(PreSelect))
          process = Process.Start(VpcExe, PreSelect);
        else
          process = Process.Start(PreSelect);

        double minToWatch = 30;
        if (DateTime.Now.TimeOfDay.TotalHours < 20.5)      /**/ { minToWatch = 30; synth.SpeakAsyncCancelAll(); await synth.Speak($"You deserve a break now! Here is a {minToWatch} minute treat for you. Enjoy!"); }
        else if (DateTime.Now.TimeOfDay.TotalHours < 20.7) /**/ { minToWatch = 15; synth.SpeakAsyncCancelAll(); await synth.Speak($"You deserve a break now! 9 o'clock is fast approaching; you have {minToWatch} minutes to enjoy."); }
        else if (DateTime.Now.TimeOfDay.TotalHours < 21.0) /**/ { minToWatch = 10; synth.SpeakAsyncCancelAll(); await synth.Speak($"You deserve a break now! It is almost 9 o'clock now; you have {minToWatch} minutes to enjoy."); }
        else                                               /**/ { minToWatch = 05; synth.SpeakAsyncCancelAll(); await synth.Speak($"You deserve a break now! It is past 9 o'clock now; have a short {minToWatch} minute break. To have a longer movie time, start earlier tomorrow."); }

        // System.Threading.Thread.Sleep(TimeSpan.FromMinutes(minToWatch));
        await Task.Delay(TimeSpan.FromMinutes(minToWatch));
        if (!process.CloseMainWindow())
        {
          process.Close();
          process.Kill();
        }

        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
        synth.SpeakAsyncCancelAll(); await synth.Speak($"Oopsy.. The {minToWatch} minute treat is over! What you wanna do now?");

      }
      catch (Exception ex) { ex.Log(); synth.SpeakAsyncCancelAll(); await synth.Speak($"Something is not right with {PreSelect}. Exception details are: {ex.Message}. Talk to you later"); }
    }
  }
}
