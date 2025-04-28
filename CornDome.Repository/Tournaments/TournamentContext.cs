using CornDome.Models.Tournaments;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository.Tournaments
{
    public class TournamentContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<TournamentRegistration> Registrations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Directly configure SQLite connection string here
                optionsBuilder.UseSqlite("Data Source=tournament_database.db");  // Use SQLite connection string
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var tournament = modelBuilder.Entity<Tournament>();
            var round = modelBuilder.Entity<Round>();
            var match = modelBuilder.Entity<Match>();
            var tournamentRegistration = modelBuilder.Entity<TournamentRegistration>();

            tournament
                .HasMany(c => c.Rounds)
                .WithOne(cr => cr.Tournament)
                .HasForeignKey(cr => cr.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            tournament
                .HasMany(tr => tr.Registrations)
                .WithOne(tr => tr.Tournament)
                .HasForeignKey(tr => tr.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            round
                .HasMany(cr => cr.Matches)
                .WithOne(ci => ci.Round)
                .HasForeignKey(ci => ci.RoundId)
                .OnDelete(DeleteBehavior.Cascade);

            match
                .HasOne(cr => cr.Tournament)
                .WithMany()
                .HasForeignKey(cr => cr.TournamentId)
                .OnDelete(DeleteBehavior.NoAction);

            tournamentRegistration
                .Ignore(tr => tr.User);
            tournamentRegistration
                .HasOne(tr => tr.Tournament)
                .WithMany(t => t.Registrations)
                .HasForeignKey(tr => tr.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
