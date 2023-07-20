using HarmonyLib;


namespace PackAnything {
    public class Patchs {
        [HarmonyPatch(typeof(Geyser), "OnSpawn")]
        public static class Geyser_OnSpawn_Patch {
            public static void Postfix(Geyser __instance) {
                if (__instance.GetType() != typeof(Geyser)) {
                    Packable packable = __instance.GetComponent<Packable>();
                    if ((UnityEngine.Object)packable != (UnityEngine.Object)null) {
                        UnityEngine.Object.Destroy(__instance.GetComponent<Packable>());
                    }
                    return;
                } else {
                    __instance.FindOrAddComponent<Packable>();
                }
            }
        }
    }
}
