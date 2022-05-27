

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
            builder.Entity<Insured>().HasKey(table => new {
                table.Id, table.PolicyNumber
               
            });
        }

        public DbSet<Insured> dbInsured  { get; set; }

        public DbSet<PolicyNo> dbPolicy { get; set; }

    }

 
   



}
