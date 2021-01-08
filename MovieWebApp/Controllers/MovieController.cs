using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieWebApp.Helpers;
using MovieWebApp.Models;
using MovieWebApp.Services;
using NLog;

namespace MovieWebApp.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        /// <summary>
        /// Get all movies by price comparison from providers
        /// </summary>
        /// <returns></returns>
        [HttpGet("movies")]
        public async Task<IActionResult> GetMoviesAsync()
        {
            return Ok(await _movieService.GetMoviesByComparingPrice());
        }

        /// <summary>
        /// Clear the cache 
        /// </summary>
        /// <returns></returns>
        [HttpGet("clearcache")]
        public Boolean GetCacheCleared()
        {
            return _movieService.ClearCache();
        }
    }
}
