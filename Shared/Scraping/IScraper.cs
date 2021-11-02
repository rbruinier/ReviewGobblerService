using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewGobbler.Shared.Scraping
{
    public delegate void ReviewScrapedDelegate(int siteId, ScrapedReview scrapedReview);

    public class ScrapedImageDetails
    {
        public string Label { get; set; }
        public string Url { get; set; }

        public ScrapedImageDetails(string label, string url)
        {
            Label = label;
            Url = url;
        }
    }

    public class ScrapedArtist
    {
        public string SiteId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }

        public ScrapedArtist(string siteId, string name, string url)
        {
            SiteId = siteId;
            Name = name;
            Url = url;
        }
    }

    public class ScrapedAuthor
    {
        public string SiteId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }

        public ScrapedAuthor(string siteId, string name, string url)
        {
            SiteId = siteId;
            Name = name;
            Url = url;
        }
    }

    public class ScrapedLabel
    {
        public string SiteId { get; set; }
        public string Name { get; set; }

        public ScrapedLabel(string siteId, string name)
        {
            SiteId = siteId;
            Name = name;
        }
    }

    public class ScrapedAlbum
    {
        public DateTime ReleaseDate;

        public int? ReleaseYear;

        public string Name { get; set; }

        public List<ScrapedArtist> Artists { get; set; }
        public List<string> Genres { get; set; }
        public List<ScrapedLabel> Labels { get; set; }
        public List<ScrapedImageDetails> Images { get; set; }

        public ScrapedAlbum()
        {
            Artists = new List<ScrapedArtist>();
            Genres = new List<string>();
            Labels = new List<ScrapedLabel>();
            Images = new List<ScrapedImageDetails>();
        }
    }

    public class ScrapedReview
    {
        public ScrapedAlbum Album { get; set; }

        public DateTime PublishedDate;

        public bool Recommended { get; set; }

        public int? Rating { get; set; }

        public List<ScrapedAuthor> Authors { get; set; }

        public string SiteIdentifier { get; set; }
        public string SiteUrl { get; set; }

        public string Summary { get; set; }

        public ScrapedReview()
        {
            Authors = new List<ScrapedAuthor>();
        }
    }

    public interface IScraper
    {
        int SiteId { get; }

        ReviewScrapedDelegate ReviewScraped { get; set; }

        Task ScrapeReviewsTillCompleted();
    }
}
