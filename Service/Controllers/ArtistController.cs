using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Model;
using ReviewGobbler.Service.Controllers.Model;
using Microsoft.EntityFrameworkCore;

namespace ReviewGobbler.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<ArtistController> _logger;

        public ArtistController(DatabaseContext databaseContext, ILogger<ArtistController> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }

        [HttpGet("{artistId}")]
        public ResponseArtist Get(int artistId)
        {
            var artist = _databaseContext
                .Artists
                .Include(artist => artist.AlbumArtists)
                    .ThenInclude(albumArtist => albumArtist.Album)
                        .ThenInclude(album => album.AlbumGenres)
                            .ThenInclude(albumGenre => albumGenre.Genre)
                .Include(artist => artist.AlbumArtists)
                    .ThenInclude(albumArtist => albumArtist.Artist)
                .Include(artist => artist.AlbumArtists)
                    .ThenInclude(albumArtist => albumArtist.Album)
                        .ThenInclude(album => album.Reviews)
                .Include(artist => artist.AlbumArtists)
                    .ThenInclude(albumArtist => albumArtist.Album)
                        .ThenInclude(album => album.AlbumArtworks)
                .Where(artist => artist.ArtistId == artistId)
                .FirstOrDefault();

            if (artist != null)
            {
                return new ResponseArtist(artist, 0);
            }
            else
            {
                return null;
            }
        }
    }
}
