using AAV.Sys.Ext;
using AsLink;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using TypingWpf.DbMdl;

namespace TypingWpf.DbExt
{
  public class DbMigrationsInitializer : MigrateDatabaseToLatestVersion<A0DbMdl, DbMigrationsConfiguration>
  {
    async public override void InitializeDatabase(A0DbMdl db)
    {
      try
      {
        var migrator = new DbMigrator(new DbMigrationsConfiguration());
        migrator.Update();

        var now = DateTime.Now;

        foreach (var ur in db.Users) ur.ModifiedAt = now; await db.TrySaveReportAsync();
      }
      catch (Exception ex) { ex.Log(); throw; }
    }
  }

  public class DbMigrationsConfiguration : DbMigrationsConfiguration<A0DbMdl>
  {
    public DbMigrationsConfiguration()
    {
      AutomaticMigrationsEnabled = true;
      AutomaticMigrationDataLossAllowed = true;
    }
    protected override void Seed(A0DbMdl db)
    {
      try
      {
        /// called upon a migration
        /// add additional seeding if needed... 
        /// ...although MigrateDatabaseToLatestVersion.InitializeDatabase seems OK too.
        /// db.TrySaveReportAsync();
      }
      catch (Exception ex) { ex.Log(); throw; }
    }
  }
}
