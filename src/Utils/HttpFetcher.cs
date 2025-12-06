using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HistoricalBordersPlugin.Utils
{
    internal static class HttpFetcher
    {
        private static readonly HttpClient _client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        /// <summary>
        /// URL から文字列（UTF-8）を取得する
        /// （失敗時は例外を投げる）
        /// </summary>
        public static async Task<string> GetStringAsync(string url)
        {
            using var resp = await _client.GetAsync(url).ConfigureAwait(false);

            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}

