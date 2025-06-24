using TypeCatch.Net5.Mdl;
using static AmbienceLib.SpeechSynth;
using dbMdl = TypingWpf.DbMdl;

namespace TypingWpf.VMs;

public partial class MainVM
{
  public string VpcExe => new[] { @"C:\Users\apigida\OneDrive\Public\bin\VPC.exe", @"C:\1\bin\VPC.exe", @"C:\c\MP\VPC\VPC\bin\x86\Release\VPC.exe", @"O:\OneDrive\Public\bin\VPC.exe" }.FirstOrDefault(File.Exists);

  async void OnUserInput()
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
        __speechSynth.SpeakFAF("Stopped.");
      }
    }
    else
    {
      _prevCharLenAt333 = PupilInput.Length;
      if (!_swMain.IsRunning)
      {
        Debug.Write($" vv Starting +++++++++++++++++ ... ");
        _swMain.Start();
        __speechSynth.SpeakFAF("StartedAt.");
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

    CrntCpm = PupilInput.Length / _swMain.Elapsed.TotalMinutes; //CurInfo = $" Typed {(PupilInput.Length),3:N0}\t Elapsed {_sw.Elapsed:mm\\:ss}\t Speed {CrntCpm:N0}";
    IsRecord = CrntCpm > _recordCpm;

    DateTime now = DateTime.Now;
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
    (string lessonTxt, int lessonLen) = LessonHelper.GetLesson(LesnTyp, SubLesnId);
    LessonText = lessonTxt;
    LessonLen = lessonLen;

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
    
    _resourcePlayer.SystemVolume = ushort.MaxValue / 3;

    try
    {
      IsInSsn = false;
      Console.Beep(500, 50);
      await Task.Delay(32); //todo: why so slow in reflecting in the UI???

      if (!_swMain.IsRunning || _swMain.ElapsedTicks == 0) // if has not been started yet.
        return;

      _swMain.Stop();

      if (PupilInput.Trim().Length < LessonLen - 3)
      {
        //__speechSynth.SpeakFAF($"Oops, too short! Let's give it another shot, superstar!", speakingRate: 1.4);
        return;
      }

      if (!IsCorrect) { __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF($"Almost there! Remember to finish typing till the last letter. Hitting Escape is like ghosting the lesson - not cool!"); return; }

      var prevRcrdCpm = RcrdCpm;
      var thisResult = new dbMdl.SessionResult { Duration = _swMain.Elapsed, ExcerciseName = DashName, PokedIn = PupilInput.Length, UserId = SelectUser, Note = $"{Environment.MachineName[..2].ToLower()}{Environment.MachineName[^1..]}", DoneAt = DateTime.Now };
      if (thisResult.CpM < .333 * prevRcrdCpm)
      {
        __speechSynth.SpeakAsyncCancelAll(); __speechSynth.SpeakFAF($"Whoa there, slow poke! Let's kick it up a notch and show this keyboard who's boss!");
        return;
      }

      //var pb = new PromptBuilder();        for (int i = 0; i < 10; i++) { pb.AppendText($"{1 + i} mississippi ", i % 2 > 0 ? PromptEmphasis.Strong : PromptEmphasis.Reduced); }        __speechSynth.SpeakFAF(pb); // __speechSynth.SpeakFAF("The end. Storing the results... 1 mississippi 2 mississippi 3 mississippi 4 mississippi 5 mississippi 6 mississippi 7 mississippi 8 mississippi 9 mississippi 10 mississippi 11 mississippi 12 mississippi 13 mississippi 14 mississippi 15 mississippi 16 mississippi 17 mississippi 18 mississippi 19 ");        await Task.Delay(3333);

      var swStoring = Stopwatch.StartNew();

      if (thisResult.CpM > prevRcrdCpm)
        thisResult.IsRecord = true;

      using (A0DbMdl db = A0DbMdl.GetA0DbMdl)
      {
        //2019-12/           _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.OrderByDescending(r => r.DoneAt).Take(10).ToList());
        //2019-12/           await Task.Delay(50);

        _ = db.SessionResults.Add(SelectSnRt = thisResult);
        await updateSettings(db);
        _ = await db.TrySaveReportAsync();
        loadListsFromDB(DashName, SelectUser, db);
        await updateDoneTodo(SelectUser, __speechSynth, db);

        //_chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.OrderByDescending(r => r.DoneAt).Take(10).ToList());
        //_chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.Where(r => r.DoneAt > DateTime.Now.AddMonths(-1)).OrderByDescending(r => r.DoneAt)); /////////////////////////////////////////
        _chartUC.LoadDataToChart(CurUserCurExcrsRsltLst.Where(r => r.DoneAt > DateTime.Today).OrderByDescending(r => r.DoneAt)); /////////////////////////////////////////

        //__speechSynth.SpeakFAF("OK?");
        //await Task.Delay(99);
      }

      IsFocusedPI = false;
      IsFocusedSB = true;
      IsFocusedSB = false;
      IsFocusedSB = true;

      //Task.Run(() =>
      {
        if (thisResult.CpM > prevRcrdCpm)
        {
          SoundPlayer.PlaySessionFinish_Good();
          __speechSynth.SpeakAsyncCancelAll(); await __speechSynth.SpeakAsync($"Wow! Congratulations! That is actually a record! {thisResult.CpM - prevRcrdCpm} characters per minute faster or {100 * (thisResult.CpM - prevRcrdCpm) / prevRcrdCpm} percent improvement!");
        }
        else
        {
          //Jun 2019: too old: SoundPlayer.PlaySessionFinish_Baad();

          if (TodoToday <= 0)
          {
            __speechSynth.SpeakAsyncCancelAll();
            await __speechSynth.SpeakAsync($"You are off the hook for today.");
          }
          else
          {
            var randomMessage =
                thisResult.CpM > .99 * prevRcrdCpm ? $"OMG! You're on fire!  New record incoming any second now!" :
                thisResult.CpM > .98 * prevRcrdCpm ? $"Yaass queen! You're so close to smashing that record!" :
                thisResult.CpM > .97 * prevRcrdCpm ? $"Holy guacamole! That was insanely good!" :
                thisResult.CpM > .96 * prevRcrdCpm ? $"Slay! You're this close to a new personal best!" :
                thisResult.CpM > .95 * prevRcrdCpm ? $"You're crushing it! Keep slaying!" :
                thisResult.CpM > .94 * prevRcrdCpm ? $"So close to perfection! You've got this!" :
                thisResult.CpM > .93 * prevRcrdCpm ? $"Yesss! Keep that awesome energy going!" :
                thisResult.CpM > .92 * prevRcrdCpm ? $"You're on a roll! Don't stop now!" :
                thisResult.CpM > .91 * prevRcrdCpm ? $"Wowza!  You're giving me life right now!" :
                thisResult.CpM > .90 * prevRcrdCpm ? $"Looking fierce!  That record is shaking!" :
                thisResult.CpM > .89 * prevRcrdCpm ? $"You're glowing up!  Keep shining!" :
                thisResult.CpM > .88 * prevRcrdCpm ? $"Serving looks and speed! Work it!" :
                thisResult.CpM > .87 * prevRcrdCpm ? $"Yaaaas! Even better than before! " :
                thisResult.CpM > .86 * prevRcrdCpm ? $"Okay, I see you! That was lit!" :
                thisResult.CpM > .85 * prevRcrdCpm ? $"You're leveling up! Next stop: record city!" :
                thisResult.CpM > .84 * prevRcrdCpm ? $"Nice one! But I bet you can slay even harder! " :
                thisResult.CpM > .83 * prevRcrdCpm ? $"Killin' it! Keep that vibe going!" :
                thisResult.CpM > .82 * prevRcrdCpm ? $"You did that! Now do it again, but fiercer!" :
                thisResult.CpM > .81 * prevRcrdCpm ? $"Getting better by the second! Time to glow up!" :
                thisResult.CpM > .80 * prevRcrdCpm ? $"Yesss! Now let's take it to the next level!" :
                thisResult.CpM > .79 * prevRcrdCpm ? $"Deep breath, and channel your inner boss babe!" :
                thisResult.CpM > .78 * prevRcrdCpm ? $"Not too shabby! But I know you've got more sass in you!" :
                thisResult.CpM > .77 * prevRcrdCpm ? $"Good start! Now let's make it Instagram-worthy! " :
                thisResult.CpM > .76 * prevRcrdCpm ? $"Nice try! But let's aim for TikTok viral, okay?" :
                thisResult.CpM > .75 * prevRcrdCpm ? $"Getting warmer! Time to bring the heat! " :
                thisResult.CpM > .74 * prevRcrdCpm ? $"You've got potential! Now unleash your inner diva!" :
                thisResult.CpM > .73 * prevRcrdCpm ? $"Remember: effort is everything! You've got this!" :
                thisResult.CpM > .72 * prevRcrdCpm ? $"Good, but not yet Insta-story worthy! Let's change that!" :
                thisResult.CpM > .71 * prevRcrdCpm ? $"Time to show these keys who's boss!" :
                thisResult.CpM > .70 * prevRcrdCpm ? $"Channel your fave pop star energy!  You can do better!" :
                thisResult.CpM > .69 * prevRcrdCpm ? $"Take a deep breath, and let's make this keyboard your runway!" :
                thisResult.CpM > .68 * prevRcrdCpm ? $"You're on your way! Now let's make it iconic! " :
                thisResult.CpM > .67 * prevRcrdCpm ? $"Pick up the pace, bestie! You've got this!" :
                thisResult.CpM > .66 * prevRcrdCpm ? $"Let's turn up the beat! Show me what you've got!" :
                thisResult.CpM > .65 * prevRcrdCpm ? $"Time to glow up your typing game!  I believe in you!" :
                thisResult.CpM > .64 * prevRcrdCpm ? $"C'mon, this keyboard isn't going to slay itself! " :
                thisResult.CpM > .63 * prevRcrdCpm ? $"Warm-up's over! Time to make these keys your BFF! " :
                thisResult.CpM > .62 * prevRcrdCpm ? $"Let's see some sparkle in those fingertips! " :
                thisResult.CpM > .61 * prevRcrdCpm ? $"Okay, but like, for real this time? " :
                thisResult.CpM > .60 * prevRcrdCpm ? $"Cute warm-up! Now let's see your true power! " :
                thisResult.CpM > .59 * prevRcrdCpm ? $"Hey girl, I know you've got more sass than that!" :
                thisResult.CpM > .58 * prevRcrdCpm ? $"We're aiming for main character energy here!" :
                thisResult.CpM > .57 * prevRcrdCpm ? $"Earth to bestie! Time to wake up those fingers! " :
                thisResult.CpM > .56 * prevRcrdCpm ? $"Last year called, it wants its typing speed back! " :
                thisResult.CpM > .55 * prevRcrdCpm ? $"Rise and shine, sleeping beauty! The keyboard misses you!" :
                thisResult.CpM > .54 * prevRcrdCpm ? $"Even your cat could type faster! (Just kidding, prove me wrong!)" :
                thisResult.CpM > .53 * prevRcrdCpm ? $"Hello? Is this thing on? We need more energy!" :
                thisResult.CpM > .52 * prevRcrdCpm ? $"Oops! I think you forgot to hit the slay button! " :
                thisResult.CpM > .51 * prevRcrdCpm ? $"Remember: slower than this, and we'll have to call it a nap! " :
                thisResult.CpM > .50 * prevRcrdCpm ? $"Phew! You barely made the cut. Now let's see some real magic! " :
                thisResult.CpM > .47 * prevRcrdCpm ? $"Houston, we have a problem!  Time to blast off!" :
                thisResult.CpM > .45 * prevRcrdCpm ? $"Your future self called, she types way faster! " :
                thisResult.CpM > .40 * prevRcrdCpm ? $"Even Internet Explorer loads faster than this!  You can do better!" :
                thisResult.CpM > .33 * prevRcrdCpm ? $"Okay, bestie, I'll count this one, but let's pretend it never happened and start fresh! " :
                $"Yikes! This is too slow to count. But don't worry, you've got this! Shake it off and try again! ";

            __speechSynth.SpeakAsyncCancelAll();

            var r = new Random(DateTime.Now.Microsecond);
            switch (r.Next(2))
            {
              default:
              case 0: await __speechSynth.SpeakAsync(randomMessage, volumePercent: 75, voice: CC.Aria, style: CC.EnusAriaNeural.Styles[new Random(DateTime.Now.Microsecond).Next(CC.EnusAriaNeural.Styles.Length)], role: CC.Girl); break;
              case 1: await __speechSynth.SpeakAsync(randomMessage, volumePercent: 75, voice: CC.Xiaomo, style: CC.ZhcnXiaomoNeural.Styles[new Random(DateTime.Now.Microsecond).Next(CC.ZhcnXiaomoNeural.Styles.Length)], role: CC.Girl); break;
            }

            __speechSynth.SpeakFree($"{DoneToday} down; {TodoToday} to go.", speakingRate: 5);

            return;
          }
        }

        runTreatIfAny();
        Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceWarning, $"{DateTime.Now:yy.MM.dd-HH:mm:ss.f} +{DateTime.Now - App.StartedAt:mm\\:ss\\.ff}    *** finishTheSession() done in {sw.Elapsed.TotalSeconds:N1} sec.");

      }//);//.ContinueWith(_ => onF1_UpdateCpmRecord(), TaskScheduler.FromCurrentSynchronizationContext());
    }
    catch (Exception ex) { _ = ex.Log(); __speechSynth.SpeakAsyncCancelAll(); await __speechSynth.SpeakAsync($"Something is not right with {PreSelect}. Exception details are: {ex.Message}. Talk to you later"); }
  }

  public static List<dbMdl.SessionResult> GetLatestLessnns(IEnumerable<dbMdl.SessionResult> curLessons) => [.. curLessons.OrderByDescending(r => r.DoneAt).Take(10).ToList()];

  async void runTreatIfAny()
  {
    if (string.IsNullOrEmpty(PreSelect))
    {
      return; //__speechSynth.SpeakAsyncCancelAll(); await __speechSynth.Speak("Nothing in the treat section!");
    }

    _resourcePlayer.SystemVolume = ushort.MaxValue / 3;

    try
    {
      Process process = Directory.Exists(PreSelect) ? Process.Start(VpcExe, PreSelect) : Process.Start(PreSelect);
      double minToWatch = 30;
      if (DateTime.Now.TimeOfDay.TotalHours < 20.5)      /**/ { minToWatch = 30; __speechSynth.SpeakAsyncCancelAll(); await __speechSynth.SpeakAsync($"You deserve a break now! Here is a {minToWatch} minute treat for you. Enjoy!"); }
      else if (DateTime.Now.TimeOfDay.TotalHours < 20.7) /**/ { minToWatch = 15; __speechSynth.SpeakAsyncCancelAll(); await __speechSynth.SpeakAsync($"You deserve a break now! 9 o'clock is fast approaching; you have {minToWatch} minutes to enjoy."); }
      else if (DateTime.Now.TimeOfDay.TotalHours < 21.0) /**/ { minToWatch = 10; __speechSynth.SpeakAsyncCancelAll(); await __speechSynth.SpeakAsync($"You deserve a break now! It is almost 9 o'clock now; you have {minToWatch} minutes to enjoy."); }
      else                                               /**/ { minToWatch = 05; __speechSynth.SpeakAsyncCancelAll(); await __speechSynth.SpeakAsync($"You deserve a break now! It is past 9 o'clock now; have a short {minToWatch} minute break. To have a longer movie time, start earlier tomorrow."); }

      // System.Threading.Thread.Sleep(TimeSpan.FromMinutes(minToWatch));
      await Task.Delay(TimeSpan.FromMinutes(minToWatch));
      if (!process.CloseMainWindow())
      {
        process.Close();
        process.Kill();
      }

      System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
      __speechSynth.SpeakAsyncCancelAll(); await __speechSynth.SpeakAsync($"Oopsy.. The {minToWatch} minute treat is over! What you wanna do now?");

    }
    catch (Exception ex) { _ = ex.Log(); __speechSynth.SpeakAsyncCancelAll(); await __speechSynth.SpeakAsync($"Something is not right with {PreSelect}. Exception details are: {ex.Message}. Talk to you later"); }
  }
}
