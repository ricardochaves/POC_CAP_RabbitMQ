using System.Collections.Generic;
using API_CAP_RabbitMQ.Consumers;
using API_CAP_RabbitMQ.Support;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;


namespace API_CAP_RabbitMQ
{
    public class Startup
    {
        public Startup()
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SystemContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "API_CAP_RabbitMQ", Version = "v1"});
            });

            services.AddDbContext<SystemContext>();

            if (Configuration.GetValue<bool>("Is_Consumer"))
                services.AddTransient<ITestQueueConsumer,TestQueueConsumer>();

            services.AddCap(x =>
            {


                // If you are using EF, you need to add the configuration：
                x.UseEntityFramework<SystemContext>(); //Options, Notice: You don't need to config x.UseSqlServer(""") again! CAP can autodiscovery.

                // CAP support RabbitMQ,Kafka,AzureService as the MQ, choose to add configuration you needed：
                x.UseRabbitMQ(o =>
                {
                    o.HostName = "localhost";
                    o.Port = 5672;
                    o.ExchangeName = "Sale";

                    o.CustomHeaders  = e => new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(Headers.MessageName, e.RoutingKey),
                        new KeyValuePair<string, string>(Headers.MessageId, MessageIdChecker.CheckMessageIdHeader(e)),
                    };
                });
                x.UseDashboard();
                x.FailedRetryCount = 2;
                x.FailedThresholdCallback = failed =>
                {
                    var logger = failed.ServiceProvider.GetService<ILogger<Startup>>();
                    logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                        requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SystemContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_CAP_RabbitMQ v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            db.Database.EnsureCreated();
        }
    }
}
