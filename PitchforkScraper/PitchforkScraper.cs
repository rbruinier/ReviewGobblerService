using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ReviewGobbler.Shared.Scraping;

namespace ReviewGobbler.PitchforkScraper
{
    public class RetryHandler : DelegatingHandler
    {
        private const int MaxRetries = 3;

        public RetryHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        { }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            for (int i = 0; i < MaxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }

            return response;
        }
    }

    public class Scraper : IScraper
    {
        private const int PageSize = 50;

        public ReviewScrapedDelegate ReviewScraped { get; set; }

        public int SiteId
        {
            get
            {
                return 1;
            }
        }

        /**
         * https://pitchfork.com/api/v2/search/?types=reviews&hierarchy=sections%2Freviews%2Falbums%2Cchannels%2Freviews%2Falbums&sort=publishdate%20desc%2Cposition%20asc&size=12&start=0
         */
        private const string UrlTemplate =
            "https://pitchfork.com/api/v2/search/?types=reviews&hierarchy=sections%2Freviews%2Falbums%2Cchannels%2Freviews%2Falbums&sort=publishdate%20desc%2Cposition%20asc&size=50&start={0}";

        public Scraper()
        {
        }

        public async Task ScrapeReviewsTillCompleted()
        {
            // only scrape first page. assume an automated scraping session once a day and only max 5 reviews are added per day
            for (int page = 0; page < 1; page++) {
                if (page >= 1)
                {
                    // we sleep a bit because pitchfork does not seem to appreciate multiple calls in a row
                    System.Threading.Thread.Sleep(5000);
                }

                Console.WriteLine($"Scraping page: {page}");

                await ScrapePage(page);
            }
        }

        private List<ScrapedReview> ConvertReview(Review review)
        {
            List<ScrapedReview> reviews = new List<ScrapedReview>();

            try
            {
                foreach (var sourceAlbum in review.Tombstone.Albums)
                {
                    try
                    {
                        var scrapedReview = new ScrapedReview();

                        if (Double.TryParse(sourceAlbum.Rating.Value, out double rating))
                        {
                            scrapedReview.Rating = (int)Math.Clamp(rating * 10.0, 0, 100);
                        }

                        scrapedReview.SiteIdentifier = review.Id;
                        scrapedReview.SiteUrl = review.Url;
                        scrapedReview.PublishedDate = review.PubDate;
                        scrapedReview.Recommended = sourceAlbum.Rating.Bnm || sourceAlbum.Rating.Bnr;
                        scrapedReview.Summary = review.SeoDescription;

                        foreach (var author in review.Authors)
                        {
                            scrapedReview.Authors.Add(new ScrapedAuthor(author.Id, author.Name, author.Url));
                        }

                        var scrapedAlbum = new ScrapedAlbum();

                        scrapedAlbum.Name = sanitizeName(sourceAlbum.Album.DisplayName);
                        scrapedAlbum.ReleaseDate = review.PubDate;
                        scrapedAlbum.ReleaseYear = sourceAlbum.Album.ReleaseYear;

                        foreach (var genre in review.Genres)
                        {
                            scrapedAlbum.Genres.Add(genre.DisplayName);
                        }

                        foreach (var artist in sourceAlbum.Album.Artists)
                        {
                            scrapedAlbum.Artists.Add(new ScrapedArtist(artist.Id, artist.DisplayName, artist.Url));
                        }

                        // in case there are no artist entries it is assumed to be a "various" artists album
                        if (scrapedAlbum.Artists.Count == 0)
                        {
                            scrapedAlbum.Artists.Add(new ScrapedArtist("0", "Various", ""));
                        }

                        foreach (var label in sourceAlbum.Album.Labels)
                        {
                            scrapedAlbum.Labels.Add(new ScrapedLabel(label.Id, label.DisplayName));
                        }

                        scrapedAlbum.Images.Add(new ScrapedImageDetails("standard", sourceAlbum.Album.Photos.Tout.Sizes.Standard));
                        scrapedAlbum.Images.Add(new ScrapedImageDetails("list", sourceAlbum.Album.Photos.Tout.Sizes.List));
                        scrapedAlbum.Images.Add(new ScrapedImageDetails("homepageSmall", sourceAlbum.Album.Photos.Tout.Sizes.HomepageSmall));
                        scrapedAlbum.Images.Add(new ScrapedImageDetails("homepageLarge", sourceAlbum.Album.Photos.Tout.Sizes.HomepageLarge));

                        scrapedReview.Album = scrapedAlbum;

                        reviews.Add(scrapedReview);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to scrape album: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to scrape review: {e.Message}");
            }

            return reviews;
        }

        private string sanitizeName(string name)
        {
            var result = name;

            if (result.EndsWith(" EP"))
            {
                result = result.Replace(" EP", "");
            }

            return result;
        }

        private async Task ScrapePage(int pageIndex)
        {
            HttpClient httpClient = new HttpClient(new RetryHandler(new HttpClientHandler()));

            var startIndex = pageIndex * PageSize;

            var url = String.Format(UrlTemplate, startIndex);

            Console.WriteLine($"Scraping reviews at {url}");

            var rawJsonAsString = await httpClient.GetStringAsync(url);

            var deserializeOptions = new JsonSerializerOptions();

            deserializeOptions.PropertyNameCaseInsensitive = true;
            deserializeOptions.AllowTrailingCommas = true;

            var response = JsonSerializer.Deserialize<Response>(rawJsonAsString, deserializeOptions);

            Console.WriteLine(response);

            foreach (var sourceReview in response.Results.List)
            {
                var scrapedReviews = ConvertReview(sourceReview);

                foreach (var scrapedReview in scrapedReviews)
                {
                    ReviewScraped(1, scrapedReview);
                }
            }
        }
    }

    public class PhotosSizes
    {
        public string List { get; set; }
        public string Standard { get; set; }
        public string HomepageSmall { get; set; }
        public string HomepageLarge { get; set; }
    }

    public class PhotosTout
    {
        public PhotosSizes Sizes { get; set; }
    }

    public class Photos
    {
        public PhotosTout Tout { get; set; }
    }

    public class Rating
    {
        [JsonPropertyName("rating")]
        public string Value { get; set; }

        [JsonPropertyName("display_rating")]
        public string DisplayRating { get; set; }
        public bool Bnm { get; set; }
        public bool Bnr { get; set; }
    }

    public class Artist
    {
        public string Id { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        public string Url { get; set; }
    }

    public class Label
    {
        public string Id { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
    }

    public class Album
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("release_year")]
        public int? ReleaseYear { get; set; }
        public List<Label> Labels { get; set; }

        public List<Artist> Artists { get; set; }

        public Photos Photos { get; set; }
    }

    public class AlbumContainer
    {
        public string Id { get; set; }
        public Album Album { get; set; }
        public Rating Rating { get; set; }
    }

    public class Tombstone
    {
        public bool Bnm { get; set; }
        public bool Bnr { get; set; }
        public List<AlbumContainer> Albums { get; set; }

    }
    public class Genre
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
    }

    public class Author
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class Review
    {
        public DateTime PubDate { get; set; }
        public string SocialDescription { get; set; }
        public string SeoDescription { get; set; }
        public List<Author> Authors { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public Tombstone Tombstone { get; set; }
        public List<Genre> Genres { get; set; }
    }

    public class Results
    {
        public List<Review> List { get; set; }
    }

    public class Response
    {
        public Results Results { get; set; }
    }
}
