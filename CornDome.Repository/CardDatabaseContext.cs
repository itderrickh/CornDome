using Microsoft.EntityFrameworkCore;
using CornDome.Models.Cards;

namespace CornDome.Repository
{
    public class CardDatabaseContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardRevision> CardRevisions { get; set; }
        public DbSet<CardImage> CardImages { get; set; }
        public DbSet<CardImageType> CardImageTypes { get; set; }
        public DbSet<CardSet> CardSets { get; set; }
        public DbSet<Landscape> Landscapes { get; set; }
        public DbSet<CardType> CardTypes { get; set; }

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

            // Configure the relationship between Card and CardRevision
            modelBuilder.Entity<Card>()
                .HasMany(c => c.Revisions)
                .WithOne(cr => cr.Card)
                .HasForeignKey(cr => cr.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between CardRevision and CardImage
            modelBuilder.Entity<CardRevision>()
                .HasMany(cr => cr.CardImages)
                .WithOne(ci => ci.Revision)
                .HasForeignKey(ci => ci.RevisionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CardRevision>()
                .HasOne(cr => cr.CardSet)
                .WithMany()
                .HasForeignKey(cr => cr.SetId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CardRevision>()
                .HasOne(cr => cr.Landscape)
                .WithMany()
                .HasForeignKey(cr => cr.LandscapeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CardRevision>()
                .HasOne(cr => cr.CardType)
                .WithMany()
                .HasForeignKey(cr => cr.TypeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CardRevision>()
                .HasMany(cr => cr.CardImages)
                .WithOne(ci => ci.Revision)
                .HasForeignKey(ci => ci.RevisionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
