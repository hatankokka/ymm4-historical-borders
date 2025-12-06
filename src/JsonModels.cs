using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HistoricalBordersPlugin
{
    /// <summary>
    /// index.json の 1項目（year, filename, countries）
    /// </summary>
    public class HistoricalIndexEntry
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; } = "";

        [JsonProperty("countries")]
        public List<string> Countries { get; set; } = new();
    }

    /// <summary>
    /// historical-basemaps の index.json 全体
    /// </summary>
    public class HistoricalIndexRoot
    {
        [JsonProperty("years")]
        public List<HistoricalIndexEntry> Years { get; set; } = new();
    }

    /// <summary>
    /// GeoJSON FeatureCollection
    /// </summary>
    public class GeoJsonFeatureCollection
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        [JsonProperty("features")]
        public List<GeoJsonFeature> Features { get; set; } = new();
    }

    /// <summary>
    /// GeoJSON Feature
    /// </summary>
    public class GeoJsonFeature
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        [JsonProperty("properties")]
        public Dictionary<string, object> Properties { get; set; } = new();

        [JsonProperty("geometry")]
        public GeoJsonGeometry? Geometry { get; set; }
    }

    /// <summary>
    /// GeoJSON Geometry（Polygon / MultiPolygon に対応）
    /// </summary>
    public class GeoJsonGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        [JsonProperty("coordinates")]
        public JToken? Coordinates { get; set; }
    }
}

