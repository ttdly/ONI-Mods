using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;

namespace GravitasMemory {
  public sealed class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      PUtil.InitLibrary();
      new PVersionCheck().Register(this, new SteamVersionChecker());
    }
  }
}