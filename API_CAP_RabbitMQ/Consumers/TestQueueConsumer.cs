using System;
using DotNetCore.CAP;
using Models;

namespace API_CAP_RabbitMQ.Consumers
{

    public interface ITestQueueConsumer
    {
        void CheckReceivedMessage(Product product);
    }

    public class TestQueueConsumer: ITestQueueConsumer, ICapSubscribe
    {
        [CapSubscribe("product.create", Group = "Product.created")]
        public void CheckReceivedMessage(Product product)
        {
            Console.WriteLine("Product Create" + product);
        }

        [CapSubscribe("user.command",Group = "User.Command")]
        public void CheckProduct(Product product)
        {
            Console.Write("User Command" + product);
        }
    }
}
