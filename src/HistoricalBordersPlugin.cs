using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace HistoricalBordersPlugin
{
    internal class HistoricalBordersPlugin : IShapePlugin
    {
        public bool IsExoShapeSupported => false;
        public bool IsExoMaskSupported => false;

        public string Name => "歴史国境（Historical Borders）";

        public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData)
        {
            return new HistoricalBordersParameter(sharedData);
        }
    }
}

