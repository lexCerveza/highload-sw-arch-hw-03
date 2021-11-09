using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Projectr.OleksiiKraievyi.CurrencyWorker
{
    public static class GoogleAnalyticsApiModule
    {
        private const string Uri = "http://google-analytics.com/collect?";
        private const string Version = "1";

        public static async Task PushEventAsync(
            HttpClient httpClient,
            GoogleAnalyticsTrackingId trackingId,
            GoogleAnalyticsEvent eventMessage,
            CancellationToken cancellationToken)
        {
            var (clientId, category, action, label) = eventMessage;

            var requestParameters = new []
            {
                new KeyValuePair<string, string>("cid", clientId),
                new KeyValuePair<string, string>("t", "event"),
                new KeyValuePair<string, string>("ec", category),
                new KeyValuePair<string, string>("ea", action),
                new KeyValuePair<string, string>("el", label),
                new KeyValuePair<string, string>("tid", trackingId.id),
                new KeyValuePair<string, string>("v", Version)
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Uri}&{string.Join("&", requestParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}")
            };

            request.Headers.Add("User-Agent", "currency-worker/1");

            var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }

    public record GoogleAnalyticsTrackingId(string id);
    public record GoogleAnalyticsEvent(string clientId, string category, string action, string label);
}