using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoPets.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ContosoPets.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();

            using(var scope = scopeFactory.CreateScope())
            {
                var contosoPetsContext = scope.ServiceProvider.GetRequiredService<ContosoPetsContext>();

                if(contosoPetsContext.Database.EnsureCreated())
                {
                    try
                    {
                        SeedData.Initialize(contosoPetsContext);
                    }
                    catch(Exception ex)
                    {
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError($"Could not initialize database with seed data. Exception: {ex.Message}");
                    }
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
