using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Sale.Infrastructure;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ICapPublisher _capBus;
        private readonly SystemContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            ICapPublisher capBus,
            SystemContext context,
            ILogger<OrderController> logger)
        {
            _capBus = capBus;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrder()
        {
            await using (_context.Database.BeginTransaction(_capBus, autoCommit: true))
            {
                var order = new Order
                {
                    ProductCode = "1234",
                    Quantity = 5
                };
                _context.Orders.Add(order);

                await _capBus.PublishAsync("order.created", new
                {
                    order.ProductCode,
                    order.Quantity
                });
            }

            _logger.LogInformation("Order sent");

            return Ok();
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
    }
}
