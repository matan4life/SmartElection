using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartElectionBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionBackend.Data
{
    public class SmartElectionContext : IdentityDbContext<User>
    {
        public SmartElectionContext(DbContextOptions<SmartElectionContext> options)
            : base(options)
        {
        }

        public DbSet<Alternative> Alternatives { get; set; }
        public DbSet<Ballot> Ballots { get; set; }
        public DbSet<CommiteeAgreement> CommiteeAgreements { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<ElectoralCommitee> ElectoralCommitees { get; set; }
        public DbSet<IoT> IoTs { get; set; }
        public DbSet<Turnout> Turnouts { get; set; }
        public DbSet<UserCertificates> UserCertificates { get; set; }
        public DbSet<UserFingers> UserFingers { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SmartElection;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CommiteeAgreement>().HasKey(x => new { x.ElectionId, x.ElectoralCommiteeId });
            builder.Entity<CommiteeAgreement>().
                HasOne(x => x.Election).
                WithMany(y => y.CommiteeAgreements)
                .HasForeignKey(x => x.ElectionId);
            builder.Entity<CommiteeAgreement>()
                .HasOne(x => x.ElectoralCommitee)
                .WithMany(y => y.CommiteeAgreements)
                .HasForeignKey(x => x.ElectoralCommiteeId);
            builder.Entity<Turnout>().HasKey(x => new { x.ElectionId, x.UserId, x.ElectoralCommiteeId });
            builder.Entity<Turnout>()
                .HasOne(x => x.User)
                .WithMany(y => y.Turnouts)
                .HasForeignKey(x => x.UserId);
            builder.Entity<Turnout>()
                .HasOne(x => x.Election)
                .WithMany(y => y.Turnouts)
                .HasForeignKey(x => x.ElectionId);
            builder.Entity<Turnout>()
                .HasOne(x => x.ElectoralCommitee)
                .WithMany(y => y.Turnouts)
                .HasForeignKey(x => x.ElectoralCommiteeId);
            builder.Entity<UserFingers>().HasKey(x => new { x.IoTId, x.UserId });
            builder.Entity<UserFingers>()
                .HasOne(x => x.User)
                .WithMany(y => y.UserFingers)
                .HasForeignKey(x => x.UserId);
            builder.Entity<UserFingers>()
                .HasOne(x => x.IoT)
                .WithMany(y => y.UserFingers)
                .HasForeignKey(x => x.IoTId);
            base.OnModelCreating(builder);
        }
    }
}
