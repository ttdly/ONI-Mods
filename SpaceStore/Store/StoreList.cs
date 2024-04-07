using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PeterHan.PLib.Core;
using SpaceStore.MyGeyser;
using SpaceStore.StoreRoboPanel;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SpaceStore.Store
{
    public class StoreList
    {

        public static List<MarketItem> marketItems = new List<MarketItem>();

        public enum ObjectType
        {
            Object,
            Element,
            NeedPack,
        }

        public class MarketItem
        {
            public Sprite sprite;
            public string name = "test";
            public int price;
            public Tag tag;
            public int quantity = 0;
            public CarePackageInfo info;

            public MarketItem(Tag tag, int price = 9999, int count = 1){
                this.tag = tag;
                sprite = Def.GetUISprite(tag).first;
                this.price = price;
                quantity = count;
                name = Assets.GetPrefab(tag).GetProperName();
                info = new CarePackageInfo(tag.ToString(), count, null);
            }

            public string GetDesc() => $"{name}\n{GetSpawnableQuantityOnly()}\n\n.";

            private string GetSpawnableQuantityOnly() {
                if (ElementLoader.GetElement(info.id.ToTag()) != null)
                    return string.Format((string)UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, GameUtil.GetFormattedMass(info.quantity));
                return EdiblesManager.GetFoodInfo(info.id) != null ? string.Format((string)UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, GameUtil.GetFormattedCaloriesForItem((Tag)info.id, info.quantity)) : string.Format((string)UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, info.quantity.ToString());
            }

            public override string ToString()
            {
                return $"\n名称：\t{name}\n标签：\t{tag}\n描述: \t {GetDesc()}";
            }
        }

        public static List<Tuple<string, int>> objects = new List<Tuple<string, int>>() {
            new Tuple<string, int>(RoboPanelConfig.ID,600),
            new Tuple<string, int>(GeoActivatorConfig.ID, 600),
        };

        public static void GetLocalList() {
            string rootDir = StaticVars.LOCAL_FILE_DIR;
            if (!Directory.Exists(rootDir)) { 
                Directory.CreateDirectory(rootDir);
                return;
            }
            string[] filePaths = Directory.GetFiles(rootDir, "store*.json");

            foreach (string filePath in filePaths) {
                ReadListFromAFile(filePath);
            }
        }

        public static void ReadListFromAFile(string path) {
#if DEBUG
            PUtil.LogDebug($"Load {path}");
#endif
            try {
                string json = File.ReadAllText(path);
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject item in jsonArray.Cast<JObject>()) {
                    string id = item["id"].ToString();
                    int quantity = (int)item["quantity"];
                    int price = (int)item["price"];
#if DEBUG
                    PUtil.LogDebug($"id:{id} quantity:{quantity} price:{price}");
#endif
                    marketItems.Add(new MarketItem(tag: new Tag(id), count: quantity, price: price));
                }
            }
            catch (UnauthorizedAccessException e) {
                PUtil.LogExcWarn(e);
            }
            catch (IOException e) {
                PUtil.LogExcWarn(e);
            }
            catch (JsonException e) {
                PUtil.LogExcWarn(e);
            }
        }

        public static void Init()
        {
            foreach (Tuple<string, int> obj in objects)
            {
                marketItems.Add(new MarketItem(obj.first, obj.second));
            }
            GetLocalList();
        }
    }
}
