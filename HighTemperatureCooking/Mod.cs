using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;

namespace HighTemperatureCooking {
  public sealed class Mod : UserMod2 {
    public static Harmony harmonyInstance;

    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      harmonyInstance = harmony;
      // 初始化 PUtil 的文件
      PUtil.InitLibrary();
      // 检查模组版本是否更新
      new PVersionCheck().Register(this, new SteamVersionChecker());
    }
  }

  public class Patch {
    // [HarmonyPatch(typeof(PrickleFruitConfig), nameof(PrickleFruitConfig.CreatePrefab))]
    public class PrickleFruitConfig_Patch {
      public static void Postfix(GameObject __result) {
        var temperatureCookable = __result.AddOrGet<TemperatureCookable>();
        temperatureCookable.cookTemperature = 344.15f;
        temperatureCookable.cookedID = "GrilledPrickleFruit";
      }
    }

    // [HarmonyPatch(typeof(MeatConfig), nameof(MeatConfig.CreatePrefab))]
    public class MeatConfig_Patch {
      public static void Postfix(GameObject __result) {
        var temperatureCookable = __result.AddOrGet<TemperatureCookable>();
        temperatureCookable.cookTemperature = 344.15f;
        temperatureCookable.cookedID = "CookedMeat";
      }
    }

    // [HarmonyPatch(typeof(MushroomConfig), nameof(MushroomConfig.CreatePrefab))]
    public class MushroomConfig_Patch {
      public static void Postfix(GameObject __result) {
        var temperatureCookable = __result.AddOrGet<TemperatureCookable>();
        temperatureCookable.cookTemperature = 344.15f;
        temperatureCookable.cookedID = "FriedMushroom";
      }
    }

    // [HarmonyPatch(typeof(FishMeatConfig), nameof(FishMeatConfig.CreatePrefab))]
    public class FishMeatConfig_Patch {
      public static void Postfix(GameObject __result) {
        var temperatureCookable = __result.AddOrGet<TemperatureCookable>();
        temperatureCookable.cookTemperature = 344.15f;
        temperatureCookable.cookedID = "CookedFish";
      }
    }

    // [HarmonyPatch(typeof(WormBasicFruitConfig), nameof(WormBasicFruitConfig.CreatePrefab))]
    public class WormBasicFruitConfig_Patch {
      public static void Postfix(GameObject __result) {
        var temperatureCookable = __result.AddOrGet<TemperatureCookable>();
        temperatureCookable.cookTemperature = 344.15f;
        temperatureCookable.cookedID = "WormBasicFood";
      }
    }

    // [HarmonyPatch(typeof(ShellfishMeatConfig), nameof(FishMeatConfig.CreatePrefab))]
    public class ShellfishMeatConfig_Patch {
      public static void Postfix(GameObject __result) {
        var temperatureCookable = __result.AddOrGet<TemperatureCookable>();
        temperatureCookable.cookTemperature = 344.15f;
        temperatureCookable.cookedID = "CookedFish";
      }
    }

    // [HarmonyPatch(typeof(MushBarConfig), nameof(ShellfishMeatConfig.CreatePrefab))]
    public class MushBarConfig_Patch {
      public static void Postfix(GameObject __result) {
        var temperatureCookable = __result.AddOrGet<TemperatureCookable>();
        temperatureCookable.cookTemperature = 344.15f;
        temperatureCookable.cookedID = "FriedMushBar";
      }
    }
  }

  [HarmonyPatch(typeof(EntityConfigManager), nameof(EntityConfigManager.LoadGeneratedEntities))]
  public class ManualPatch {
    public static void Prefix() {
      var targetMethod1 = AccessTools.Method(typeof(MushBarConfig), nameof(ShellfishMeatConfig.CreatePrefab));
      var targetMethod2 = AccessTools.Method(typeof(ShellfishMeatConfig), nameof(FishMeatConfig.CreatePrefab));
      var targetMethod3 = AccessTools.Method(typeof(WormBasicFruitConfig), nameof(WormBasicFruitConfig.CreatePrefab));
      var targetMethod4 = AccessTools.Method(typeof(FishMeatConfig), nameof(FishMeatConfig.CreatePrefab));
      var targetMethod5 = AccessTools.Method(typeof(MushroomConfig), nameof(MushroomConfig.CreatePrefab));
      var targetMethod6 = AccessTools.Method(typeof(MeatConfig), nameof(MeatConfig.CreatePrefab));
      var targetMethod7 = AccessTools.Method(typeof(PrickleFruitConfig), nameof(PrickleFruitConfig.CreatePrefab));
      var postfix1 = AccessTools.Method(typeof(Patch.MushroomConfig_Patch), nameof(Patch.MushroomConfig_Patch.Postfix));
      var postfix2 = AccessTools.Method(typeof(Patch.ShellfishMeatConfig_Patch),
        nameof(Patch.ShellfishMeatConfig_Patch.Postfix));
      var postfix3 = AccessTools.Method(typeof(Patch.WormBasicFruitConfig_Patch),
        nameof(Patch.WormBasicFruitConfig_Patch.Postfix));
      var postfix4 = AccessTools.Method(typeof(Patch.FishMeatConfig_Patch), nameof(Patch.FishMeatConfig_Patch.Postfix));
      var postfix5 = AccessTools.Method(typeof(Patch.MushroomConfig_Patch), nameof(Patch.MushroomConfig_Patch.Postfix));
      var postfix6 = AccessTools.Method(typeof(Patch.MeatConfig_Patch), nameof(Patch.MeatConfig_Patch.Postfix));
      var postfix7 = AccessTools.Method(typeof(Patch.PrickleFruitConfig_Patch),
        nameof(Patch.PrickleFruitConfig_Patch.Postfix));
      Mod.harmonyInstance.Patch(targetMethod1, postfix: new HarmonyMethod(postfix1));
      Mod.harmonyInstance.Patch(targetMethod2, postfix: new HarmonyMethod(postfix2));
      Mod.harmonyInstance.Patch(targetMethod3, postfix: new HarmonyMethod(postfix3));
      Mod.harmonyInstance.Patch(targetMethod4, postfix: new HarmonyMethod(postfix4));
      Mod.harmonyInstance.Patch(targetMethod5, postfix: new HarmonyMethod(postfix5));
      Mod.harmonyInstance.Patch(targetMethod6, postfix: new HarmonyMethod(postfix6));
      Mod.harmonyInstance.Patch(targetMethod7, postfix: new HarmonyMethod(postfix7));
    }
  }
}