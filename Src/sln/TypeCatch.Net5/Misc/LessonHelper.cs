using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using AAV.Sys.Helpers;

namespace TypingWpf
{
  public static partial class LessonHelper
  {
    static readonly Random _random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
    public const int PaddingLen = 250;

    static (string lessonTxt, int lessonLen) GetLesson_Phrases(string lessonLenStr)
    {
      if (!int.TryParse(lessonLenStr, out var lessonLen))
        lessonLen = 100;

      if (lessonLen < 10)
      {
        lessonLen =
          lessonLen == 0 ? 10 :
          lessonLen == 1 ? 100 :
          lessonLen == 2 ? 200 :
          lessonLen == 3 ? 300 :
          lessonLen == 4 ? 400 :
          lessonLen == 5 ? 500 :
          100;
      }

      string txt;

      if (_random.Next(10) % 2 == 0)
      {
        var random = _random.Next(AllPhrases.Length - lessonLen - PaddingLen);
        var nextCr = AllPhrases.IndexOf("\n", random) + 1;
        if (nextCr + lessonLen + PaddingLen < AllPhrases.Length)
          random = nextCr;

        txt = AllPhrases.Substring(random, lessonLen + PaddingLen); // .Trim(); LessonLen = LessonText.Length;
      }
      else
      {
        var allLines = AllPhrases.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        var sb = new StringBuilder();
        for (var i = 0; sb.Length < lessonLen && i < allLines.Length; i++)
        {
          sb.Append(allLines[_random.Next(allLines.Length)]).Append(Environment.NewLine);
        }

        txt = sb.ToString();

        Trace.WriteLine($"sb:{sb.Length} chars,  rv:{txt.Length} chars,  rv:{txt.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length} lines.");
      }

      return (txt, lessonLen);
    }
    public static void CodeGen()
    {
      var i = 0;
      foreach (var s in _specialDrill.Split(new[] { "[#]", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
      {
        if (s.Length > 77 || s.StartsWith("\t")) continue;
        var ln = $"<MenuItem Header=\"_{i + 1,-2:X} {s,-30}\"      Command=\"{{Binding LTCmd}}\" CommandParameter=\"D-{i++}\"  />";
        Trace.TraceInformation(ln);
      }
    }
    public static (string lessonTxt, int lessonLen) GetLesson(LessonType _0to3, string sublesson)
    {
      string s;
      switch (_0to3)
      {
        case LessonType.BasicLessons: s = _basicLessons; break;
        case LessonType.Combinations: s = _combinations; break;
        case LessonType.DigitSymbols: s = _digitSymbols; break;
        case LessonType.SpecialDrill: s = _specialDrill; break;
        case LessonType.Experimental: s = getFromOneDrive(sublesson); return (s, s.Length);
        case LessonType.PhrasesRandm: return GetLesson_Phrases(sublesson);
        case LessonType.EditableFile: s = getCreateFromOneDrive(sublesson); return (s, s.Length);
        default: s = $"Nothing for {_0to3}"; break;
      }

      var ss = s.Split(new[] { "[#]" }, StringSplitOptions.RemoveEmptyEntries);

      if (int.TryParse(sublesson, out var sublessonInt))
        if (sublessonInt < ss.Length)
        {
          var sss = ss[sublessonInt].Split(new[] { '\n' });
          if (sss.Length > 1)
          {
            s = sss[1].Trim(new char[] { ' ', '\r', '\n', '\t' });
            return (s, s.Length);
          }
        }

      return ("????????", 9);
    }

    public static Dictionary<string, string> EditableFiles
    {
      get
      {
        if (_files == null)
        {
          _files = new Dictionary<string, string>();
          foreach (var file in Directory.GetFiles(OneDrive.Folder(_exercizeDir), "*.txt"))
          {
            _files.Add(Path.GetFileNameWithoutExtension(file), File.ReadAllText(file));
          }
        }
        return _files;
      }
    }
    static Dictionary<string, string> _files = null;


    static string getCreateFromOneDrive(string sublesson)
    {
      if (EditableFiles.ContainsKey(sublesson) && !string.IsNullOrWhiteSpace(EditableFiles[sublesson]))
        return EditableFiles[sublesson];
      else
      {
        var file = OneDrive.Folder($@"{_exercizeDir}{sublesson}.txt");
        if (!File.Exists(file))
        {
          var dflt =
              (sublesson.Equals("msa")) ? string.Format(_msa, string.IsNullOrEmpty(Environment.UserDomainName.ToLower()) ? "random" : Environment.UserDomainName.ToLower(), Environment.UserName.ToLower()) :
              (sublesson.Equals("nymi")) ? string.Format(_nym, Environment.UserName.ToLower()) :
              typeof(LessonHelper).GetField($"_{sublesson}", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null).ToString();

          using (var fs = File.Create(file))
          {
            var info = new UTF8Encoding(true).GetBytes(dflt ?? $"Feel free to replace this with whatever excercise you deem fit...\r\n...preferably matching the topic of '{sublesson}'.");
            fs.Write(info, 0, info.Length);
            fs.Close();
          }

          var p = Process.Start("Notepad.exe", file);
          p.WaitForExit();
        }

        _files.Add(Path.GetFileNameWithoutExtension(file), File.ReadAllText(file));

        return EditableFiles[sublesson];
      }
    }
    static string getFromOneDrive(string sublessonId)
    {
      var file = OneDrive.Folder($@"{_exercizeDir}Excersize_{sublessonId}.txt");
      if (!File.Exists(file))
      {
        //Clipboard.SetText(file);
        File.Create(file).Close();
        var p = Process.Start("Notepad.exe", file);
        p.WaitForExit();
      }

      return File.ReadAllText(file);
    }

    public static string ExercizeDir => _exercizeDir;
    #region Excercises - hardcoded and defaults:
    #endregion
  }

  public enum LessonType
  {
    BasicLessons,
    Combinations,
    DigitSymbols,
    SpecialDrill,
    PhrasesRandm,
    Experimental,
    EditableFile
  }
}
