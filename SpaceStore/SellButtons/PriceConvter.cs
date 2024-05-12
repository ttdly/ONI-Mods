using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpaceStore.Store;
using System.Reflection;

namespace SpaceStore.SellButtons {
    public class PriceConvter {
        public static PriceConvter Instance;
        public static bool Dirty = true;
        public Dictionary<Tag, float> sellItems = new Dictionary<Tag, float>() {};
        
        public PriceConvter() {
            Instance = this;
            GetLocalList();
            if (Instance.sellItems.Count == 0 ) { 
                LoadFromAssembly();
            }
        }

        public static void GetLocalList() {
            string rootDir = StaticVars.LOCAL_FILE_DIR;
            if (!Directory.Exists(rootDir)) {
                Directory.CreateDirectory(rootDir);
                return;
            }
            string[] filePaths = Directory.GetFiles(rootDir, "sell*.json");

            foreach (string filePath in filePaths) {
                ReadListFromAFile(filePath);
            }
        }

        public static void LoadFromAssembly() {
            try {
                Assembly asm = Assembly.GetAssembly(typeof(StoreList));
                Stream stream = asm.GetManifestResourceStream("SpaceStore.config.sell.json");
                string json = "";
                using (StreamReader reader = new StreamReader(stream)) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        json += line;
                    }
                }
                PraseJson(json);
            }
            catch (Exception ex) {
                PUtil.LogError("错误： " + ex.Message);
            }
        }

        public static void ReadListFromAFile(string path) {
            try {
                string json = File.ReadAllText(path);
                PraseJson(json);
            }
            catch (UnauthorizedAccessException e) {
                PUtil.LogExcWarn(e);
            }
        }

        public static void PraseJson(string json) {
            try {
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject item in jsonArray.Cast<JObject>()) {
                    string id = item["id"].ToString();
                    float price = (float)item["price"];
                    Instance.sellItems.Add(new Tag(id), price);
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
    }
}
