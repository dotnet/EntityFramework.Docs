
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class InMemoryRequiredPropertiesSample
{
    public static void Required_properties_validated_with_in_memory_database()
    {
        Console.WriteLine($">>>> Sample: {nameof(Required_properties_validated_with_in_memory_database)}");
        Console.WriteLine();

        using var context = new UserContext();

        context.Add(new User());

        try
        {
            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
        }

        Console.WriteLine();
    }

    public static void Required_property_validation_with_in_memory_database_can_be_disabled()
    {
        Console.WriteLine($">>>> Sample: {nameof(Required_property_validation_with_in_memory_database_can_be_disabled)}");
        Console.WriteLine();

        using var context = new UserContextWithNullCheckingDisabled();

        context.Add(new User());

        context.SaveChanges();

        Console.WriteLine();
    }

    #region UserEntityType
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }
    }
    #endregion

    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .LogTo(Console.WriteLine, new[] { InMemoryEventId.ChangesSaved })
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase("UserContext");
        }
    }

    public class UserContextWithNullCheckingDisabled : DbContext
    {
        public DbSet<User> Users { get; set; }

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .LogTo(Console.WriteLine, new[] { InMemoryEventId.ChangesSaved })
                .UseInMemoryDatabase("UserContextWithNullCheckingDisabled")
                .EnableNullabilityCheck(false);
        }
        #endregion
    }
}
