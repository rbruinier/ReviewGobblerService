using Microsoft.EntityFrameworkCore;
using System.Linq;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Shared.Managers
{
    public class AlbumManager
    {
        private DatabaseContext _databaseContext;

        public AlbumManager(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Album FetchAlbumByNameAndArtist(string name, Artist artist)
        {
            var existingAlbum = _databaseContext
                .Albums
                .Include(album => album.AlbumArtists)
                    .ThenInclude(albumArtist => albumArtist.Artist)
                .Include(album => album.AlbumGenres)
                    .ThenInclude(albumGenre => albumGenre.Genre)
                .Include(album => album.AlbumLabels)
                    .ThenInclude(albumLabel => albumLabel.Label)
                .Where(album => album
                        .AlbumArtists
                        .Any(albumArtist => albumArtist.ArtistId == artist.ArtistId))
                .SingleOrDefault(album => album.Name == name);

            return existingAlbum;
        }

        public void InsertOrUpdateAlbum(Album album)
        {
            _databaseContext.Albums.Update(album);

            _databaseContext.SaveChanges();
        }
    }
}
