using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;

namespace TweaksPack {
  public class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      PUtil.InitLibrary();
      new PVersionCheck().Register(this, new SteamVersionChecker());
      new PLocalization().Register();
      TweaksPackPatches.ManualPatchBuildings(harmony);
    }
  }
}