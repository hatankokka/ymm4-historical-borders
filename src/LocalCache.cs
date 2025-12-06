using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HistoricalBordersPlugin.Utils
{
    internal static class LocalCache
    {
        // %LOCALAPPDATA%\HistoricalBordersPlugin\cache
        private static readonly string CacheDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "HistoricalBordersPlugin", "cache");

        static LocalCache()
        {
            try
            {
                if (!Directory.Exists(CacheDir))
                    Directory.CreateDirectory(CacheDir);
            }
            catch
            {
                // directory 作成失敗時はキャッシュ無効
            }
        }

        /// <summary>
        /// キャッシュファイルパス（URL → SHA1 で変換）
        /// </summary>
        private static string GetCachePath(string url)
        {
            using var sha1 = SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(url));
            var hash = BitConverter.ToString(bytes).Replace("-", "");
            return Path.Combine(CacheDir, hash + ".json");
        }

        /// <summary>
        /// キャッシュを読み込む（存在すれば true）
        /// </summary>
        public static bool TryReadCache(string url, out string? text)
        {
            text = null;
            try
            {
                var path = GetCachePath(url);
                if (!File.Exists(path)) return false;

                text = File.ReadAllText(path, Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// キャッシュを書き込む
        /// </summary>
        public static async Task WriteCacheAsync(string url, string text)
        {
            try
            {
                var path = GetCachePath(url);
                await File.WriteAllTextAsync(path, text, Encoding.UTF8);
            }
            catch
            {
                // 書き込み失敗は無視
            }
        }
    }
}

