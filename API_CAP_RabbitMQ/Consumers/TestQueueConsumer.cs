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
        [CapSubscribe("test")]
        public void CheckReceivedMessage(Product product)
        {
            Console.WriteLine(product);
        }
    }
}
