using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace API_CAP_RabbitMQ.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ICapPublisher _capBus;
        private readonly SystemContext _context;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICapPublisher capPublisher, SystemContext db)
        {
            _logger = logger;
            _capBus = capPublisher;
            _context = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var p = new Product()
            {
                Name = "Abóbora",
                Value = 20,
                Enable = false
            };

            await using (_context.Database.BeginTransaction(_capBus, autoCommit: true))
            {
                _context.Products.Add(p);
                // await _capBus.PublishAsync("test.queue", p);
                await _capBus.PublishAsync("test.queue", "xpto");
                await _context.SaveChangesAsync();
            }
            return Ok();
        }


    }
}
