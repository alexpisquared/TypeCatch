using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AAV.Sys.Helpers;
using TypeCatch.Net5.Misc;

namespace TypingWpf;

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

  public static List<DrillData> LoadDrillDataArray()
  {
    var lessons = File.ReadAllText("Assets\\TypingDrillList.txt").Split(new[] { "·" }, StringSplitOptions.RemoveEmptyEntries);
    List<DrillData> rv = new();

    Trace.WriteLine($"*** {lessons.Length} lessons");
    foreach (var lesson in lessons)
    {
      var allLines = lesson.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      var headerpp = allLines.First().Split(new char[] { '≈' }, StringSplitOptions.RemoveEmptyEntries);
      Trace.WriteLine($"***   {allLines.First()}  {allLines.Length - 1,4} lines   {"dsf".Substring(1, 2)}");

      rv.Add(new DrillData(headerpp[^1].Trim('≡'), headerpp[0], lesson.Split(new char[] { '≡' }, StringSplitOptions.RemoveEmptyEntries)[^1].Trim(new[] { '\r', '\n' }))); // .last() == [^1]
    }

    foreach (var dd in rv)
      Trace.WriteLine($"***   {dd.SqlExcerciseName,-6}  {dd.Header}  {dd.Content[..8]}");

    return rv;
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
    switch (lt)
    {
      case LessonType.PhrasesRandm: return GetLesson_Phrases(sublesson);
      case LessonType.EditableFile: { var lessonContent = getCreateFromOneDrive(sublesson); return (lessonContent, lessonContent.Length); }
      case LessonType.DrillsInFile: { var lessonContent = LoadDrillDataArray()[int.Parse(sublesson)].Content; return (lessonContent, lessonContent.Length); }
      case LessonType.BasicLessons: return GetHardcodedLessonObsolete(sublesson, _basicLessons);
      case LessonType.Combinations: return GetHardcodedLessonObsolete(sublesson, _combinations);
      case LessonType.NumerSymbols: return GetHardcodedLessonObsolete(sublesson, _digitSymbols);
      case LessonType.SpecialDrill: return GetHardcodedLessonObsolete(sublesson, _specialDrill);
      case LessonType.Experimental: return (getFromOneDrive(sublesson), getFromOneDrive(sublesson).Length);
      default: return GetHardcodedLessonObsolete(sublesson, $"Nothing for {lt}");
    }
  }

  private static (string lessonTxt, int lessonLen) GetHardcodedLessonObsolete(string sublesson, string allLessonsForType)
  {
    var lessonArray = allLessonsForType.Split(new[] { "· " }, StringSplitOptions.RemoveEmptyEntries);

    if (int.TryParse(sublesson, out var sublessonInt) && sublessonInt < lessonArray.Length)
    {
      var lessonHardcoded = lessonArray[sublessonInt].Trim(new[] { '\r', '\n' });

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
  NumerSymbols,
  SpecialDrill,
  PhrasesRandm,
  Experimental, // resolved from next by numer vs tla
  EditableFile, // resolved from prev by tla vs numer
  DrillsInFile
}
