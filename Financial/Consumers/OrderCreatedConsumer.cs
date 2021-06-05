using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace Financial.Consumers
{
    public class OrderCreatedConsumer : ICapSubscribe
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        [CapSubscribe("order.created", Group = "financial.order.created")]
        public void CheckReceivedMessage(OrderCreatedMessage message)
        {
            _logger.LogInformation($"Financial: Sale created message processed: {message}");
        }
    }

    public record OrderCreatedMessage(string ProductCode, int Quantity);
}
