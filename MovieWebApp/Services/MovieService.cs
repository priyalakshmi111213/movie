using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MovieWebApp.Helpers;
using MovieWebApp.Models;
using Newtonsoft.Json;
using NLog;

namespace MovieWebApp.Services
{
    /// <summary>
    /// MovieService Class which extends BaseService and Implements IMovieService Interface
    /// </summary>
    public class MovieService : IMovieService
    {
        private readonly IMemoryCache _cache;
        private readonly IBaseService _baseService;
        private static Logger logger = LogManager.GetCurrentClassLogger();



        public MovieService(IMemoryCache cache, IBaseService baseService)
        {
            _cache = cache;
            _baseService = baseService;
           
        }

        /// <summary>
        /// GetDeserializedMovies
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<Movie> GetDeserializedMovies(string obj)
        {
            try
            {
                string jsonstring = JsonConvert.SerializeObject(obj);
                var stringObject = JsonConvert.DeserializeObject<string>(jsonstring);
                MoviesHelper moviesHelper = JsonConvert.DeserializeObject<MoviesHelper>(stringObject);
                return moviesHelper.Movies;
            }
            catch (Exception ex)
            {
                logger.Error(ex,"Ëxception Occured while JsonConversion");
            }
            return new List<Movie>();
        }
       
        /// <summary>
        /// Get Movies from All Providers
        /// </summary>
        /// <returns></returns>
        public async Task<List<Movie>> GetMovies()
        {
            List<Movie> movies = new List<Movie>();
            try
            {
               
                if (_cache.TryGetValue(Constants.Movies, out IEnumerable<Movie> moviesCache))
                {
                    return moviesCache.ToList();
                }
                List<string> providers = _baseService.GetAllProviders();
                foreach (string provider in providers)
                {
                    HttpResponseMessage response = await _baseService.GetDataFromExternalAPI(provider + "/movies");
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        var obj = response.Content.ReadAsStringAsync().Result;
                        if (obj != null)
                        {
                            List<Movie> tempMovies = GetDeserializedMovies(obj);
                            tempMovies.ForEach(tempMovie => tempMovie.Provider = provider);
                            movies.AddRange(tempMovies);
                        }
                    }
                }
                if (movies != null && movies.Count != 0)
                {
                    _cache.Set(Constants.Movies, movies);
                }
                return movies.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Ëxception Occured while Fetching Movies");
            }

            return new List<Movie>();
        }

        /// <summary>
        /// GetMovieByIdAndProvider
        /// </summary>
        /// <param name="id"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task<MovieDetail> GetMovieByIdAndProvider(string id, string provider)
        {
            MovieDetail movieDetail = null;
            try
            {
                logger.Info("Request Movie by ID {0} Provider: {1}", provider, id);
                HttpResponseMessage response = await _baseService.GetDataFromExternalAPI(provider + "/movie/" + id);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var obj = response.Content.ReadAsStringAsync().Result;
                    if (obj != null)
                    {
                        string jsonstring = JsonConvert.SerializeObject(obj);
                        var deserializedString = JsonConvert.DeserializeObject<string>(jsonstring);
                        movieDetail = JsonConvert.DeserializeObject<MovieDetail>(deserializedString);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex,"Unable to Fetch Movie with Id: {0} and Provider: {1}",id,provider);
            }
            
            return movieDetail;
        }

        // <summary>
        // Get FullMovie Details
        // </summary>
        // <returns></returns>
        public async Task<List<MovieDetail>> GetFullMovieDetails()
        {
            List<MovieDetail> movieDetails = new List<MovieDetail>();
            try
            {
                if (_cache.TryGetValue(Constants.MovieDetails, out IEnumerable<MovieDetail> moviesDetailsCache))
                {
                    return moviesDetailsCache.ToList();
                }
                List<Movie> movies = await GetMovies();
                logger.Info("Count of movies to Fetch movie details by provider and Id : {0}",movies.Count);
                foreach (Movie movie in movies)
                {
                    MovieDetail movieDetail = await GetMovieByIdAndProvider(movie.Id, movie.Provider);
                    if (movieDetail!=null)
                    {
                        movieDetail.Provider = movie.Provider;
                        movieDetails.Add(movieDetail);
                    }
                }
                if (movieDetails != null && movieDetails.Count != 0)
                {
                    _cache.Set(Constants.MovieDetails, movieDetails);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unable to Fetch Movie Details");
            }
            return movieDetails;
        }

        /// <summary>
        /// GetLeasePriceMovies
        /// </summary>
        /// <returns></returns>
        public List<MovieDetail> GetLeasePriceMovies(List<MovieDetail> movieDetails)
        {
            List<MovieDetail> comparedMovies = new List<MovieDetail>();
            try
            {
                var list = from movie in movieDetails
                           group movie by movie.Title into movieGrp
                           let minPriceMovie = movieGrp.Min(x => x.Price)
                           select new
                           {
                               movieTitleKey = movieGrp.Key,
                               comparedMovieObj = movieGrp.First(s => s.Price == minPriceMovie),
                               price = minPriceMovie
                           };
                if (list != null)
                {
                    foreach (var movieObj in list)
                    {
                        if (movieObj.comparedMovieObj.Id != null)
                        {
                            comparedMovies.Add(movieObj.comparedMovieObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unable to Fetch Movies with compared price");
            }
            return comparedMovies;
        }

        /// <summary>
        /// GetMoviesByComparingPrice
        /// </summary>
        /// <returns></returns>
        public async Task<List<MovieDetail>> GetMoviesByComparingPrice()
        {
            List<MovieDetail> movieDetails = await GetFullMovieDetails();
            return  GetLeasePriceMovies(movieDetails);
        }



        /// <summary>
        /// ClearCache
        /// </summary>
        /// <returns></returns>
        public Boolean ClearCache()
        {
            _cache.Remove(Constants.MovieDetails);
            _cache.Remove(Constants.Movies);
            return true;
        }
    }
}
