using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PeterHan.PLib.Core;
using SpaceStore.Store;

namespace SpaceStore.SellButtons {
  public class PriceConvter {
    public static PriceConvter Instance;
    public static bool Dirty = true;
    public Dictionary<Tag, float> sellItems = new Dictionary<Tag, float>();

    public PriceConvter() {
      Instance = this;
      GetLocalList();
      if (Instance.sellItems.Count == 0) LoadFromAssembly();
    }

    public static void GetLocalList() {
      var rootDir = StaticVars.LOCAL_FILE_DIR;
      if (!Directory.Exists(rootDir)) {
        Directory.CreateDirectory(rootDir);
        return;
      }

      var filePaths = Directory.GetFiles(rootDir, "sell*.json");

      foreach (var filePath in filePaths) ReadListFromAFile(filePath);
    }

    public static void LoadFromAssembly() {
      try {
        var asm = Assembly.GetAssembly(typeof(StoreList));
        var stream = asm.GetManifestResourceStream("SpaceStore.config.sell.json");
        var json = "";
        using (var reader = new StreamReader(stream)) {
          string line;
          while ((line = reader.ReadLine()) != null) json += line;
        }

        PraseJson(json);
      } catch (Exception ex) {
        PUtil.LogError("错误： " + ex.Message);
      }
    }

    public static void ReadListFromAFile(string path) {
      try {
        var json = File.ReadAllText(path);
        PraseJson(json);
      } catch (UnauthorizedAccessException e) {
        PUtil.LogExcWarn(e);
      }
    }

    public static void PraseJson(string json) {
      try {
        var jsonArray = JArray.Parse(json);
        foreach (var item in jsonArray.Cast<JObject>()) {
          var id = item["id"].ToString();
          var price = (float)item["price"];
          Instance.sellItems.Add(new Tag(id), price);
        }
      } catch (UnauthorizedAccessException e) {
        PUtil.LogExcWarn(e);
      } catch (IOException e) {
        PUtil.LogExcWarn(e);
      } catch (JsonException e) {
        PUtil.LogExcWarn(e);
      }
    }
  }
}