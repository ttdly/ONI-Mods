using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using KMod;
using Newtonsoft.Json;

namespace StoreGoods {
  public class SpaceStoreConfigWriter {
    public static string config_file_path = Path.GetFullPath(Path.Combine(
      Manager.GetDirectory(),
      "spacestore",
      $"store_{MethodBase.GetCurrentMethod().DeclaringType.FullName}_2024_4_13.json"
    ));

    public static bool allow_change = true;

    public static void Write() {
      var list = new List<MarketItem> {
        new MarketItem(GeoActivatorConfig.ID, 1, 600)
      };

      var json = JsonConvert.SerializeObject(list, Formatting.Indented);
      if (Detect(json)) File.WriteAllText(config_file_path, json);
    }

    public static bool Detect(string now) {
      if (!File.Exists(config_file_path)) return true;
      if (allow_change) return false;
      var now_hash = GetMd5Hash(now);
      var old = File.ReadAllText(config_file_path);
      var old_hash = GetMd5Hash(old);
#if DEBUG
            PUtil.LogDebug($"{now_hash}--{old_hash}");
#endif
      return now_hash == old_hash;
    }

    private static string GetMd5Hash(string input) {
      using (var md5 = MD5.Create()) {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);

        var sb = new StringBuilder();
        for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("X2"));
        return sb.ToString();
      }
    }

    public class MarketItem {
      public string id;
      public int price;
      public int quantity;

      public MarketItem(string id, int quantity, int price) {
        this.id = id;
        this.quantity = quantity;
        this.price = price;
      }
    }
  }
}