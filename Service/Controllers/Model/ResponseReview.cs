using System;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Service.Controllers.Model
{
    public class ResponseReview
    {
        public int Id { get; set; }
        public int PublishedDate { get; set; }
        public int? Rating { get; set; } // normalized from 0 to 100
        public bool Recommended { get; set; }
        public int SiteId { get; set; }
        public string ReviewUrl { get; set; }
        public string Summary { get; set; }

        public ResponseReview(Review review)
        {
            Id = review.ReviewId;
            PublishedDate = (int)((DateTimeOffset)review.PublishedDate).ToUnixTimeSeconds();
            Rating = review.Rating;
            Recommended = review.Recommended;
            SiteId = review.SiteId;
            Summary = review.Summary;

            if (review.SiteId == 1)
            {
                ReviewUrl = "https://pitchfork.com" + review.SiteUrl;
            }
        }
    }
}
