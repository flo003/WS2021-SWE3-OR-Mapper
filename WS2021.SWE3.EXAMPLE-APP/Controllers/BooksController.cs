using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS2021.SWE3.EXAMPLE_APP.Model;

namespace WS2021.SWE3.EXAMPLE_APP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Book> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Book
            {
                Name = Summaries[rng.Next(Summaries.Length)],
                Author = Summaries[rng.Next(Summaries.Length)],
                IBan = Summaries[rng.Next(Summaries.Length)],
                ReleaseDate = DateTime.Now.AddDays(index),
            })
            .ToArray();
        }
    }
}
