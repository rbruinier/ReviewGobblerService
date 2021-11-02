using System;
namespace ReviewGobbler.Shared.Model
{
    public class AlbumArtwork
    {
        public int AlbumArtworkId { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; }

        public string Label { get; set; }
        public string Url { get; set; }

        public int SiteId { get; set; }
        public Site Site { get; set; }
    }
}
