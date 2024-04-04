using HarmonyLib;
using PeterHan.PLib.Actions;
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
            StaticVars.Action = new PActionManager().CreateAction("OpenSpaceStore", MyString.UI.MENU_TOOL.TITLE, new PKeyBinding());
        }
    }

    public static class StaticVars {
        public static string ToolName = "SpaceStoreTool";
        public static Tag PrimaryTag = new Tag("SpaceStore");
        public static Tag AutoTag = new Tag("RoboPanleAdd");
        public static int Coin = 0;
        public static PAction Action;

        public static void AddCoin(int amount) {
            Coin += amount;
        }
    }
}
