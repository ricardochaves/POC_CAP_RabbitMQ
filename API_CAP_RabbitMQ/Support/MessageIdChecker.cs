using System;
using System.Linq;
using System.Text;
using DotNetCore.CAP.Internal;
using RabbitMQ.Client.Events;

namespace API_CAP_RabbitMQ.Support
{
    public static class MessageIdChecker
    {
        public static string CheckMessageIdHeader(BasicDeliverEventArgs e)
        {
            if (e.BasicProperties.Headers == null)
                return SnowflakeId.Default().NextId().ToString();

            var (_, value) = e.BasicProperties.Headers.FirstOrDefault(x => x.Key == "cap-msg-id");
            return value != null ? Encoding.UTF8.GetString((byte[]) value) : SnowflakeId.Default().NextId().ToString();
        }
    }
}
