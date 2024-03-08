using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;

namespace SpaceStore {
    public sealed class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            // 初始化 PUtil 的文件
            PUtil.InitLibrary();
            // 检查模组版本是否更新
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }
    }

    public static class StaticVars {
        public static Tag PrimaryTag = new Tag("SpaceStore");
        public static Tag AutoTag = new Tag("RoboPanleAdd");
    }
}
