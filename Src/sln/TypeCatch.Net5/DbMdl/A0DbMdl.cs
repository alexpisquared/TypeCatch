namespace TypingWpf.DbMdl
{
  using AAV.Sys.Ext;
  using AAV.Sys.Helpers;
  using System;
  using System.Data.Entity;
  using System.Diagnostics;
  using System.Runtime.CompilerServices;
  using TypeCatch.Net5;
  using TypeCatch.Net5.Properties;
  using TypingWpf.DbExt;

  public partial class A0DbMdl : DbContext
  {
    #region Ext
    A0DbMdl(string cs)
    {
      Database.Connection.ConnectionString = cs;

      /// according to SqlLocalDb.exe:
      /// MSSQLLocalDB == ProjectsV12 - on ASUS2 which does not let 
      /// MSSQLLocalDB == ProjectsV14 - on VAIO which does not let 
      /// using MSSQLLocalDB - does not let WK files to be read at HO ==>
      /// ==> using ProjectsV13 seems OK.
      /// 
      /// 
      /// 
      /*
      C:\>sqllocaldb stop   MSSQLLocalDB   ...   LocalDB instance "MSSQLLocalDB" stopped.
      C:\>sqllocaldb delete MSSQLLocalDB   ...   LocalDB instance "MSSQLLocalDB" deleted.
      C:\>sqllocaldb create MSSQLLocalDB   ...   LocalDB instance "MSSQLLocalDB" created with version 13.0.1601.5.
      C:\>sqllocaldb start  MSSQLLocalDB   ...   LocalDB instance "MSSQLLocalDB" started.                */

      /// this works too but SQLEXPRESS is misleading:
      /// $@"Data Source=.\SQLEXPRESS;AttachDbFilename={dbfn};Integrated Security=True;Connect Timeout=10;User Instance=True;";
      /// todo:
      ///     install local db on asus2 off the installer on a onedrive
      ///     and/or
      ///     run a setup program for TypeCatch with the prerequisite kicking in and installing sql db 2014.

      Trace.WriteLineIf(ExnLogr.AppTraceLevelCfg.TraceVerbose, $"::>{Database.Connection.ConnectionString}");
      //..Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff}   cTor {swUnitTest.ElapsedMilliseconds:N0} ms.");
    }


    const string _rgn = "azuresqluser", _key = "use proper key here";
    static readonly AzureSqlCredentials _asc;

    static A0DbMdl()
    {
      try
      {
        _asc = JsonIsoFileSerializer.Load<AzureSqlCredentials>();

#if StillInitializing
        if (_asc?.Usr == _rgn)
          return;
        JsonIsoFileSerializer.Save<AzureSqlCredentials>(new AzureSqlCredentials { Key = _key, Usr = _rgn });
        _asc = JsonIsoFileSerializer.Load<AzureSqlCredentials>();
#endif
      }
      catch (Exception ex) { ex.Log(); }
    }

    public static A0DbMdl GetA0DbMdlAzureDb => new A0DbMdl($"data source=sqs.database.windows.net;initial catalog=OneBase;persist security info=True;user id={_asc.Usr};password=\"{_asc.Key}\";MultipleActiveResultSets=True;App=EntityFramework");
    public static A0DbMdl GetA0DbMdlExpress => new A0DbMdl(@"Data Source=.\SQLEXPRESS;initial catalog=TypeCatchDb;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
    public static A0DbMdl GetA0DbMdlLocalDb => new A0DbMdl($@"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename={App.Dbfn};Integrated Security=True;Connect Timeout=15;");

    public static A0DbMdl GetInitDbx([CallerMemberName] string cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0)
    {
      Bpr.BeepShort();

      var sw = Stopwatch.StartNew();
      var db = GetA0DbMdlAzureDb;

      Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff}   took {sw.ElapsedMilliseconds:N0} ms."); sw.Restart();

      if (!db.Database.Exists())
        DbInitializer.SetDbInitializer();

      Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff}   took {sw.ElapsedMilliseconds:N0} ms.");

      return db;
    }
    public static A0DbMdl GetMigrate([CallerMemberName] string cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0)
    {
      var sw = Stopwatch.StartNew();
      //Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff} GetCreateA0DbMdl: called from {cfp}({cln}):\t{cmn}()");

      Bpr.BeepShort();
      var db = GetA0DbMdlAzureDb;
      Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff}   took {sw.ElapsedMilliseconds:N0} ms."); sw.Restart();
      if (db.Database.Exists())
      {
#if !NeedToWaitFor_4sec
        if (!db.Database.CompatibleWithModel(false))
        {
          DbInitializer.SetMigraInitializer();
        }
#endif
      }
      else
      {
        DbInitializer.SetDbInitializer();
      }

      Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff}   took {sw.ElapsedMilliseconds:N0} ms.");
      return db;
    }
    #endregion

    public virtual DbSet<AppStng> AppStngs { get; set; }
    public virtual DbSet<SessionResult> SessionResults { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Entity<AppStng>()
          .Property(e => e.Note)
          .IsUnicode(false);

      modelBuilder.Entity<SessionResult>()
          .Property(e => e.Note)
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .Property(e => e.Note)
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .HasMany(e => e.SessionResults)
          .WithRequired(e => e.User)
          .WillCascadeOnDelete(false);
    }
  }
}
/*
;lkj 
;lkj that is odd 
;lkj that is odd 88. Like i said`~
*/