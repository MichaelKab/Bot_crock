﻿using System;
using System.Threading.Tasks;
using Croc.Medkiosk.TelegramBot.Data;
using Croc.Medkiosk.TelegramBot.Data.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Croc.Medkiosk.TelegramBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();
                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    configuration
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    var loggingConfigSection = hostingContext.Configuration.GetSection("Logging");
                    logging.AddFile(loggingConfigSection);
                })
                .ConfigureServices((_, services) =>
                {
                    services
                        .Configure<BotConfig>(_.Configuration.GetSection(nameof(BotConfig)));

                    services
                        .AddHostedService<BotManager>()
                        .AddDbContextFactory<newmed2_dockerContext>(
                            options => options.UseNpgsql(
                                _.Configuration.GetConnectionString(nameof(newmed2_dockerContext))));

                    services.AddSingleton<DbQueries>();
                });
    }
}
