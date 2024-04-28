using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using System.Collections.Generic;

namespace WirelessProject {
    public sealed class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            // 初始化 PUtil 的文件
            PUtil.InitLibrary();
            // 检查模组版本是否更新
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods) {
            base.OnAllModsLoaded(harmony, mods);
            LocString.CreateLocStringKeys(typeof(Strings.BUILDING));
            LocString.CreateLocStringKeys(typeof(Strings.BUILDINGS));
        }
    }
}
