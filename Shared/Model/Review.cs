using System;
using System.Collections.Generic;

namespace ReviewGobbler.Shared.Model
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public IList<AuthorReview> AuthorReviews { get; set; }
        public DateTime PublishedDate { get; set; }
        public int? Rating { get; set; } // normalized from 0 to 100
        public bool Recommended { get; set; }
        public int SiteId { get; set; }
        public Site Site { get; set; }
        public string SiteUrl { get; set; }
        public string SiteReviewId { get; set; }
        public string Summary { get; set; }
    }
}
