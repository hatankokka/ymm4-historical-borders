using YukkuriMovieMaker.Plugin;

namespace HistoricalBordersPlugin
{
    /// <summary>
    /// YMM4 に「歴史的国境図形」を登録するプラグイン本体
    /// </summary>
    public class BorderHistoryPlugin : IShapePlugin
    {
        // プラグインの名前（YMM4 の UI に表示される）
        public string DisplayName => "Historical Borders (歴史的国境)";

        // プラグインの説明（設定 → プラグイン一覧に表示）
        public string Description =>
            "GitHub historical-basemaps から国境データを取得し、年代別に描画する YMM4 用図形プラグイン。";

        // 図形パラメータ（プロパティウィンドウで表示される）
        public IShapeParameter CreateShapeParameter()
        {
            return new HistoricalBordersParameter();
        }

        // 描画処理を実行するクラス
        public IShapeSource CreateShapeSource(IShapeParameter parameter)
        {
            return new HistoricalBordersSource((HistoricalBordersParameter)parameter);
        }
    }
}

