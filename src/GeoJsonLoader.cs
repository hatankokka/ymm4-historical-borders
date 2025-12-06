// src/GeoJsonLoader.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HistoricalBordersPlugin.Utils;

namespace HistoricalBordersPlugin
{
    internal static class GeoJsonLoader
    {
        private const string BaseUrl =
            "https://raw.githubusercontent.com/aourednik/historical-basemaps/master/geojson/";

        // filename + filter をキーにキャッシュ
        private static readonly Dictionary<string, List<Vector2[]>> _cache = new(StringComparer.OrdinalIgnoreCase);

        public static List<Vector2[]> GetPolygons(string filename, string? countryFilter)
        {
            var key = $"{filename}::{countryFilter ?? ""}";
            lock (_cache)
            {
                if (_cache.TryGetValue(key, out var cached))
                    return cached;
            }

            var url = BaseUrl + filename;
            string json;
            if (LocalCache.TryReadCache(url, out var cachedJson) && !string.IsNullOrWhiteSpace(cachedJson))
            {
                json = cachedJson!;
            }
            else
            {
                json = HttpFetcher.GetStringAsync(url).GetAwaiter().GetResult();
                LocalCache.WriteCacheAsync(url, json).GetAwaiter().GetResult();
            }

            var fc = JsonConvert.DeserializeObject<GeoJsonFeatureCollection>(json);
            var result = new List<Vector2[]>();

            if (fc != null)
            {
                foreach (var feature in fc.Features)
                {
                    // 国名フィルタ
                    if (!string.IsNullOrWhiteSpace(countryFilter) &&
                        feature.Properties.TryGetValue("NAME", out var nameObj))
                    {
                        var name = Convert.ToString(nameObj);
                        if (string.IsNullOrEmpty(name) ||
                            !name.Contains(countryFilter, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }

                    if (feature.Geometry?.Coordinates is not JToken coords)
                        continue;

                    switch (feature.Geometry.Type)
                    {
                        case "Polygon":
                            ExtractPolygon(coords, result);
                            break;
                        case "MultiPolygon":
                            foreach (var poly in coords)
                                ExtractPolygon(poly, result);
                            break;
                    }
                }
            }

            lock (_cache)
            {
                _cache[key] = result;
            }

            return result;
        }

        private static void ExtractPolygon(JToken coords, List<Vector2[]> dest)
        {
            // Polygon: [ [ [lon,lat], ... ] , [hole] ... ]
            var outerRing = coords.First; // とりあえず外周だけ
            if (outerRing is not JArray arr) return;

            var pts = new List<Vector2>();
            foreach (var pt in arr)
            {
                if (pt is not JArray xy || xy.Count < 2) continue;
                var lon = xy[0]!.Value<double>();
                var lat = xy[1]!.Value<double>();
                pts.Add(new Vector2((float)lon, (float)lat));
            }
            if (pts.Count > 2)
                dest.Add(pts.ToArray());
        }
    }
}

