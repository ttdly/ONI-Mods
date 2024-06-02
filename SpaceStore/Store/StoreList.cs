using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PeterHan.PLib.Core;
using STRINGS;
using UnityEngine;

namespace SpaceStore.Store {
  public class StoreList {
    public enum ObjectType {
      Object,
      Element,
      NeedPack
    }

    public static List<MarketItem> marketItems = new List<MarketItem>();

    public static void GetLocalList() {
      var rootDir = StaticVars.LOCAL_FILE_DIR;
      if (!Directory.Exists(rootDir)) {
        Directory.CreateDirectory(rootDir);
        return;
      }

      var filePaths = Directory.GetFiles(rootDir, "store*.json");

      foreach (var filePath in filePaths) ReadListFromAFile(filePath);
    }

    public static void LoadFromAssembly() {
      try {
        var asm = Assembly.GetAssembly(typeof(StoreList));
        var stream = asm.GetManifestResourceStream("SpaceStore.config.store.json");
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
          var quantity = (int)item["quantity"];
          var price = (int)item["price"];
          AddOneItem(new Tag(id), price, quantity);
        }
      } catch (UnauthorizedAccessException e) {
        PUtil.LogExcWarn(e);
      } catch (IOException e) {
        PUtil.LogExcWarn(e);
      } catch (JsonException e) {
        PUtil.LogExcWarn(e);
      }
    }

    public static void AddOneItem(Tag tag, int price, int quantity) {
      try {
        var marketItem = new MarketItem(tag, price, quantity);
        if (price > 0 && quantity > 0)
          marketItems.Add(marketItem);
        else
          PUtil.LogWarning($"{tag} price or quantity can not less than zero!");
      } catch (Exception) {
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
        var psi = new ProcessStartInfo {
          FileName = "https://spacestore-config-editor.pages.dev/",
          UseShellExecute = true
        };
        Process.Start(psi);
      } catch (Exception ex) {
        // 处理可能发生的异常
        Console.WriteLine("无法打开浏览器: " + ex.Message);
      }
    }

    public static void Init() {
      GetLocalList();
      if (marketItems.Count == 0) LoadFromAssembly();
    }

    public class MarketItem {
      public CarePackageInfo info;
      public string name = "test";
      public int price;
      public int quantity;
      public Tuple<Sprite, Color> sprite;
      public Tag tag;

      public MarketItem(Tag tag, int price = 9999, int count = 1) {
        var go = Assets.GetPrefab(tag) ?? throw new Exception($"Tag {tag} not exist");
        this.tag = tag;
        sprite = Def.GetUISprite(tag);
        this.price = price;
        quantity = count;
        name = go.GetProperName();
        info = new CarePackageInfo(tag.ToString(), count, null);
      }

      public string GetDesc() {
        return $"{name}\n{GetSpawnableQuantityOnly()}\n\n.";
      }

      private string GetSpawnableQuantityOnly() {
        if (ElementLoader.GetElement(info.id.ToTag()) != null)
          return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY,
            GameUtil.GetFormattedMass(info.quantity));
        return EdiblesManager.GetFoodInfo(info.id) != null
          ? string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY,
            GameUtil.GetFormattedCaloriesForItem((Tag)info.id, info.quantity))
          : string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, info.quantity.ToString());
      }

      public override string ToString() {
        return $"\n名称：\t{name}\n标签：\t{tag}\n描述: \t {GetDesc()}";
      }
    }
  }
}