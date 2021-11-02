using System;
using System.Collections.Generic;

namespace ReviewGobbler.Shared.Model
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public IList<AuthorReview> AuthorReviews { get; set; }
    }
}
