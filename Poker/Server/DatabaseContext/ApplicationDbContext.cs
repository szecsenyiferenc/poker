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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Server=DESKTOP-DFI3P6Q;Database=Poker;Trusted_Connection=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PokerUserModel>().HasKey(p => p.Id);
            modelBuilder.Entity<PokerUserModel>().HasIndex(p => p.Username).IsUnique();
        }
    }
}
