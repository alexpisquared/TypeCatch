﻿using MVVM.Common;
using System;
using System.Diagnostics;
using AsLink;
using System.Windows;
using System.IO;
using System.Globalization;
using TypingWpf.DbDataTran;
using TypingWpf.DbMdl;
using AAV.Sys.Helpers;
using SpeechSynthLib.Adapter;

namespace TypingWpf.VMs
{
  public partial class MainVM : BindableBaseViewModel
  {
    void onF2(object o)
    {
      var tdy = DateTime.Today;
      var gnd = (VoiceGender)(tdy.DayOfYear % 3 + 1);
      synth.Rate = 4;

      // { Console.WriteLine("CULTURE ISO ISO WIN DISPLAYNAME                              ENGLISHNAME"); foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures)) { Console.Write("{0,-8}", ci.Name); Console.Write(" {0,-3}", ci.TwoLetterISOLanguageName); Console.Write(" {0,-3}", ci.ThreeLetterISOLanguageName); Console.Write(" {0,-3}", ci.ThreeLetterWindowsLanguageName); Console.Write(" {0,-40}", ci.DisplayName); Console.WriteLine(" {0,-40}", ci.EnglishName); } }

      {
        var ci = new CultureInfo("zh-Hans");

        for (int va = 1; va < 3; va++)
        {
          foreach (InstalledVoice iv in synth.GetInstalledVoices())
          {
            synth.SelectVoice(iv.VoiceInfo.Name);
            synth.Speak($"{iv.VoiceInfo.Description} {gnd}. {ci.Name}.  voice Alternate = {va}.");

            synth.SelectVoiceByHints(gnd, VoiceAge.Child,   /**/ va, ci); synth.Speak($"{VoiceAge.Child } {va}");
            synth.SelectVoiceByHints(gnd, VoiceAge.Teen,    /**/ va, ci); synth.Speak($"{VoiceAge.Teen  } {va}");
            synth.SelectVoiceByHints(gnd, VoiceAge.Adult,   /**/ va, ci); synth.Speak($"{VoiceAge.Adult } {va}");
            synth.SelectVoiceByHints(gnd, VoiceAge.Senior,  /**/ va, ci); synth.Speak($"{VoiceAge.Senior} {va}");

            var pr = synth.GetCurrentlySpokenPrompt();
            Trace.WriteLine($"{pr}");
          }
        }

        promptSample();

        if (Debugger.IsAttached)
        {
          synth.SelectVoiceByHints(gnd, VoiceAge.Child, 1, ci); synth.Speak($"{VoiceAge.Child }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Teen, 1, ci); synth.Speak($"{VoiceAge.Teen  }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Adult, 1, ci); synth.Speak($"{VoiceAge.Adult }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Senior, 1, ci); synth.Speak($"{VoiceAge.Senior}");
          synth.SelectVoiceByHints(gnd, VoiceAge.Child, 2, ci); synth.Speak($"{VoiceAge.Child }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Teen, 2, ci); synth.Speak($"{VoiceAge.Teen  }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Adult, 2, ci); synth.Speak($"{VoiceAge.Adult }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Senior, 2, ci); synth.Speak($"{VoiceAge.Senior}");
          synth.SelectVoiceByHints(gnd, VoiceAge.Child, 3, ci); synth.Speak($"{VoiceAge.Child }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Teen, 3, ci); synth.Speak($"{VoiceAge.Teen  }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Adult, 3, ci); synth.Speak($"{VoiceAge.Adult }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Senior, 3, ci); synth.Speak($"{VoiceAge.Senior}");
          synth.SelectVoiceByHints(gnd, VoiceAge.Child, 4, ci); synth.Speak($"{VoiceAge.Child }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Teen, 4, ci); synth.Speak($"{VoiceAge.Teen  }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Adult, 4, ci); synth.Speak($"{VoiceAge.Adult }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Senior, 4, ci); synth.Speak($"{VoiceAge.Senior}");
          synth.SelectVoiceByHints(gnd, VoiceAge.Child, 5, ci); synth.Speak($"{VoiceAge.Child }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Child, 6, ci); synth.Speak($"{VoiceAge.Child }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Child, 7, ci); synth.Speak($"{VoiceAge.Child }");
          synth.SelectVoiceByHints(gnd, VoiceAge.Child, 8, ci); synth.Speak($"{VoiceAge.Child }");
        }
      }
    }
    void onF3(object o) { _soundPlayer.Test(); }
    async void onF4(object o)
    {
      //runTreatIfAny();
      await updateDoneTodo(SelectUser, synth, A0DbMdl.GetA0DbMdlAzureDb);
      synth.SpeakAsyncCancelAll(); synth.SpeakAsync($"Total done Since Day 0: {ExrzeRuns}, done Today, {DoneToday}, left for today: {TodoToday}");
    }
    void onF5(object o) { sessionLoad_Start_lazy(); }
    void onF6(object o) { synth.SpeakAsyncCancelAll(); synth.SpeakAsync(string.IsNullOrEmpty(PreSelect) ? "Nothing is preselected" : $"I see there is something to play: {PreSelect}."); }
    void onF7(object o) { Process.Start("Explorer.exe", IsoHelper.GetIsoFolder()); }
    void onF8(object o)
    {
      if (Clipboard.ContainsFileDropList())
      {
        Trace.WriteLine("");
        var ff = Clipboard.GetFileDropList();
        if (ff.Count > 0)
        {
          synth.SpeakAsyncCancelAll(); synth.SpeakAsync($"looks like an {(File.Exists(ff[0]) ? "" : "non-")}existing file with extension {Path.GetExtension(ff[0])}.");

          PreSelect = (ff[0]);
        }
        else
        {
          synth.SpeakAsyncCancelAll(); synth.SpeakAsync($"Nothing found here.");
        }
      }
      else if (Clipboard.ContainsText())
      {
        PreSelect = Clipboard.GetText();
        synth.SpeakAsyncCancelAll(); synth.SpeakAsync(PreSelect.StartsWith("http") ? $"looks like a URL." : $"Does not Look like URL.");
      }
    }
    void onF9(object o) { var rv = new DataTransfer().CopyChunkyAzureSynch(A0DbMdl.GetA0DbMdlLocalDb, A0DbMdl.GetA0DbMdlAzureDb); InfoMsg = $"{rv.report}"; }
    void onFA(object o) { ProLTgl = !ProLTgl; }
    void onFB(object o) { /*Process.Start("Explorer.exe", Tracing.RemoteLogFolder);*/ }
    void onFC(object o) { Process.Start("Explorer.exe", AAV.Sys.Helpers.OneDrive.Folder(LessonHelper.ExercizeDir)); }
  }
}
