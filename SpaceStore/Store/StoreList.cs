using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PeterHan.PLib.Core;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
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
            public Tuple<Sprite, Color> sprite;
            public string name = "test";
            public int price;
            public Tag tag;
            public int quantity = 0;
            public CarePackageInfo info;

            public MarketItem(Tag tag, int price = 9999, int count = 1){
                GameObject go = Assets.GetPrefab(tag) ?? throw new Exception($"Tag {tag} not exist");
                this.tag = tag;
                sprite = Def.GetUISprite(tag);
                this.price = price;
                quantity = count;
                name = go.GetProperName();
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
                    AddOneItem(new Tag(id), price, quantity);
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

        public static void AddOneItem(Tag tag, int price, int quantity) {
            try {
                MarketItem marketItem = new MarketItem(tag, price, quantity);
                if (price > 0 && quantity > 0) {
                    marketItems.Add(marketItem);
                } else {
                    PUtil.LogWarning($"{tag} price or quantity can not less than zero!");
                }
                
            }
            catch (Exception) {
                PUtil.LogWarning($"{tag} not exist");
            }
        }

        public static void OpenConfigFolder() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                PUtil.LogDebug(StaticVars.LOCAL_FILE_DIR);
                Process.Start(new ProcessStartInfo("explorer", $"\"{StaticVars.LOCAL_FILE_DIR}\"") { UseShellExecute = true });
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                Process.Start("open", StaticVars.LOCAL_FILE_DIR);
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                Process.Start("xdg-open", StaticVars.LOCAL_FILE_DIR);
            } else {
                PUtil.LogError("Unsupport platform");
            }
        }

        public static void OpenWebPage() {
            try {
                ProcessStartInfo psi = new ProcessStartInfo {
                    FileName = "https://spacestore-config-editor.pages.dev/",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex) {
                // 处理可能发生的异常
                Console.WriteLine("无法打开浏览器: " + ex.Message);
            }
        }

        public static void Init(){
            GetLocalList();
            if (marketItems.Count == 0) {
                marketItems.Add(new MarketItem(SimHashes.Water.CreateTag(), 1000, 200));
            }
        }
    }
}
