using FixPack.DestructibleFeatures;
using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace FixPack {
    public sealed class Mod : KMod.UserMod2 {

        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
            RegisterSting();
            new POptions().RegisterOptions(this,typeof(Option));
        }

        public static void RegisterSting() {
            LocString.CreateLocStringKeys(typeof(String.UI));
            DestructibleStrings.AddStrings();
        }

    }
}
