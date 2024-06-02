using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;

namespace FragmentThermostat {
  public sealed class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      // 初始化 PUtil 的文件
      PUtil.InitLibrary();
      // 检查模组版本是否更新
      new PVersionCheck().Register(this, new SteamVersionChecker());
      new PLocalization().Register();
      new POptions().RegisterOptions(this, typeof(ModOptions));
#if DEBUG
            ModUtil.RegisterForTranslation(typeof(ModString));
#endif
    }
  }
}