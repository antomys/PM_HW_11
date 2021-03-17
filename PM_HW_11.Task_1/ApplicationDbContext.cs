using System;
using DepsWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DepsWebApp
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Accounts table
        /// </summary>
        public DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
        }

        /// <summary>
        /// On configuring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres");
            optionsBuilder.LogTo(Console.WriteLine);
            //base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// On model creation
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("Accounts");
            //base.OnModelCreating(modelBuilder);
        }
    }
}