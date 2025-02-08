using HarmonyLib;
using KMod;
using PackAnything.Movable;
using PackAnything.Movable.Stories;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;

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
#endif
      new POptions().RegisterOptions(this, typeof(Options));
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