using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player;
using YukkuriMovieMaker.Plugin;

namespace HistoricalBordersPlugin
{
    public class HistoricalBordersSource : IShapeSource
    {
        private readonly HistoricalBordersParameter _parameter;

        public HistoricalBordersSource(HistoricalBordersParameter parameter)
        {
            _parameter = parameter;
        }

        public IReadOnlyList<EditableVertex[]> BuildVertices(RenderInfo info)
        {
            int year = (int)_parameter.Year.CurrentValue;
            string? filter = _parameter.CountryFilter;

            var entry = HistoricalIndexLoader.GetClosestEntry(year);
            if (entry == null)
                return Array.Empty<EditableVertex[]>();

            var polygons = GeoJsonLoader.GetPolygons(entry.Filename, filter);
            if (polygons.Count == 0)
                return Array.Empty<EditableVertex[]>();

            List<EditableVertex[]> list = new();

            foreach (var ring in polygons)
            {
                var verts = ring
                    .Select(v => ConvertLatLonToLocal(v, info))
                    .Select(pt => new EditableVertex(pt))
                    .ToArray();

                list.Add(verts);
            }

            return list;
        }

        /// <summary>
        /// WGS84 座標（lon,lat）を YMM4 のローカル座標（-1〜1）へマッピングする
        /// </summary>
        private static Vector2 ConvertLatLonToLocal(Vector2 lonlat, RenderInfo info)
        {
            float x = lonlat.X / 180f;   // -180〜180 → -1〜1
            float y = lonlat.Y / 90f;    // -90〜90   → -1〜1

            // 画角補正（Shape 内で縦横比が違う）
            float ar = info.AspectRatio;
            if (ar > 1f) x /= ar;
            else y *= ar;

            return new Vector2(x, -y); // Y 軸反転
        }

        // 境界線色・塗り潰しなど YMM4 が自動処理するパラメータ
        public bool CanFill => true;
        public bool CanStroke => true;
    }
}
