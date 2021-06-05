using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using API_CAP_RabbitMQ.Consumers;
using API_CAP_RabbitMQ.Support;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using DotNetCore.CAP.Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;

namespace API_CAP_RabbitMQ
{
    public class Program
    {
        public static async Task<int> Main()
        {
            return await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

    [Command]
    public class ApiCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            await Program.CreateHostBuilder(Array.Empty<string>()).Build().RunAsync();
        }
    }

    [Command("consumer")]
    public class ConsumerCommand : ICommand
    {
        public IConfigurationRoot Configuration { get; private set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureServices(services =>
                {
                    Configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables().Build();
                    services.AddDbContext<SystemContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
                    services.AddDatabaseDeveloperPageExceptionFilter();

                    services.AddDbContext<SystemContext>();

                    services.AddTransient<ITestQueueConsumer, TestQueueConsumer>();

                    services.AddCap(x =>
                    {
                        // If you are using EF, you need to add the configuration：
                        x.UseEntityFramework<SystemContext>(); //Options, Notice: You don't need to config x.UseSqlServer(""") again! CAP can autodiscovery.

                        // CAP support RabbitMQ,Kafka,AzureService as the MQ, choose to add configuration you needed：
                        x.UseRabbitMQ(o =>
                        {
                            o.HostName = "localhost";
                            o.Port = 5672;
                            o.ExchangeName = "POC";

                            o.CustomHeaders = e => new List<KeyValuePair<string, string>>
                            {
                                // new KeyValuePair<string, string>(Headers.MessageId, SnowflakeId.Default().NextId().ToString()),
                                new KeyValuePair<string, string>(Headers.MessageName, e.RoutingKey),
                                new KeyValuePair<string, string>(Headers.MessageId, MessageIdChecker.CheckMessageIdHeader(e)),
                            };
                        });
                        x.FailedRetryCount = 2;
                        x.FailedThresholdCallback = failed =>
                        {
                            var logger = failed.ServiceProvider.GetService<ILogger<Startup>>();
                            logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                            requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
                        };
                    });
                })
                .Build()
                .RunAsync();
        }
    }
}