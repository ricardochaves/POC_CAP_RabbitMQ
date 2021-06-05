using System.Threading.Tasks;
using CliFx;
using Financial.EntryCommands;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Financial
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<ApiCommand.Startup>();
                });
    }
}
