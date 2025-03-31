using System;
using System.Linq;
using HarmonyLib;
using KMod;
using PackAnything.Movable;
using PackAnything.Movable.Stories;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using UnityEngine;

namespace PackAnything {
  public class Mod : UserMod2 {
    public static Harmony HarmonyInstance;

    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      HarmonyInstance = harmony;
      PUtil.InitLibrary();
      new PVersionCheck().Register(this, new SteamVersionChecker());
      new PLocalization().Register();
      LocString.CreateLocStringKeys(typeof(ModString), "");
#if DEBUG
      ModUtil.RegisterForTranslation(typeof(ModString.Options));
      ModUtil.RegisterForTranslation(typeof(ModString.INFO));
#endif
      new POptions().RegisterOptions(this, typeof(Options));
    }
  }

  [HarmonyPatch(typeof(ModsScreen), "BuildDisplay")]
  public class ModsScreenBuildDisplayPatch {
    public static void Postfix() {
      for (var index = 0; index != Global.Instance.modManager.mods.Count; ++index) {
        var mod = Global.Instance.modManager.mods[index];
        if (mod.staticID != "CalYu.PackAnything") continue;
        if (Strings.TryGet(mod.title, out var result1))
          mod.title = result1;
      }
    }
  }

  [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
  public class PatchBuildings {
    public static void Prefix() {
      GravitiesMovable.PatchBuildings(Mod.HarmonyInstance);
      // ActivateMovable.PatchBuildings(Mod.HarmonyInstance);
      CommonMovable.PatchBuildings(Mod.HarmonyInstance);
      LonelyMinionMovable.PatchBuildings(Mod.HarmonyInstance);
      StoryMovable.PatchBuildings(Mod.HarmonyInstance);
      GravitasCreatureManipulatorMovable.PatchBuildings(Mod.HarmonyInstance);
    }
  }
}