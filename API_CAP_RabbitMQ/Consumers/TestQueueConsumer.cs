using System;
using System.Threading.Tasks;
using CAP_Consumer;
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

            Consumer.ExecuteMessage(product, Execute);
        }

        // [CapSubscribe("user.command",Group = "User.Command")]
        // public void CheckProduct(Product product)
        // {
        //     Console.Write("User Command" + product);
        // }

        public static async void Execute(PKGMessage<Product> message)
        {
            var p = message.Data;
            // ...
            message.Commit();
        }
    }
}
