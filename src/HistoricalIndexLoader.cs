// src/HistoricalIndexLoader.cs
using System;
using System.Linq;
using Newtonsoft.Json;
using HistoricalBordersPlugin.Utils; // HttpFetcher / LocalCache の namespace に合わせて

namespace HistoricalBordersPlugin
{
    internal static class HistoricalIndexLoader
    {
        private const string IndexUrl =
            "https://raw.githubusercontent.com/aourednik/historical-basemaps/master/index.json";

        private static readonly object _lock = new();
        private static HistoricalIndex? _index;

        public static HistoricalIndex Index
        {
            get
            {
                EnsureLoaded();
                return _index!;
            }
        }

        private static void EnsureLoaded()
        {
            if (_index != null) return;

            lock (_lock)
            {
                if (_index != null) return;

                string json;
                if (LocalCache.TryReadCache(IndexUrl, out var cached) && !string.IsNullOrWhiteSpace(cached))
                {
                    json = cached!;
                }
                else
                {
                    // 同期的に取得（プラグイン側から await しないため）
                    json = Utils.HttpFetcher.GetStringAsync(IndexUrl).GetAwaiter().GetResult();
                    LocalCache.WriteCacheAsync(IndexUrl, json).GetAwaiter().GetResult();
                }

                _index = JsonConvert.DeserializeObject<HistoricalIndex>(json) ?? new HistoricalIndex();
                _index.Years = _index.Years.OrderBy(y => y.Year).ToList();
            }
        }

        public static HistoricalYearEntry? GetClosestEntry(int year)
        {
            var idx = Index;
            return idx.Years.OrderBy(y => Math.Abs(y.Year - year)).FirstOrDefault();
        }
    }
}

