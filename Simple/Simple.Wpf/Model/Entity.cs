using System.Data.Entity;

namespace Simple.Wpf.Model
{
    public class Entity
    {
        public static void ConfigureDbModel(DbModelBuilder builder)
        {
            builder.Entity<Entity>().HasKey(e => e.Id);
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}