using System;
using System.Collections.Generic;

namespace ReviewGobbler.Shared.Model
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public IList<AlbumGenre> AlbumGenres { get; set; }
    }
}
