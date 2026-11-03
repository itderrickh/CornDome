using Microsoft.EntityFrameworkCore;
using CornDome.Models.Cards;

namespace CornDome.Repository
{
    public class CardDatabaseContext(DbContextOptions<CardDatabaseContext> options) : DbContext(options)
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardImage> CardImages { get; set; }
        public DbSet<CardImageType> CardImageTypes { get; set; }
        public DbSet<CardSet> CardSets { get; set; }
        public DbSet<Landscape> Landscapes { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<DarklandsCard> DarklandsCards { get; set; }
        public DbSet<DarklandsCardType> DarklandsCardTypes { get; set; }
        public DbSet<DarklandsOverlord> DarklandsOverlords { get; set; }
        public DbSet<DarklandsCardImage> DarklandsCardImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Directly configure SQLite connection string here
                optionsBuilder.UseSqlite("Data Source=carddatabase.db");  // Use SQLite connection string
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Card>()
                .HasMany(c => c.CardImages)
                .WithOne(ci => ci.Card)
                .HasForeignKey(ci => ci.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Card>()
                .HasMany(c => c.CardSets)
                .WithMany(s => s.Cards)
                .UsingEntity(j => j.ToTable("cardSet"));

            modelBuilder.Entity<Card>()
                .HasOne(c => c.Landscape)
                .WithMany()
                .HasForeignKey(c => c.LandscapeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Card>()
                .HasOne(c => c.CardType)
                .WithMany()
                .HasForeignKey(c => c.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DarklandsCard>(entity =>
            {
                entity.HasOne(x => x.CardType)
                    .WithMany()
                    .HasForeignKey(x => x.CardTypeId)
                    .IsRequired();

                entity.HasOne(x => x.Overlord)
                    .WithMany()
                    .HasForeignKey(x => x.OverlordId)
                    .IsRequired(false);

            });

            modelBuilder.Entity<DarklandsCardImage>(entity =>
            {
                entity.HasOne(x => x.CardImageType)
                    .WithMany()
                    .HasForeignKey(x => x.CardImageTypeId);

                entity.HasOne(x => x.Card)
                    .WithMany(x => x.Images)
                    .HasForeignKey(x => x.CardId);
            });
        }
    }
}
