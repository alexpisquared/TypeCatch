using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AAV.Sys.Helpers;

namespace TypingWpf
{
  public static partial class LessonHelper
  {
    static readonly Random _random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
    public const int PaddingLen = 1; // for occasional ending with ' ' or '\r'.

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

      var allText = File.ReadAllText("Assets\\FreeTextLesson.txt");

      if (_random.Next(10) % 2 == 0)
      {
        return (allText.Substring(_random.Next(allText.Length - lessonLen - PaddingLen), lessonLen + PaddingLen), lessonLen);
      }
      else
      {
        var allLines = allText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        var sb = new StringBuilder();
        for (var i = 0; sb.Length < lessonLen && i < allLines.Length; i++)
        {
          sb.Append(allLines[_random.Next(allLines.Length)]).Append(Environment.NewLine);
        }

        var lessonBody = sb.Length > lessonLen + PaddingLen ? sb.ToString().Substring(0, lessonLen + PaddingLen) : sb.ToString();        //Trace.WriteLine($"sb:{sb.Length} chars,  rv:{lessonBody.Length} chars,  rv:{lessonBody.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length} lines.");
        return (lessonBody, lessonLen);
      }

    }

    private static string[] LoadAllLessons()
    {
      var lessons = File.ReadAllText("Assets\\TypingDrillList.txt").Split(new[] { "·" }, StringSplitOptions.RemoveEmptyEntries);

      Trace.WriteLine($"*** {lessons.Length} lessons");
      foreach (var lesson in lessons)
      {
        var allLines = lesson.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        Trace.WriteLine($"***   {allLines.First()}  {allLines.Length - 1,4} lines");
      }

      return lessons;
    }

    public static void CodeGen()
    {
      var i = 0;
      foreach (var s in _specialDrill.Split(new[] { "·", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
      {
        if (s.Length > 77 || s.StartsWith("\t")) continue;
        var ln = $"<MenuItem Header=\"_{i + 1,-2:X} {s,-30}\"      Command=\"{{Binding LTCmd}}\" CommandParameter=\"D-{i++}\"  />";
        Trace.TraceInformation(ln);
      }
    }
    public static (string lessonTxt, int lessonLen) GetLesson(LessonType lt, string sublesson)
    {
      string allLessonsForType;
      switch (lt)
      {
        case LessonType.BasicLessons: allLessonsForType = _basicLessons; break;
        case LessonType.Combinations: allLessonsForType = _combinations; break;
        case LessonType.DigitSymbols: allLessonsForType = _digitSymbols; break;
        case LessonType.SpecialDrill: allLessonsForType = _specialDrill; break;
        case LessonType.Experimental: allLessonsForType = getFromOneDrive(sublesson); return (allLessonsForType, allLessonsForType.Length);
        case LessonType.PhrasesRandm: return GetLesson_Phrases(sublesson);
        case LessonType.EditableFile: allLessonsForType = getCreateFromOneDrive(sublesson); return (allLessonsForType, allLessonsForType.Length);
        case LessonType.DrillsInFile: allLessonsForType = "Drills in file is under construction"; return (allLessonsForType, allLessonsForType.Length);
        default: allLessonsForType = $"Nothing for {lt}"; break;
      }

      var lessonArrayFromFile = LoadAllLessons();

      var lessonArray = allLessonsForType.Split(new[] { "· " }, StringSplitOptions.RemoveEmptyEntries);

      if (int.TryParse(sublesson, out var sublessonInt) && sublessonInt < lessonArray.Length)
      {
        var lessonHardcoded = lessonArray[sublessonInt].Trim(new[] { '\r', '\n' });

        var lessonHeaderName = lessonHardcoded.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).First();
        var lessonFromFile = lessonArrayFromFile.FirstOrDefault(r => r.StartsWith(lessonHeaderName));
        if (lessonFromFile != null)
          return (lessonFromFile, lessonFromFile.Length);

        return (lessonHardcoded, lessonHardcoded.Length);
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
    EditableFile,
    DrillsInFile
  }
}
