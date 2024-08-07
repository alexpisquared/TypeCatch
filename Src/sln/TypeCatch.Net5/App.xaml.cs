using TypeCatch.Net5.AsLink;

namespace TypeCatch.Net5;

public partial class App : Application
{
  public static readonly DateTime StartedAt = DateTime.Now;
  public static TraceSwitch // copy for orgl in C:\C\Lgc\ScrSvrs\AAV.SS\App.xaml.cs
    AppTraceLevel_Config = new("CfgTraceLevelSwitch", "Switch in config file:  <system.diagnostics><switches><!--0-off, 1-error, 2-warn, 3-info, 4-verbose. --><add name='CfgTraceLevelSwitch' value='3' /> "),
    AppTraceLevel_inCode = new("Verbose________Trace", "This is the trace for all               messages.") { Level = TraceLevel.Verbose },
    AppTraceLevel_Warnng = new("ErrorAndWarningTrace", "This is the trace for Error and Warning messages.") { Level = TraceLevel.Warning };

  protected override /*async*/ void OnStartup(StartupEventArgs e)
  {
    Bpr.Beep1of2();

    Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;

    //todo?Tracer.SetupTracingOptions("TypingWpf", AppTraceLevel_Warnng);

    base.OnStartup(e);

#if _DEBUG
    await sw();
    //MainVM.UpdateDoneTodo("Zoe", null);
    //DeadlockDemo.NoDeadlock();
    //DeadlockDemo.Causes_A_Deadlock();

    //Trace.WriteLine(LessonHelper.GetLesson(LessonType.Combinations, 1));
    //LessonHelper.CodeGen();
    //new xChartPoc().ShowDialog(); 
    //IsoHelper.ListIsoFolders(); return;
    //new Vws.DbExplorer2().ShowDialog(); // new JsonToSqlMigrator().ShowDialog();
#else
    var mw = new MainWindow();
    var vm = MainVM.Create(mw.cChart1);
    if (vm != null)
      BindableBaseViewModel.ShowModalMvvmAsync(vm, mw);
    else
#endif
      Shutdown();
  }
  protected override async void OnExit(ExitEventArgs e) // a copy from AlexPi.Scr: there seems to be an issue with the process hanging in after the explicit call of the Shutdown() method.
  {
    base.OnExit(e);

    await Task.Delay(1500); // j.i.c.

    Trace.WriteLine($"{DateTime.Now:yy.MM.dd HH:mm:ss.f} +{DateTime.Now - StartedAt:mm\\:ss\\.ff}   App.OnExit() => Process.GetCurrentProcess().Kill();   <= nothing should happen after this");
    Process.GetCurrentProcess().Kill();
    Trace.WriteLine($"{DateTime.Now:yy.MM.dd HH:mm:ss.f} +{DateTime.Now - StartedAt:mm\\:ss\\.ff}   App.OnExit() => never got here!");
    Environment.Exit(87);
    Environment.FailFast("Environment.FailFast");
  }

  public static Stopwatch SW = Stopwatch.StartNew();
  async Task swUnitTest()
  {
    var s = Stopwatch.StartNew();
    await Task.Delay(100); Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"{s.ElapsedMilliseconds:N0}");
    await Task.Delay(100); Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"{s.ElapsedMilliseconds:N0}");
    await Task.Delay(100); Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"{s.ElapsedMilliseconds:N0}");
    s.Stop();
    await Task.Delay(100); Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"{s.ElapsedMilliseconds:N0} stopped - should not change.");
    s.Start();
    await Task.Delay(100); Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"{s.ElapsedMilliseconds:N0}");
    s.Restart();
    await Task.Delay(100); Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"{s.ElapsedMilliseconds:N0}");
  }

  static readonly string _dbUser = Environment.UserName.Equals("alexp") ? "Alex" : Environment.UserName.Equals("APigida") ? "Alex" : Environment.UserName;

  //#if DEBUG
  //;      //_dbfn = OneDrive.Folder($@"Public\AppData\TypeCatch\ZoePi\TypeCatchDb.ZoePi.rls.mdf");
  //#else
  //    _dbfn = OneDrive.Folder($@"Public\AppData\TypeCatch\TypeCatchDb.{_dbUser}.rls.mdf");
  //#endif

  public static string Dbfn { get; } = OneDrive.Folder($@"Public\AppData\TypeCatch\TypeCatchDb.ZoePi.rls.mdf");
}
/*
IF (NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'TCh')) 
BEGIN
  EXEC ('CREATE SCHEMA [TCh] AUTHORIZATION [dbo]')
END
go

ALTER SCHEMA TCh     TRANSFER dbo.AppStng
ALTER SCHEMA TCh     TRANSFER dbo.SessionResult
ALTER SCHEMA TCh     TRANSFER dbo.User
*/
