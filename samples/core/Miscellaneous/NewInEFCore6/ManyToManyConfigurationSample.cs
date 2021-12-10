﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public static class ManyToManyConfigurationSample
{
    public static void Many_to_many_relationships_may_need_less_configuration()
    {
        Console.WriteLine($">>>> Sample: {nameof(Many_to_many_relationships_may_need_less_configuration)}");
        Console.WriteLine();

        ManyToManyTest<NoConfigContext>();
        ManyToManyTest<JustNavigationsContext>();
        ManyToManyTest<SpecifySharedEntityTypeContext>();
        ManyToManyTest<SpecifyEntityTypeContext>();
        // ManyToManyTest<SpecifyEntityTypeAndKeyContext>();
        ManyToManyTest<SpecifyEntityTypeAndFksContext>();
    }

    public static void ManyToManyTest<TContext>()
        where TContext : BaseContext, new()
    {
        Console.WriteLine($"{typeof(TContext).Name}: ");
        Console.WriteLine();

        using (var context = new TContext())
        {
            context.Database.EnsureDeleted();

            Console.WriteLine(context.Model.ToDebugString());
            Console.WriteLine();
            
            context.Log = true;
            context.Database.EnsureCreated();
            context.Log = false;

            var cats = new List<Cat>
            {
                new() { Name = "Alice" },
                new() { Name = "Mac" }
            };

            context.AddRange(new List<Human>
            {
                new() { Name = "Wendy", Cats = { cats[0], cats[1] } },
                new() { Name = "Arthur", Cats = { cats[0], cats[1] } }
            });

            context.SaveChanges();
        }
        
        Console.WriteLine();

        using (var context = new TContext())
        {
            var cats = context.Cats.Include(e => e.Humans).ToList();
            foreach (var cat in cats)
            {
                Console.WriteLine($"{cat.Name} has {cat.Humans.Count} humans.");
            }
        }
        Console.WriteLine();
    }

    public class Cat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Human> Humans { get; } = new List<Human>();
    }

    public class Human
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Cat> Cats { get; } = new List<Cat>();
    }
    
    public class CatHuman
    {
        public int CatsId { get; set; }
        public int HumansId { get; set; }
        //
        // public Cat Cat { get; set; }
        // public Human Human { get; set; }
    }

    public class NoConfigContext : BaseContext
    {
    }

    public class JustNavigationsContext : BaseContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region JustNavigation
            modelBuilder.Entity<Cat>()
                .HasMany(e => e.Humans)
                .WithMany(e => e.Cats);
            #endregion
        }
    }

    public class SpecifySharedEntityTypeContext : BaseContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cat>()
                .HasMany(e => e.Humans)
                .WithMany(e => e.Cats)
                .UsingEntity<Dictionary<string, object>>();
        }
    }

    public class SpecifyEntityTypeContext : BaseContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region SpecifyEntityType
            modelBuilder.Entity<Cat>()
                .HasMany(e => e.Humans)
                .WithMany(e => e.Cats)
                .UsingEntity<CatHuman>();
            #endregion
        }
    }

    public class SpecifyEntityTypeAndKeyContext : BaseContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region SpecifyEntityTypeAndKey
            modelBuilder.Entity<Cat>()
                .HasMany(e => e.Humans)
                .WithMany(e => e.Cats)
                .UsingEntity<CatHuman>(
                    e => e.HasKey(e => new { e.CatsId, e.HumansId }));
            #endregion
        }
    }

    public class SpecifyEntityTypeAndFksContext : BaseContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region SpecifyEntityTypeAndFks
            modelBuilder.Entity<Cat>()
                .HasMany(e => e.Humans)
                .WithMany(e => e.Cats)
                .UsingEntity<CatHuman>(
                    e => e.HasOne<Human>().WithMany().HasForeignKey(e => e.CatsId),
                    e => e.HasOne<Cat>().WithMany().HasForeignKey(e => e.HumansId),
                    e => e.HasKey(e => new { e.CatsId, e.HumansId }));
            #endregion
        }
    }

    public abstract class BaseContext : DbContext
    {
        public bool Log { get; set; }

        public DbSet<Cat> Cats { get; set; }
        public DbSet<Human> Humans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFCoreSample");

            optionsBuilder.LogTo(
                s =>
                    {
                        if (Log)
                        {
                            Console.WriteLine(s);
                        }
                    },
                new[] { RelationalEventId.CommandExecuted });
        }
    }
}
