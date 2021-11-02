using System;
using System.Collections.Generic;

namespace ReviewGobbler.Service.Controllers.Model
{
    public class SearchResponse
    {
        public IEnumerable<ResponseArtist> Artists { get; set; }
        public IEnumerable<ResponseAlbum> Albums { get; set; }

        public SearchResponse(IEnumerable<ResponseArtist> artists, IEnumerable<ResponseAlbum> albums)
        {
            Artists = artists;
            Albums = albums;
        }
    }
}
