using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using AAV.WPF.Helpers;
using MVVM.Common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using TypingWpf.VMs;
using TypingWpf;

namespace TypeCatch.Net5
{
  public partial class App : Application
  {
    public static readonly DateTime Started = DateTime.Now;
    public static TraceSwitch // copy for orgl in C:\C\Lgc\ScrSvrs\AAV.SS\App.xaml.cs
      AppTraceLevel_Config = new TraceSwitch("CfgTraceLevelSwitch", "Switch in config file:  <system.diagnostics><switches><!--0-off, 1-error, 2-warn, 3-info, 4-verbose. --><add name='CfgTraceLevelSwitch' value='3' /> "),
      AppTraceLevel_inCode = new TraceSwitch("Verbose________Trace", "This is the trace for all               messages.") { Level = TraceLevel.Verbose },
      AppTraceLevel_Warnng = new TraceSwitch("ErrorAndWarningTrace", "This is the trace for Error and Warning messages.") { Level = TraceLevel.Warning };

    static readonly string
      _dbUser = Environment.UserName.Equals("alexp") ? "Alex" : Environment.UserName.Equals("APigida") ? "Alex" : Environment.UserName,
#if DEBUG
      _dbfn = AAV.Sys.Helpers.OneDrive.Folder($@"Public\AppData\TypeCatch\TypeCatchDb.ZoePi.rls.mdf");      //_dbfn = OneDrive.Folder($@"Public\AppData\TypeCatch\ZoePi\TypeCatchDb.ZoePi.rls.mdf");
#else
      _dbfn = AAV.Sys.Helpers.OneDrive.Folder($@"Public\AppData\TypeCatch\TypeCatchDb.{_dbUser}.rls.mdf");
#endif
    public static string Dbfn => _dbfn;

    public static Stopwatch SW = Stopwatch.StartNew();

    protected override /*async*/ void OnStartup(StartupEventArgs e)
    {
      Bpr.BeepShort();

      Application.Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;

      AAV.Sys.Helpers.Tracer.SetupTracingOptions("TypingWpf", AppTraceLevel_Warnng);

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
        /*await*/
        BindableBaseViewModel.ShowModalMvvmAsync(vm, mw);
      else
#endif
        Shutdown();
    }

    async Task sw()
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
    protected override void OnExit(ExitEventArgs e) { base.OnExit(e); Trace.TraceInformation($" {DateTime.Now:HH:mm:ss} The End. \r\n\n"); }
  }
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
