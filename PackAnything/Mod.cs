using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;

namespace PackAnything {
  public class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      // TODO 故事特质建筑移动需要同步
      base.OnLoad(harmony);
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
}