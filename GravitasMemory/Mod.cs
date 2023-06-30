using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.PatchManager;

namespace GravitasMemory
{
    public sealed class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }
    }
}
