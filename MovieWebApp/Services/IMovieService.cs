using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieWebApp.Models;

namespace MovieWebApp.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetMovies();
        Task<List<MovieDetail>> GetFullMovieDetails();
        Task<List<MovieDetail>> GetMoviesByComparingPrice();
        Boolean ClearCache();
    }
}
