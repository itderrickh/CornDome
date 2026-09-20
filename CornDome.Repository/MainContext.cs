using CornDome.Models;
using CornDome.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository
{
    public class MainContext(DbContextOptions<MainContext> options) : DbContext(options)
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<FeedbackRequest> CardFeedbacks { get; set; }
        public DbSet<DiscordConnection> DiscordConnections { get; set; }
        public DbSet<PlayAvailability> PlayAvailabilities { get; set; }
        public DbSet<PlayPreferences> PlayPreferences { get; set; }
        public DbSet<BugReport> BugReports { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<DeckComment> DeckComments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Directly configure SQLite connection string here
                optionsBuilder.UseSqlite("Data Source=main_database.db");  // Use SQLite connection string
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>();
            modelBuilder.Entity<Role>();
            modelBuilder.Entity<FeedbackRequest>();
            modelBuilder.Entity<BugReport>();
            modelBuilder.Entity<LogEntry>();

            modelBuilder.Entity<PlayAvailability>()
                .HasOne(c => c.User)
                .WithMany(u => u.PlayAvailabilities)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscordConnection>()
                .HasOne(c => c.User)
                .WithMany(u => u.DiscordConnections)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(c => c.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(c => c.Role)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(c => c.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Deck>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(d => d.DeckComments)
                    .WithOne(c => c.Deck)
                    .HasForeignKey(c => c.DeckId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DeckComment>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.CommentText)
                    .HasMaxLength(400);
                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.Deck)
                    .WithMany(d => d.DeckComments)
                    .HasForeignKey(c => c.DeckId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
