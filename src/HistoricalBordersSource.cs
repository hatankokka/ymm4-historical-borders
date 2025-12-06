using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace HistoricalBordersPlugin
{
    internal class HistoricalBordersSource : IShapeSource, IDisposable
    {
        private readonly IGraphicsDevicesAndContext devices;
        private readonly HistoricalBordersParameter parameter;

        // Path キャッシュ（年ごと）
        private readonly Dictionary<int, IGeometry> geometryCache = new();

        public HistoricalBordersSource(IGraphicsDevicesAndContext devices, HistoricalBordersParameter parameter)
        {
            this.devices = devices;
            this.parameter = parameter;
        }

        public void Dispose()
        {
            foreach (var g in geometryCache.Values)
                g.Dispose();
            geometryCache.Clear();
        }

        public void Draw(IShapeRenderContext context)
        {
            int year = (int)parameter.Year.CurrentValue;
            double glow = parameter.Glow.CurrentValue;

            // ■ 対象年の GeoJSON Path を取得
            var geo = GetOrLoadGeometry(year, parameter.CountryFilter);

            if (geo == null)
                return;

            // ■ 塗りつぶし
            if (parameter.FillEnabled)
            {
                using var fillBrush = devices.CreateBrush(parameter.FillColor);
                context.DrawingContext.FillGeometry(geo, fillBrush);
            }

            // ■ Glow（外側に太い線）
            if (glow > 0)
            {
                using var glowBrush = devices.CreateBrush(new Color4(
                    parameter.BorderColor.R,
                    parameter.BorderColor.G,
                    parameter.BorderColor.B,
                    0.4f
                ));

                context.DrawingContext.DrawGeometry(
                    geo,
                    glowBrush,
                    (float)(2f + glow * 0.15f)
                );
            }

            // ■ 境界線
            using var borderBrush = devices.CreateBrush(parameter.BorderColor);
            context.DrawingContext.DrawGeometry(
                geo,
                borderBrush,
                2.0f
            );
        }

        /// <summary>
        /// 描画用 PathGeometry を取得 or ロード
        /// </summary>
        private IGeometry? GetOrLoadGeometry(int year, string filter)
        {
            if (geometryCache.TryGetValue(year, out var cached))
                return cached;

            // GeoJSON 取得
            var index = HistoricalIndexLoader.Index;
            if (index == null)
                return null;

            var entry = index.Years.OrderBy(y => Math.Abs(y.Year - year)).FirstOrDefault();
            if (entry == null)
                return null;

            var geojson = HistoricalIndexLoader.GetGeoJson(entry.Filename, filter);
            if (geojson == null || geojson.Count == 0)
                return null;

            // PathGeometry に変換
            var geo = devices.CreatePathGeometry();

            foreach (var poly in geojson)
            {
                geo.BeginFigure(poly[0], false);

                for (int i = 1; i < poly.Count; i++)
                    geo.AddLine(poly[i]);

                geo.EndFigure(true);
            }

            geometryCache[year] = geo;
            return geo;
        }
    }
}

