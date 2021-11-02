using Microsoft.EntityFrameworkCore;
using System.Linq;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Shared.Managers
{
    public class ReviewManager
    {
        private DatabaseContext _databaseContext;

        public ReviewManager(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Review FetchReviewBySiteReviewId(int siteId, string siteReviewId, int albumId)
        {
            return _databaseContext
                .Reviews
                .Where(review => review.Site.SiteId == siteId)
                .Where(review => review.Album.AlbumId == albumId)
                .SingleOrDefault(review => review.SiteReviewId == siteReviewId);
        }

        public void InsertReview(Review review)
        {
            _databaseContext.Reviews.Add(review);

            _databaseContext.SaveChanges();
        }
    }
}
