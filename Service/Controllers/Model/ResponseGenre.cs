using System;
using ReviewGobbler.Shared.Model;

namespace ReviewGobbler.Service.Controllers.Model
{
    public class ResponseGenre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ResponseGenre(Genre genre)
        {
            Id = genre.GenreId;
            Name = genre.Name;
        }
    }
}
