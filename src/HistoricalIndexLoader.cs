using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using HistoricalBordersPlugin.Utils;

namespace HistoricalBordersPlugin
{
    internal static class HistoricalIndexLoader
    {
        private const string IndexUrl =
            "https://raw.githubusercontent.com/aourednik/historical-basemaps/master/index.json";

        private static List<HistoricalIndexEntry>? _entries;

        public static List<HistoricalIndexEntry> Load()
        {
            if (_entries != null) return _entries;

            string json = HttpFetcher.GetStringAsync(IndexUrl).GetAwaiter().GetResult();
            var root = JsonConvert.DeserializeObject<HistoricalIndexRoot>(json);

            _entries = root?.Years ?? new List<HistoricalIndexEntry>();
            return _entries;
        }

        public static HistoricalIndexEntry? GetClosestEntry(int year)
        {
            var list = Load();
            if (list.Count == 0) return null;

            HistoricalIndexEntry? best = null;
            int bestDiff = int.MaxValue;

            foreach (var e in list)
            {
                int diff = Math.Abs(e.Year - year);
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    best = e;
                }
            }
            return best;
        }
    }

    internal class HistoricalIndexRoot
    {
        [JsonProperty("years")]
        public List<HistoricalIndexEntry> Years { get; set; } = new();
    }

    internal class HistoricalIndexEntry
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; } = "";

        [JsonProperty("countries")]
        public List<string> Countries { get; set; } = new();
    }
}
