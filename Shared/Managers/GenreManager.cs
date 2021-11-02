using System.Linq;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Shared.Managers
{
    public class GenreManager
    {
        private DatabaseContext _databaseContext;

        public GenreManager(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Genre FetchOrInsertGenreByName(string name)
        {
            var existingGenre = _databaseContext.Genres.SingleOrDefault(genre => genre.Name == name);

            if (existingGenre != null)
            {
                return existingGenre;
            }

            var genre = new Genre
            {
                Name = name
            };

            _databaseContext.Genres.Add(genre);

            _databaseContext.SaveChanges();

            return genre;
        }
    }
}
