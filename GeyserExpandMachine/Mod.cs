using GeyserExpandMachine.Screen;
using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;

namespace GeyserExpandMachine {
  public class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      PUtil.InitLibrary();
      new PVersionCheck().Register(this, new SteamVersionChecker());
      ModAssets.LoadAssets();
#if DEBUG
      Localization.RegisterForTranslation(typeof(ModString));
#endif
    }
  }
}