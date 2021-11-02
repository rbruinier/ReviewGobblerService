using System;
using System.Collections.Generic;

namespace ReviewGobbler.Shared.Model
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public int Version { get; set; } // there could be multiple artists with the same name, we need a way to keep track of multiple
        public IList<AlbumArtist> AlbumArtists { get; set; }
    }
}
