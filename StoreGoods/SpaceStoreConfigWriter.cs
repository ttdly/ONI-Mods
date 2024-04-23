using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace StoreGoods {
    public class SpaceStoreConfigWriter {

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
            string filePath = Path.GetFullPath(Path.Combine(
                KMod.Manager.GetDirectory(),
                "spacestore",
                $"store_{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}_2024_4_13.json"
                ));
            List<MarketItem> list = new List<MarketItem>() { 
                new MarketItem(GeoActivatorConfig.ID, 1, 800),
                new MarketItem(RoboPanelConfig.ID, 1, 600),
            };

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
