using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;

namespace HighTemperatureCooking {
    public sealed class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            // 初始化 PUtil 的文件
            PUtil.InitLibrary();
            // 检查模组版本是否更新
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }
    }

    public class Patch {
        [HarmonyPatch(typeof(PrickleFruitConfig), nameof(PrickleFruitConfig.CreatePrefab))]
        public class PrickleFruitConfig_Patch {
            public static void Postfix(GameObject __result) {
                TemperatureCookable temperatureCookable = __result.AddOrGet<TemperatureCookable>();
                temperatureCookable.cookTemperature = 344.15f;
                temperatureCookable.cookedID = "GrilledPrickleFruit";
            }
        }

        [HarmonyPatch(typeof(MeatConfig), nameof(MeatConfig.CreatePrefab))]
        public class MeatConfig_Patch {
            public static void Postfix(GameObject __result) {
                TemperatureCookable temperatureCookable = __result.AddOrGet<TemperatureCookable>();
                temperatureCookable.cookTemperature = 344.15f;
                temperatureCookable.cookedID = "CookedMeat";
            }
        }

        [HarmonyPatch(typeof(MushroomConfig), nameof(MushroomConfig.CreatePrefab))]
        public class MushroomConfig_Patch {
            public static void Postfix(GameObject __result) {
                TemperatureCookable temperatureCookable = __result.AddOrGet<TemperatureCookable>();
                temperatureCookable.cookTemperature = 344.15f;
                temperatureCookable.cookedID = "FriedMushroom";
            }
        }

        [HarmonyPatch(typeof(FishMeatConfig), nameof(FishMeatConfig.CreatePrefab))]
        public class FishMeatConfig_Patch {
            public static void Postfix(GameObject __result) {
                TemperatureCookable temperatureCookable = __result.AddOrGet<TemperatureCookable>();
                temperatureCookable.cookTemperature = 344.15f;
                temperatureCookable.cookedID = "CookedFish";
            }
        }

        [HarmonyPatch(typeof(WormBasicFruitConfig), nameof(WormBasicFruitConfig.CreatePrefab))]
        public class WormBasicFruitConfig_Patch {
            public static void Postfix(GameObject __result) {
                TemperatureCookable temperatureCookable = __result.AddOrGet<TemperatureCookable>();
                temperatureCookable.cookTemperature = 344.15f;
                temperatureCookable.cookedID = "WormBasicFood";
            }
        }

        [HarmonyPatch(typeof(ShellfishMeatConfig), nameof(FishMeatConfig.CreatePrefab))]
        public class ShellfishMeatConfig_Patch {
            public static void Postfix(GameObject __result) {
                TemperatureCookable temperatureCookable = __result.AddOrGet<TemperatureCookable>();
                temperatureCookable.cookTemperature = 344.15f;
                temperatureCookable.cookedID = "CookedFish";
            }
        }

        [HarmonyPatch(typeof(MushBarConfig), nameof(ShellfishMeatConfig.CreatePrefab))]
        public class MushBarConfig_Patch {
            public static void Postfix(GameObject __result) {
                TemperatureCookable temperatureCookable = __result.AddOrGet<TemperatureCookable>();
                temperatureCookable.cookTemperature = 344.15f;
                temperatureCookable.cookedID = "FriedMushBar";
            }
        }
    }
}
