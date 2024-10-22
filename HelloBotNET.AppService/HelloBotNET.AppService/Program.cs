// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the MIT License, See LICENCE in the project root for license information.

using HelloBotNET.AppService;
using HelloBotNET.AppService.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Is(Serilog.Events.LogEventLevel.Information)
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (context, services) =>
        {
            // Add bot service.
            services.AddSingleton<HelloBot>();

            // Add long polling service
            services.AddHostedService<Worker>();
        }
    )
    .Build();

await host.RunAsync();

Console.WriteLine("Start!");
