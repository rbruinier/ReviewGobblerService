using System;
using System.Collections.Generic;
using System.Linq;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Service.Controllers.Model
{
    public class ResponseArtist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ResponseAlbum> Albums { get; set; }

        public ResponseArtist(Artist artist, int nestingLevel)
        {
            Id = artist.ArtistId;
            Name = artist.Name;

            if (nestingLevel == 0 && artist.AlbumArtists != null)
            {
                Albums = artist.AlbumArtists.Select(albumArtist => new ResponseAlbum(albumArtist.Album, nestingLevel + 1)).ToArray();
            }
        }
    }
}
