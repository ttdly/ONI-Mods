using FixPack.DestructibleFeatures;
using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using PeterHan.PLib.PatchManager;

namespace FixPack {
    public sealed class Mod : KMod.UserMod2 {

        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
            RegisterSting();
            new POptions().RegisterOptions(this,typeof(FixPack.Option));
        }

        public static void RegisterSting() {
            LocString.CreateLocStringKeys(typeof(FixPack.String.UI));
            DestructibleStrings.AddStrings();
        }

    }
}
