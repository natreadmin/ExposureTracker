

using ExposureTracker.Models;

namespace ExposureTracker.Data
{
    public class AppDbContext: DbContext
    {

        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Insured>().HasKey(table => new
            {
                table.Identifier,table.FullNameDOB,


            });
        }

        public DbSet<Insured> dbLifeData  { get; set; }

        public DbSet<fullnamedob> Fullnamedobs { get; set; }

    }

 
   



}
