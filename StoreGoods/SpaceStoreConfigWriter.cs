using Newtonsoft.Json;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace StoreGoods {
    public class SpaceStoreConfigWriter {
        public static string config_file_path = Path.GetFullPath(Path.Combine(
                KMod.Manager.GetDirectory(),
                "spacestore",
                $"store_{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}_2024_4_13.json"
                ));

        public static bool allow_change = true;

        public class MarketItem{
            public string id;
            public int quantity;
            public int price;
            public MarketItem(string id, int quantity, int price) {
                this.id = id;
                this.quantity = quantity;
                this.price = price;
            }
        }

        public static void Write() {
            List<MarketItem> list = new List<MarketItem>() { 
                new MarketItem(GeoActivatorConfig.ID, 1, 600),
            };

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            if (Detect(json)) {
                File.WriteAllText(config_file_path, json);
            }
        }

        public static bool Detect(string now) {
            if (!File.Exists(config_file_path)) {
                return true;
            }
            if (allow_change) return false;
            string now_hash = GetMd5Hash(now);
            string old = File.ReadAllText(config_file_path);
            string old_hash = GetMd5Hash(old);
#if DEBUG
            PUtil.LogDebug($"{now_hash}--{old_hash}");
#endif
            return now_hash == old_hash;
        }

        private static string GetMd5Hash(string input) {
            using (MD5 md5 = MD5.Create()) {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
