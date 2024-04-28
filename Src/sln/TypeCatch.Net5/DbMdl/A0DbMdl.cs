namespace TypeCatch.Net5.DbMdl;
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


  static readonly AzureSqlCredentials _asc;

  static A0DbMdl()
  {
    try
    {
#if StillInitializing
      const string _rgn = "azuresqluser", _key = "Everything is your friend...";
      if (_asc?.Usr == _rgn)
        return;
      JsonIsoFileSerializer.Save<AzureSqlCredentials>(new AzureSqlCredentials { Key = _key, Usr = _rgn });
#endif

      _asc = JsonIsoFileSerializer.Load<AzureSqlCredentials>() ?? throw new ArgumentNullException("@@@@@@@@@@@@@@@@");
    }
    catch (Exception ex) { ex.Log(); throw; }
  }

  public static A0DbMdl GetA0DbMdl => GetA0DbMdlExpress;
  public static A0DbMdl GetA0DbMdlAzureDb => new($"data source=sqs.database.windows.net;initial catalog=OneBase;persist security info=True;user id={_asc.Usr};password=\"{_asc.Key}\";MultipleActiveResultSets=True;App=EntityFramework");
  public static A0DbMdl GetA0DbMdlExpress => new(@"Data Source=.\SQLEXPRESS;initial catalog=OneBase;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
  public static A0DbMdl GetA0DbMdlExp_OLD => new(@"Data Source=.\SQLEXPRESS;initial catalog=TypeCatchDb;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
  public static A0DbMdl GetA0DbMdlLocalDb => new($@"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename={App.Dbfn};Integrated Security=True;Connect Timeout=15;");

  public static A0DbMdl GetInitDbx([CallerMemberName] string cmn = "", [CallerFilePath] string cfp = "", [CallerLineNumber] int cln = 0)
  {
    Bpr.Beep1of2();

    var sw = Stopwatch.StartNew();
    var db = GetA0DbMdl;

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

    Bpr.Beep1of2();
    var db = GetA0DbMdl;
    Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff}   took {sw.ElapsedMilliseconds:N0} ms."); sw.Restart();
    if (db.Database.Exists())
#if !NeedToWaitFor_4sec
      if (!db.Database.CompatibleWithModel(false))
        DbInitializer.SetMigraInitializer();
#endif
      else
        DbInitializer.SetDbInitializer();

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
/*
;lkj 
;lkj that is odd 
;lkj that is odd 88. Like i said`~
*/