using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EFSaving.ExplicitValuesGenerateProperties
{
    public class EmployeeContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFSaving.ExplicitValuesGenerateProperties;Trusted_Connection=True;ConnectRetryCount=0");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region EmploymentStarted
            modelBuilder.Entity<Employee>()
                .Property(b => b.EmploymentStarted)
                .HasDefaultValueSql("CONVERT(date, GETDATE())");
            #endregion

            #region LastPayRaise
            modelBuilder.Entity<Employee>()
                .Property(b => b.LastPayRaise)
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Employee>()
                .Property(b => b.LastPayRaise)
                .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            #endregion
        }
    }
}
