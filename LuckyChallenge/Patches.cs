using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Klei.CustomSettings;
using ProcGen;

namespace LuckyChallenge {
  public class Patches {
    public static SettingConfig LuckyChallenge = new ToggleSettingConfig(nameof(LuckyChallenge), STRINGS.UI.CONFIG.NAME,
      STRINGS.UI.CONFIG.DESC, new SettingLevel("Disabled", STRINGS.UI.CONFIG.DISABLED, STRINGS.UI.CONFIG.DISABLED_TIP),
      new SettingLevel("Enabled", STRINGS.UI.CONFIG.ENABLE, STRINGS.UI.CONFIG.ENABLE_TIP), "Disabled", "Disabled");

    public static Harmony harmony = new Harmony("com.ttdlyu.mod");

    public static void ApplyDig(int cell, ref float mass) {
      var go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)GiftConfig.ID), Grid.CellToPos(cell), Grid.SceneLayer.Move);
      go.SetActive(true);
      mass = 0f;
    }

    [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
    public class Db_Initialize_Patch {
      public static void Postfix() {
        LocString.CreateLocStringKeys(typeof(STRINGS), "");
      }
    }

    [HarmonyPatch(typeof(CustomGameSettings), "SetSurvivalDefaults")]
    public class CustomGameSettings_OnPrefabInit_Patch {
      public static void Postfix() {
        CustomGameSettings.Instance.AddQualitySettingConfig(LuckyChallenge);
      }
    }

    [HarmonyPatch(typeof(MutatedWorldData), MethodType.Constructor, typeof(ProcGen.World),
      typeof(List<WorldTrait>), typeof(List<WorldTrait>))]
    public class MutatedWorldData_Constructor_Patch {
      public static void Postfix(MutatedWorldData __instance) {
        var world = __instance.world;
        var subworlds = __instance.subworlds;
        if (CustomGameSettings.Instance.GetCurrentQualitySetting(LuckyChallenge).id == "Enabled") {
          world.worldTemplateRules?.Clear();
          if (subworlds != null)
            foreach (var subworld in subworlds)
              subworld.Value.subworldTemplateRules?.Clear();
        }
      }
    }


    [HarmonyPatch(typeof(CustomGameSettings), nameof(CustomGameSettings.Print))]
    public class Game_LoadSettings_Patch {
      public static void Postfix() {
        var fieldInfo = typeof(CustomGameSettings).GetField("CurrentQualityLevelsBySetting",
          BindingFlags.NonPublic | BindingFlags.Instance);
        var qualityLevels = (Dictionary<string, string>)fieldInfo.GetValue(CustomGameSettings.Instance);
        string value;
        var hasConfig = qualityLevels.TryGetValue("LuckyChallenge", out value);
        if (hasConfig && value == "Enabled")
          harmony.Patch(typeof(WorldDamage).GetMethod(nameof(WorldDamage.OnDigComplete)),
            new HarmonyMethod(typeof(Patches).GetMethod(nameof(ApplyDig))));
      }
    }

    //[HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.CreatePrefab))]
    //public class MinionConfig_CreatePrefab_Patch {
    //    public static void Postfix(GameObject __result) {
    //        __result.AddComponent<MinionGift>();
    //    }
    //}
  }
}