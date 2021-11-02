using Microsoft.EntityFrameworkCore;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Shared.DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<AlbumArtwork> AlbumArtworks { get; set; }

        public DbSet<AlbumArtist> AlbumArtists { get; set; }
        public DbSet<AlbumGenre> AlbumGenres { get; set; }
        public DbSet<AlbumLabel> AlbumLabels { get; set; }

        public DbSet<AuthorReview> AuthorReviews { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<AlbumArtist>()
                .HasKey(sc => new { sc.AlbumId, sc.ArtistId });

            modelBuilder
                .Entity<AlbumGenre>()
                .HasKey(sc => new { sc.AlbumId, sc.GenreId });

            modelBuilder
                .Entity<Album>()
                .HasMany(album => album.Reviews)
                .WithOne(review => review.Album);

            modelBuilder
                .Entity<Album>()
                .HasMany(album => album.AlbumArtworks)
                .WithOne(albumArtwork => albumArtwork.Album);

            modelBuilder
                .Entity<AlbumArtwork>()
                .HasOne(albumArtwork => albumArtwork.Site);

            modelBuilder.Entity<Site>()
                 .HasMany(site => site.Reviews)
                 .WithOne(review => review.Site);

            modelBuilder
                .Entity<AlbumLabel>()
                .HasKey(sc => new { sc.AlbumId, sc.LabelId });

            modelBuilder
                .Entity<AuthorReview>()
                .HasKey(sc => new { sc.AuthorId, sc.ReviewId });


            modelBuilder
                .Entity<Site>()
                .HasData(
                    new Site
                    {
                        SiteId = 1,
                        Name = "Pitchfork"
                    },
                    new Site
                    {
                        SiteId = 2,
                        Name = "Resident Advisor"
                    },
                    new Site
                    {
                        SiteId = 3,
                        Name = "Tiny Mix Tapes"
                    }
                );
        }
    }
}
