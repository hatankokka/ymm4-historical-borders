using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project;

namespace HistoricalBordersPlugin
{
    internal class HistoricalBordersParameter(SharedDataStore? sharedData) : ShapeParameterBase(sharedData)
    {
        // ■ 年代（スライダー）
        [Display(GroupName = "歴史国境", Name = "表示する年")]
        [AnimationSlider("F0", "年", -50000, 2100)]
        public Animation Year { get; } = new Animation(1900, -50000, 2100);

        // ■ 国名フィルタ（部分一致）
        [Display(GroupName = "歴史国境", Name = "国名フィルタ（部分一致）")]
        public string CountryFilter
        {
            get => countryFilter;
            set => Set(ref countryFilter, value);
        }
        private string countryFilter = "";

        // ■ 境界線の色
        [Display(GroupName = "歴史国境", Name = "境界線の色")]
        public Color4 BorderColor
        {
            get => borderColor;
            set => Set(ref borderColor, value);
        }
        private Color4 borderColor = new Color4(1, 1, 1, 1);

        // ■ 塗りつぶし ON/OFF
        [Display(GroupName = "歴史国境", Name = "塗りつぶしを有効にする")]
        public bool FillEnabled
        {
            get => fillEnabled;
            set => Set(ref fillEnabled, value);
        }
        private bool fillEnabled = false;

        // ■ 塗りつぶし色
        [Display(GroupName = "歴史国境", Name = "塗りつぶし色")]
        public Color4 FillColor
        {
            get => fillColor;
            set => Set(ref fillColor, value);
        }
        private Color4 fillColor = new Color4(0, 0, 1, 0.4f);

        // ■ Glow 強度
        [Display(GroupName = "歴史国境", Name = "光彩（Glow）強度")]
        [AnimationSlider("F1", "", 0, 50)]
        public Animation Glow { get; } = new Animation(0, 0, 50);

        public HistoricalBordersParameter() : this(null) { }

        public override IShapeSource CreateShapeSource(IGraphicsDevicesAndContext devices)
        {
            return new HistoricalBordersSource(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables()
        {
            // Year と Glow はアニメーション対応
            yield return Year;
            yield return Glow;
        }

        // データ保存
        protected override void LoadSharedData(SharedDataStore store)
        {
            var data = store.Load<SharedData>();
            if (data is null) return;
            data.CopyTo(this);
        }

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new SharedData(this));
        }

        public class SharedData(HistoricalBordersParameter p)
        {
            public double Year { get; } = p.Year.Values[0].Value;
            public string CountryFilter { get; } = p.CountryFilter;
            public Color4 BorderColor { get; } = p.BorderColor;
            public bool FillEnabled { get; } = p.FillEnabled;
            public Color4 FillColor { get; } = p.FillColor;
            public double Glow { get; } = p.Glow.Values[0].Value;

            public void CopyTo(HistoricalBordersParameter p)
            {
                p.Year.Values[0].Value = Year;
                p.CountryFilter = CountryFilter;
                p.BorderColor = BorderColor;
                p.FillEnabled = FillEnabled;
                p.FillColor = FillColor;
                p.Glow.Values[0].Value = Glow;
            }
        }
    }
}

