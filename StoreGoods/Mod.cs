using System.Collections.Generic;
using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;

namespace StoreGoods {
  public sealed class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      // 初始化 PUtil 的文件
      PUtil.InitLibrary();
      // 检查模组版本是否更新
      new PVersionCheck().Register(this, new SteamVersionChecker());
      new PLocalization().Register();
#if DEBUG
            ModUtil.RegisterForTranslation(typeof(MyString));
#endif
    }

    public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods) {
      base.OnAllModsLoaded(harmony, mods);
      SpaceStoreConfigWriter.Write();
      LocString.CreateLocStringKeys(typeof(MyString));
    }
  }

  public class StaticVars {
    public static Tag AutoTag = new Tag("AutoTweak");
  }
}