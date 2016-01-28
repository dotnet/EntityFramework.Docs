using Microsoft.Data.Entity;

namespace EFSaving.ExplicitValuesGenerateProperties
{
    public class EmployeeContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFSaving.ExplicitValuesGenerateProperties;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(b => b.EmploymentStarted)
                .HasDefaultValueSql("CONVERT(date, GETDATE())");
        }
    }
}
