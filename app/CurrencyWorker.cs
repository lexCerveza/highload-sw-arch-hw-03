using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Projectr.OleksiiKraievyi.CurrencyWorker
{
    public class CurrencyWorker : BackgroundService
    {
        private static readonly TimeSpan RunInterval = TimeSpan.FromMinutes(1);

        private readonly ILogger<CurrencyWorker> _logger;
        private readonly GoogleAnalyticsTrackingId _googleAnalyticsTrackingId;
        private readonly HttpClient _httpClient;

        public CurrencyWorker(
            ILogger<CurrencyWorker> logger,
            GoogleAnalyticsTrackingId googleAnalyticsTrackingId,
            HttpClient httpClient) => (_logger, _googleAnalyticsTrackingId, _httpClient) = (logger, googleAnalyticsTrackingId, httpClient);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var currencies = await CurrencyApiModule.GetCurrenciesAsync(_httpClient, stoppingToken);
                decimal? usdRate = GetRate(currencies, "usd");

                if (!usdRate.HasValue)
                {
                    _logger.LogWarning("Can't find USD rate. Skipping iteration.");
                    await Task.Delay(RunInterval);

                    continue;
                }

                _logger.LogInformation($"USD rate : {usdRate}");

                var googleAnalyticsEvent = GetGoogleAnalyticsEvent(usdRate.Value);

                await GoogleAnalyticsApiModule.PushEventAsync(
                    _httpClient,
                    _googleAnalyticsTrackingId,
                    googleAnalyticsEvent,
                    stoppingToken);

                await Task.Delay(RunInterval);
            }
        }

        private static decimal? GetRate(CurrencyInfo[] currencies, string cc)
            => currencies
                .FirstOrDefault(c => c.cc.ToLowerInvariant().Equals(cc.ToLowerInvariant()))
                ?.rate;

        private static GoogleAnalyticsEvent GetGoogleAnalyticsEvent(decimal rate)
        {
            return new GoogleAnalyticsEvent(
                $"currency-worker-{Guid.NewGuid()}",
                "currency-worker-job",
                "get",
                $"UAH/USD rate -> {rate}");
        }
    }
}