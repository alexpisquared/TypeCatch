namespace TypingWpf.DbExt
{
  public class DbInitializer : DropCreateDatabaseIfModelChanges<A0DbMdl>
  {
    public bool IsDbReady { get; private set; }
    public static void SetMigraInitializer() => Database.SetInitializer(new DbMigrationsInitializer());
    public static void SetDbInitializer()
    {
      try
      {
        Database.SetInitializer(new CreateDatabaseIfNotExists<A0DbMdl>());
        Database.SetInitializer(new DropCreateDatabaseIfModelChanges<A0DbMdl>());
        Database.SetInitializer(new DbInitializer());
      }
      catch (Exception ex) { ex.Log(); throw; }
    }

    async protected override void Seed(A0DbMdl db)
    {
      base.Seed(db);

      try
      {
        var now = DateTime.Now;

        db.Users.Add(new User { UserId = "Plr1", FullName = "", Note = "Prepopulated", CreatedAt = now });
        db.Users.Add(new User { UserId = "Plr2", FullName = "", Note = "Prepopulated", CreatedAt = now });
        db.Users.Add(new User { UserId = "Plr3", FullName = "", Note = "Prepopulated", CreatedAt = now });
        db.Users.Add(new User { UserId = "Plr4", FullName = "", Note = "Prepopulated", CreatedAt = now });

        db.AppStngs.Add(new AppStng { LesnTyp = (int)LessonType.PhrasesRandm, SubLesnId = "0", CreatedAt = DateTime.Now, Id = 1, UserId = "Plr1", Note = "Auto pre-loaded." });

        await db.SaveChangesAsync(); // .TrySaveReportAsync();
      }
      catch (Exception ex) { var v = ex.Log(); throw; }
    }
  }
}


