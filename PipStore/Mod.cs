using HarmonyLib;
using KMod;
using PipStore.Screen;
using PipStore.Screen.Basic;

namespace PipStore {
    public sealed class Mod : UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            ModAssets.LoadAssets();
        }
    }
}