using System.Net.Http;
using System.Threading.Tasks;

namespace HistoricalBordersPlugin.Utils
{
    internal static class HttpFetcher
    {
        private static readonly HttpClient _client = new HttpClient();

        public static async Task<string> GetStringAsync(string url)
        {
            return await _client.GetStringAsync(url);
        }
    }
}
