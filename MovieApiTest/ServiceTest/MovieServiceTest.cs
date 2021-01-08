using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieWebApp.Helpers;
using MovieWebApp.Models;
using MovieWebApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MovieApiTest.ServiceTest
{
    [TestClass]
    public class MovieServiceTest
    {
        private MovieService movieServiceTest;
        public const string filmWorld = "filmworld";
        public const string cinemaWorld = "cinemaworld";


        /// <summary>
        /// Return empty list if providers are not present
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetEmptyListMovies_IfProvidersAreNotPresent()
        {
            var mockBaseService = new Mock<IBaseService>();
            var mockIMemoryCache = new Mock<IMemoryCache>();
            MovieService service = new MovieService(mockIMemoryCache.Object, mockBaseService.Object);
            mockBaseService.Setup(s => s.GetDataFromExternalAPI(It.IsAny<string>())).Returns(() => Task.FromResult(getMockHttpResponseObject()));
            List<Movie> movies = await service.GetMovies();
            Assert.IsNotNull(movies);
        }

        /// <summary>
        /// Test for Least Price Logic
        /// </summary>
        [Fact]
        public void GetLeasetPriceMovies_WhenPassingAllProviderMovies()
        {
            var mockBaseService = new Mock<IBaseService>();
            var mockIMemoryCache = new Mock<IMemoryCache>();
            MovieService service = new MovieService(mockIMemoryCache.Object, mockBaseService.Object);
            List<MovieDetail> movies = service.GetLeasePriceMovies(this.getMoviesListOfAllProviders());
            Assert.IsNotNull(movies);
            Assert.AreEqual(movies.Count,3);
        }

        /// <summary>
        /// Test for Serialization
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetMovieByIdAndProvider_LogicForSerialization()
        {
            var mockBaseService = new Mock<IBaseService>();
            var mockIMemoryCache = new Mock<IMemoryCache>();
            var mockIConfiguration = new Mock<IConfiguration>();
            MovieService service = new MovieService(mockIMemoryCache.Object, mockBaseService.Object);
            mockBaseService.Setup(s => s.GetDataFromExternalAPI(It.IsAny<string>())).Returns(() => Task.FromResult(this.getMockHttpResponseObject()));
            var movieDetail = await service.GetMovieByIdAndProvider("1", cinemaWorld);
            Assert.IsNotNull(movieDetail);
            Assert.IsNotNull(movieDetail.Id);
            Assert.IsInstanceOfType(movieDetail, typeof(MovieDetail));
        }

        /// <summary>
        /// Test to get all movies from external source
        /// </summary>
        /// <returns></returns>
        [Fact]
        [Priority(0)]
        public async Task GetMoviesDataFromExternalSource()
        {
            HttpResponseMessage responseMessage = null;
            var handler = new TimeoutHandler
            {
                DefaultTimeout = TimeSpan.FromSeconds(100),
                InnerHandler = new HttpClientHandler()
            };
            using (var cts = new CancellationTokenSource())
            using (var client = new HttpClient(handler))
            {
                var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings-test.json")
                                .Build();
                var baseUrl = config[ConfigurationConstants.APIUrl];
                var headerKey = config.GetSection(ConfigurationConstants.Header).GetSection(ConfigurationConstants.Key).Value;
                var headerValue = config.GetSection(ConfigurationConstants.Header).GetSection(ConfigurationConstants.Value).Value;
                client.Timeout = Timeout.InfiniteTimeSpan;
                client.DefaultRequestHeaders.Add(headerKey,headerValue);
                var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + filmWorld + "/movies");
                responseMessage = await client.SendAsync(request, cts.Token);
                responseMessage.EnsureSuccessStatusCode();
                Assert.AreEqual(responseMessage.StatusCode,HttpStatusCode.OK);
            }
        }


        #region data

        public HttpResponseMessage getMockHttpResponseObject()
        {
            MovieDetail movie = new MovieDetail()
            {
                Id="1",Title ="Movie 1",Price=20,Year=2019
            };
            var json = JsonConvert.SerializeObject(movie);
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage.StatusCode = HttpStatusCode.OK;
            responseMessage.Content = new StringContent(json);
            return responseMessage;
        }

        public List<MovieDetail> getMoviesListOfAllProviders()
        {
            List<MovieDetail> movies = new List<MovieDetail>()
            {
                new MovieDetail{ Id ="1",  Title ="Movie 1",Price=20,Year=2019 },
                new MovieDetail{ Id ="2",  Title ="Movie 1",Price=30,Year=2019 },
                new MovieDetail{ Id ="3",  Title ="Movie 2",Price=40 ,Year=2019},
                new MovieDetail{ Id ="4",  Title ="Movie 2",Price=50,Year=2019 },
                new MovieDetail{ Id ="4",  Title ="Movie 3",Price=50,Year=2019 }
            };
            return movies;
        }
        #endregion

    }
}
