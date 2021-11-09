using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Projectr.OleksiiKraievyi.CurrencyWorker;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
        services
            .AddLogging()
            .AddHostedService<CurrencyWorker>()
            .AddSingleton<ILoggerFactory, LoggerFactory>()
            .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
            .AddSingleton(services => new GoogleAnalyticsTrackingId(Environment.GetEnvironmentVariable("GOOGLE_ANALYTICS_TRACKING_ID")))
            .AddHttpClient<CurrencyWorker>())
    .Build()
    .Run();