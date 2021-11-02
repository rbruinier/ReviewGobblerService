using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReviewGobbler.Service.Business.Players.AppleMusic;
using ReviewGobbler.Service.Business.Players.Spotify;
using ReviewGobbler.Service.Controllers.Model;
using ReviewGobbler.Shared.DAL;

namespace ReviewGobbler.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ISpotifyPlayerResolver _spotifyResolver;
        private readonly IAppleMusicPlayerResolver _appleMusicResolver;
        private readonly ILogger<ReviewController> _logger;

        public PlayerController(
            DatabaseContext databaseContext,
            ISpotifyPlayerResolver spotifyResolver,
            IAppleMusicPlayerResolver appleMusicResolver,
            ILogger<ReviewController> logger)
        {
            _databaseContext = databaseContext;
            _spotifyResolver = spotifyResolver;
            _appleMusicResolver = appleMusicResolver;
            _logger = logger;
        }

        [HttpGet("spotify")]
        public async Task<ResponsePlayer> GetSpotify(int albumId)
        {
            var album = _databaseContext
                .Albums
                .Include(album => album.AlbumArtists)
                    .ThenInclude(albumArtist => albumArtist.Artist)
                .Where(album => album.AlbumId == albumId)
                .FirstOrDefault();

            if (album == null)
            {
                return null;
            }

            var resolvedAlbum = await _spotifyResolver.ResolveAlbum(album.Name, album.AlbumArtists[0].Artist.Name);

            if (resolvedAlbum == null)
            {
                return null;
            }

            return new ResponsePlayer()
            {
                LocalUri = resolvedAlbum.LocalUri,
                WebUrl = resolvedAlbum.WebUrl,
                PlayerId = 1
            };
        }

        [HttpGet("appleMusic")]
        public async Task<ResponsePlayer> GetAppleMusic(int albumId)
        {
            var album = _databaseContext
                .Albums
                .Include(album => album.AlbumArtists)
                    .ThenInclude(albumArtist => albumArtist.Artist)
                .Where(album => album.AlbumId == albumId)
                .FirstOrDefault();

            if (album == null)
            {
                return null;
            }

            var resolvedAlbum = await _appleMusicResolver.ResolveAlbum(album.Name, album.AlbumArtists[0].Artist.Name);

            if (resolvedAlbum == null)
            {
                return null;
            }

            return new ResponsePlayer()
            {
                LocalUri = resolvedAlbum.LocalUri,
                WebUrl = resolvedAlbum.WebUrl,
                PlayerId = 1
            };
        }
    }
}
