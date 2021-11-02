using System;
namespace ReviewGobbler.Shared.Model
{
    public class AuthorReview
    {
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int ReviewId { get; set; }
        public Review Review { get; set; }
    }
}
