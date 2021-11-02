using System;
using System.Collections.Generic;

namespace ReviewGobbler.Service.Controllers.Request
{
    public class ReviewFilter
    {
        public int? MinimumRating { get; set; }
        public int? MaximumRating { get; set; }

        public List<int> GenreIds { get; set; }
        public List<int> SiteIds { get; set; }

        public ReviewFilter()
        {
        }
    }
}
