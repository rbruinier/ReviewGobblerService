using System.Linq;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Shared.Managers
{
    public class SiteManager
    {
        private DatabaseContext _databaseContext;

        public SiteManager(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Site FetchSiteById(int siteId)
        {
            return _databaseContext.Sites.SingleOrDefault(site => site.SiteId == siteId);
        }
    }
}
