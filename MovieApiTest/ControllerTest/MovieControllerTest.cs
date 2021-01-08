using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieWebApp.Controllers;
using MovieWebApp.Models;
using MovieWebApp.Services;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MovieApiTest
{
    [TestClass]
    public class MovieControllerTest
    {
        [Fact]
        public async Task GetMoviesTest()
        {
            List<MovieDetail> movies = new List<MovieDetail>()
            {
                new MovieDetail(){ Title="test_1", Id="cm1"},
                new MovieDetail(){ Title="test_2", Id="cm1"},
                new MovieDetail(){ Title="test_1", Id="fm1"},
                new MovieDetail(){ Title="test_2", Id="fm1"},
            };
            var movieService = new Mock<IMovieService>();
            movieService.Setup(s => s.GetMoviesByComparingPrice()).Returns(() => Task.FromResult(movies));
            MovieController movieController = new MovieController(movieService.Object);
            IActionResult actionResult = await movieController.GetMoviesAsync();
            OkObjectResult okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOfType(actionResult,typeof(OkObjectResult));
        }


    }
}
