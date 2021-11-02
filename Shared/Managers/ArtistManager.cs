using System.Linq;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Shared.Managers
{
    public class ArtistManager
    {
        private DatabaseContext _databaseContext;

        public ArtistManager(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Artist FetchOrInsertArtistByName(string name)
        {
            var existingArtist = _databaseContext.Artists.SingleOrDefault(artist => artist.Name == name);

            if (existingArtist != null)
            {
                return existingArtist;
            }

            var artist = new Artist
            {
                Name = name,
                Version = 1
            };

            _databaseContext.Artists.Add(artist);

            _databaseContext.SaveChanges();

            return artist;
        }
    }
}
