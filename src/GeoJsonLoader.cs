using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HistoricalBordersPlugin.Utils;

namespace HistoricalBordersPlugin
{
    public static class HistoricalIndexLoader
    {
        private const string IndexUrl =
            "https://raw.githubusercontent.com/aourednik/historical-basemaps/master/index.json";

        private static List<HistoricalIndexEntry>? _entries;

        /// <summary>
        /// 年代に最も近いデータを返す
        /// </summary>
        public static HistoricalIndexEntry? GetClosestEntry(int year)
        {
            EnsureLoaded();

            if (_entries == null || _entries.Count == 0)
                return null;

            // 差の絶対値が最小のものを返す
            return _entries
                .OrderBy(e => Math.Abs(e.Year - year))
                .FirstOrDefault();
        }

        private static void EnsureLoaded()
        {
            if (_entries != null) return;

            // キャッシュ利用
            string json;
            if (LocalCache.TryReadCache(IndexUrl, out var cached) && !string.IsNullOrEmpty(cached))
            {
                json = cached!;
            }
            else
            {
                json = HttpFetcher.GetStringAsync(IndexUrl).GetAwaiter().GetResult();
                LocalCache.WriteCacheAsync(IndexUrl, json).GetAwaiter().GetResult();
            }

            var root = JObject.Parse(json);
            var arr = (JArray)root["years"]!;

            _entries = arr
                .Select(item => new HistoricalIndexEntry
                {
                    Year = item["year"]!.Value<int>(),
                    Filename = item["filename"]!.Value<string>()!,
                    Countries = item["countries"]!.Select(c => c.Value<string>()!).ToList()
                })
                .ToList();
        }
    }

    public class HistoricalIndexEntry
    {
        public int Year { get; set; }
        public string Filename { get; set; } = "";
        public List<string> Countries { get; set; } = new();
    }
}
