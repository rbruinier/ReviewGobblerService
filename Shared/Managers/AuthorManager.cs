using System.Linq;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Shared.Managers
{
    public class AuthorManager
    {
        private DatabaseContext _databaseContext;

        public AuthorManager(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Author FetchOrInsertAuthorByName(string name)
        {
            var existingAuthor = _databaseContext.Authors.SingleOrDefault(author => author.Name == name);

            if (existingAuthor != null)
            {
                return existingAuthor;
            }

            var author = new Author
            {
                Name = name
            };

            _databaseContext.Authors.Add(author);

            _databaseContext.SaveChanges();

            return author;
        }
    }
}
