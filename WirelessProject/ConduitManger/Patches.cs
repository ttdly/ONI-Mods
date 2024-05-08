using HarmonyLib;

namespace WirelessProject.ConduitManger {
    public class Patches {
        [HarmonyPatch(typeof(ConduitConsumer), "ConduitUpdate")]
        public class ConduitConsumer_ConduitConsumer_Patch {
            public static bool Prefix(ConduitConsumer __instance) {
                if (__instance.gameObject.HasTag(StaticVar.HasProxyTag)) {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ConduitDispenser), "ConduitUpdate")]
        public class ConduitDispenser_ConduitUpdate_Patch {
            public static bool Prefix(ConduitDispenser __instance) {
                if (__instance.gameObject.HasTag(StaticVar.HasProxyTag)) {
                    return false;
                }
                return true;
            }
        }
    }
}
