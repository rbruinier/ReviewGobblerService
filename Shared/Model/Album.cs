using System;
using System.Collections.Generic;

namespace ReviewGobbler.Shared.Model
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string Name { get; set; }
        public int? ReleaseYear { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public IList<AlbumArtist> AlbumArtists { get; set; }
        public IList<AlbumGenre> AlbumGenres { get; set; }
        public IList<AlbumLabel> AlbumLabels { get; set; }

        public IList<AlbumArtwork> AlbumArtworks { get; set; }

        public IList<Review> Reviews { get; set; }
    }
}
