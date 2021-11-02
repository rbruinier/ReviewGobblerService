using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReviewGobbler.Shared.DAL;
using ReviewGobbler.Shared.Model;
using Microsoft.EntityFrameworkCore;
using ReviewGobbler.Service.Controllers.Model;

namespace ReviewGobbler.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(DatabaseContext databaseContext, ILogger<ReviewController> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }

        [HttpGet("list")]
        public IEnumerable<ResponseAlbum> Get(int? page, int? minimumRating, int? maximumRating, string genreIds, string siteIds)
        {
            if (page.HasValue == false)
            {
                page = 0;
            }

            List<int> parsedGenreIds = null;

            if (genreIds != null)
            {
                parsedGenreIds = genreIds.Split(',')
                   .Select(i => int.Parse(i)).ToList<int>();
            }

            List<int> parsedSiteIds = null;

            if (siteIds != null)
            {
                parsedSiteIds = siteIds.Split(',')
                   .Select(i => int.Parse(i)).ToList<int>();
            }

            var albumQuery = _databaseContext
                .Albums.AsQueryable();

            albumQuery = albumQuery
                .Include(album => album.AlbumGenres)
                    .ThenInclude(albumGenre => albumGenre.Genre)
                .Include(album => album.AlbumArtists)
                    .ThenInclude(albumArtist => albumArtist.Artist)
                .Include(album => album.Reviews)
                .Include(album => album.AlbumArtworks);

            if (minimumRating.HasValue)
            {
                albumQuery = albumQuery
                    .Where(album => album
                        .Reviews.Average(review => review.Rating) >= minimumRating.Value);
            }

            if (maximumRating.HasValue)
            {
                albumQuery = albumQuery
                    .Where(album => album
                        .Reviews.Average(review => review.Rating) <= maximumRating.Value);

            }

            if (parsedGenreIds != null)
            {
                albumQuery = albumQuery
                    .Where(album => album
                        .AlbumGenres
                        .Any(albumGenre => parsedGenreIds.Contains(albumGenre.GenreId)));
            }


            if (parsedSiteIds != null)
            {
                albumQuery = albumQuery
                    .Where(album => album
                        .Reviews
                        .Any(review => parsedSiteIds.Contains(review.SiteId)));
            }

            var albums = albumQuery
                .OrderByDescending(album => album
                    .Reviews
                    .OrderByDescending(review => review
                        .PublishedDate)
                    .First()
                    .PublishedDate)
                .Skip(page.Value * 20)
                .Take(20)
                .ToList<Album>();

            return albums.Select(album => new ResponseAlbum(album, 0)).ToArray();
        }
    }
}
