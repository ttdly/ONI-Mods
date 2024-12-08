using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;

namespace SyntheticEvolution {
    public sealed class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            // 初始化 PUtil 的文件
            PUtil.InitLibrary();
            // 检查模组版本是否更新
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }
    }

    public class Patches {
        [HarmonyPatch(typeof(MinionConfig), "OnSpawn")]
        public static class MinionConfig_OnSpawn_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<Evolution>();
            }
        }
    }
}
