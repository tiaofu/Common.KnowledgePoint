

using System.Data.Entity;

namespace QuartzJob.Contexts
{
    public class DbContext<Entity>: EntitiesContext where Entity : class
    {
        public DbContext(string connection)
            :base(connection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(User.ToUpper());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Entity> Models { get; set; }
    }
}
