using Microsoft.EntityFrameworkCore;
using SmartElectionCertificationCentre.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionCertificationCentre.Data
{
    public class PrivateContext : DbContext
    {
        public DbSet<PrivateKey> PrivateKeys { get; set; }

        public PrivateContext(DbContextOptions<PrivateContext> options) : base(options) {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Privacy;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PrivateKey>().HasKey(x => x.CertificateThumbprint);
        }
    }
}
