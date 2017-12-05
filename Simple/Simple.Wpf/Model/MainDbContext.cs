using System;
using System.Data.Entity;

namespace Simple.Wpf.Model
{
    public class MainDbContext : DbContext
    {
        public DbSet<Entity> Entities { get; set; }

        public MainDbContext()
            : this(false)
        {
        }

        public MainDbContext(bool enabledProxyCreation)
        {
            Database.SetInitializer(new MainDbInitializer());
            Configuration.ProxyCreationEnabled = enabledProxyCreation;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Entity.ConfigureDbModel(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private class MainDbInitializer : IDatabaseInitializer<MainDbContext> 
        {
            public void InitializeDatabase(MainDbContext context)
            {
                CreateTablesIfNotExists(context);
            }

            private void CreateTablesIfNotExists(MainDbContext context)
            {
                context.Database.ExecuteSqlCommand(@"
CREATE TABLE IF NOT EXISTS `Entities` (
    `Id`   INTEGER PRIMARY KEY ASC AUTOINCREMENT,
    `Name` TEXT NOT NULL
)");
            }
        }
    }
}