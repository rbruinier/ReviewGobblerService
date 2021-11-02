using System;
using System.Collections.Generic;
using System.Linq;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Service.Controllers.Model
{
    public class ResponseAlbum
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ReleaseYear { get; set; }
        public IList<ResponseArtist> Artists { get; set; }
        public IList<ResponseGenre> Genres { get; set; }
        public IList<ResponseReview> Reviews { get; set; }
        public string MainArtworkUrl { get; set; }

        public ResponseAlbum(Album album, int nestingLevel)
        {
            Id = album.AlbumId;
            Name = album.Name;
            ReleaseYear = album.ReleaseYear;
            Reviews = album.Reviews.Select(review => new ResponseReview(review)).ToArray();
            Genres = album.AlbumGenres.Select(albumGenre => new ResponseGenre(albumGenre.Genre)).ToArray();

            // this allows a complete list of artists for all albums of an artist
            if (nestingLevel <= 1)
            {
                Artists = album.AlbumArtists.Select(albumArtist => new ResponseArtist(albumArtist.Artist, nestingLevel + 1)).ToArray();
            }

            MainArtworkUrl = GetBestAlbumArtworkUrl(album);
        }

        private string GetBestAlbumArtworkUrl(Album album)
        {
            var artwork = album.AlbumArtworks.FirstOrDefault(artwork => artwork.Label == "standard");

            if (artwork == null)
            {
                return null;
            }

            return artwork.Url;
        }
    }
}
