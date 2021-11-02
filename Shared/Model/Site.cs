using System.Collections.Generic;

namespace ReviewGobbler.Shared.Model
{
    public class Site
    {
        public int SiteId { get; set; }
        public string Name { get; set; }
        public IList<Review> Reviews { get; set; }
    }
}
