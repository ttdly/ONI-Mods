using HarmonyLib;

namespace TweaksPack {
    public class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            TweaksPackPatches.ManualPatchBuildings(harmony);
        }
    }
}
