using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Financial.Consumers;
using Financial.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Financial.EntryCommands
{
    [Command("order-created-consumer")]
    public class OrderCreatedConsumerCommand : ICommand
    {
        public IConfiguration Configuration { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables().Build();

            await Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureServices(StartupServices)
                .Build()
                .RunAsync();
        }

        private void StartupServices(IServiceCollection services)
        {
            services.AddScoped<OrderCreatedConsumer>();

            services.AddDbContext<SystemContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SystemContext")));

            services.AddCap(x =>
            {
                // If you are using EF, you need to add the configuration：
                x.UseEntityFramework<SystemContext>();
                // CAP support RabbitMQ,Kafka,AzureService as the MQ, choose to add configuration you needed：
                x.UseRabbitMQ(o =>
                {
                    o.HostName = "localhost";
                    o.Port = 5672;
                    o.ExchangeName = "Sale";

                    o.CustomHeaders = e => new List<KeyValuePair<string, string>>
                    {
                        // TODO: use MessageIdChecker
                        new(Headers.MessageId, SnowflakeId.Default().NextId().ToString()), new(Headers.MessageName, e.RoutingKey)
                    };
                });

                x.FailedRetryCount = 2;
                x.FailedThresholdCallback = failed =>
                {
                    var logger = failed.ServiceProvider.GetService<ILogger<OrderCreatedConsumerCommand>>();
                    logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                        requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
                };
            });
        }
    }
}
