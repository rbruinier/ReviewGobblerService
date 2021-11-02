using System;
using System.Linq;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Managers;
using ReviewGobbler.Shared.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewGobbler.Shared.Scraping
{
    public class ReviewProcessor
    {
        private int _siteId;

        private DatabaseContext _databaseContext;

        private GenreManager _genreManager;
        private ArtistManager _artistManager;
        private LabelManager _labelManager;
        private AuthorManager _authorManager;
        private AlbumManager _albumManager;
        private ReviewManager _reviewManager;
        private SiteManager _siteManager;

        private IScraper _scraper;

        public ReviewProcessor(int siteId, IScraper scraper, DatabaseContext databaseContext)
        {
            _siteId = siteId;

            _scraper = scraper;
            _databaseContext = databaseContext;

            _genreManager = new GenreManager(_databaseContext);
            _artistManager = new ArtistManager(_databaseContext);
            _labelManager = new LabelManager(_databaseContext);
            _authorManager = new AuthorManager(_databaseContext);
            _albumManager = new AlbumManager(_databaseContext);
            _reviewManager = new ReviewManager(_databaseContext);
            _siteManager = new SiteManager(_databaseContext);
        }

        public async Task ProcessReviews()
        {
            _scraper.ReviewScraped = ReviewScraped;

            await _scraper.ScrapeReviewsTillCompleted();
        }

        internal void ReviewScraped(int siteId, ScrapedReview scrapedReview)
        {
            var site = _siteManager.FetchSiteById(_siteId);

            if (site == null)
            {
                throw new ApplicationException("Unknown site id.");
            }

            var genres = new List<Genre>();
            var artists = new List<Artist>();
            var labels = new List<Label>();
            var authors = new List<Author>();

            foreach (var scrapedGenre in scrapedReview.Album.Genres)
            {
                var genre = _genreManager.FetchOrInsertGenreByName(scrapedGenre);

                genres.Add(genre);
            }

            foreach (var scrapedArtist in scrapedReview.Album.Artists)
            {
                var artist = _artistManager.FetchOrInsertArtistByName(scrapedArtist.Name);

                artists.Add(artist);
            }

            foreach (var scrapedLabel in scrapedReview.Album.Labels)
            {
                var label = _labelManager.FetchOrInsertLabelByName(scrapedLabel.Name);

                labels.Add(label);
            }

            foreach (var scrapedAuthor in scrapedReview.Authors)
            {
                var author = _authorManager.FetchOrInsertAuthorByName(scrapedAuthor.Name);

                authors.Add(author);
            }

            var album = _albumManager.FetchAlbumByNameAndArtist(scrapedReview.Album.Name, artists[0]);

            if (album == null)
            {
                album = new Album
                {
                    Name = scrapedReview.Album.Name,
                    ReleaseYear = scrapedReview.Album.ReleaseYear,
                    ReleaseDate = scrapedReview.Album.ReleaseDate,
                    AlbumArtists = new List<AlbumArtist>(),
                    AlbumGenres = new List<AlbumGenre>(),
                    AlbumLabels = new List<AlbumLabel>(),
                    AlbumArtworks = new List<AlbumArtwork>()
                };

                foreach (var image in scrapedReview.Album.Images)
                {
                    album.AlbumArtworks.Add(new AlbumArtwork
                    {
                        Label = image.Label,
                        Url = image.Url,
                        Site = site
                    });
                }
            }

            foreach (var artist in artists)
            {
                if (album.AlbumArtists.FirstOrDefault(albumArtist => albumArtist.Artist.ArtistId == artist.ArtistId) != null)
                {
                    break;
                }

                var albumArtist = new AlbumArtist
                {
                    Album = album,
                    Artist = artist
                };

                album.AlbumArtists.Add(albumArtist);
            }

            foreach (var genre in genres)
            {
                if (album.AlbumGenres.FirstOrDefault(abumGenre => abumGenre.Genre.GenreId == genre.GenreId) != null)
                {
                    break;
                }

                var albumGenre = new AlbumGenre
                {
                    Album = album,
                    Genre = genre
                };

                album.AlbumGenres.Add(albumGenre);
            }

            foreach (var label in labels)
            {
                if (album.AlbumLabels.FirstOrDefault(albumLabel => albumLabel.Label.LabelId == label.LabelId) != null)
                {
                    break;
                }

                var albumLabel = new AlbumLabel
                {
                    Album = album,
                    Label = label
                };

                album.AlbumLabels.Add(albumLabel);
            }

            _albumManager.InsertOrUpdateAlbum(album);

            var existingReview = _reviewManager.FetchReviewBySiteReviewId(1, scrapedReview.SiteIdentifier, album.AlbumId);

            if (existingReview != null)
            {
                Console.WriteLine($"Review skipped at {scrapedReview.SiteUrl}; it is already scraped.");

                return;
            }

            Console.WriteLine($"Adding review at {scrapedReview.SiteUrl}.");

            var review = new Review
            {
                PublishedDate = scrapedReview.PublishedDate,
                Site = site,
                Album = album,
                Rating = scrapedReview.Rating,
                Recommended = scrapedReview.Recommended,
                SiteReviewId = scrapedReview.SiteIdentifier,
                SiteUrl = scrapedReview.SiteUrl,
                AuthorReviews = new List<AuthorReview>(),
                Summary = scrapedReview.Summary
            };

            foreach (var author in authors)
            {
                var authorReview = new AuthorReview
                {
                    Review = review,
                    Author = author
                };

                review.AuthorReviews.Add(authorReview);
            }

            _reviewManager.InsertReview(review);
        }
    }
}
