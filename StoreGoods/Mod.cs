using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using System.IO;

namespace StoreGoods {
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
            string configPath = Path.GetFullPath(Path.Combine(KMod.Manager.GetDirectory(), "spacestore", "store_2024_4_13_store_goods.json"));
            SpaceStoreConfigWriter.Write(configPath);
        }
    }

    public class StaticVars {
        public static Tag AutoTag = new Tag("AutoTweak");
    }
}
