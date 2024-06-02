using HarmonyLib;
using KMod;
using PeterHan.PLib.Database;
using UnityEngine;

namespace LuckyChallenge {
  public class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      new PLocalization().Register();
      ModUtil.RegisterForTranslation(typeof(STRINGS));
    }
  }

  [HarmonyPatch(typeof(FarmTileConfig), nameof(FarmTileConfig.DoPostConfigureComplete))]
  public class Ppppp {
    public static void Postfix(GameObject go) {
      go.AddComponent<Manual>();
    }
  }
}