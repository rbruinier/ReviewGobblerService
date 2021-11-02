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
    public class SearchController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<SearchController> _logger;

        public SearchController(DatabaseContext databaseContext, ILogger<SearchController> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }

        [HttpGet("query")]
        public SearchResponse Get(string keyword)
        {
            var artists = _databaseContext
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
                .Where(artist => EF.Functions.Like(artist.Name, $"%{keyword}%"))
                .Take(10)
                .ToList<Artist>();

            var albums = _databaseContext
                .Albums
                .Include(album => album.AlbumGenres)
                    .ThenInclude(albumGenre => albumGenre.Genre)
                .Include(album => album.AlbumArtists)
                    .ThenInclude(albumArtist => albumArtist.Artist)
                .Include(album => album.Reviews)
                .Include(album => album.AlbumArtworks)
                .Where(album => EF.Functions.Like(album.Name, $"%{keyword}%"))
                .Take(10)
                .ToList<Album>();

            return new SearchResponse(
                artists.Select(artist => new ResponseArtist(artist, 0)).ToArray(),
                albums.Select(album => new ResponseAlbum(album, 0)).ToArray());
        }
    }
}
