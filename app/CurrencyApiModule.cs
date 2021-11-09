using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Projectr.OleksiiKraievyi.CurrencyWorker
{
    public static class CurrencyApiModule
    {
        private const string GetCurrenciesUrl = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";

        public static async Task<CurrencyInfo[]> GetCurrenciesAsync(HttpClient httpClient, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync(GetCurrenciesUrl);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CurrencyInfo[]>();
        }
    }

    public record CurrencyInfo(string cc, decimal rate);
}