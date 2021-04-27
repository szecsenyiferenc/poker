using Microsoft.EntityFrameworkCore;
using Poker.Shared.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<PokerUserModel> PokerUsers { get; set; }
        private readonly string _connectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(_connectionString);

        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PokerUserModel>().HasKey(p => p.Id);
            modelBuilder.Entity<PokerUserModel>().HasIndex(p => p.Username).IsUnique();
        }
    }
}
