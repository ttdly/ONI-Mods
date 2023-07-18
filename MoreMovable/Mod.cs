using HarmonyLib;
using MoreMovable;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;

namespace MoveGeysers {
    public class Mod :KMod.UserMod2{
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }
    }

    public class Patchs {
        public static void AddMovable(GameObject go) {
            go.AddOrGet<Pickupable>();
            go.AddOrGet<Movable>();
            go.AddOrGet<RemoveClearable>();
        }

        [HarmonyPatch(typeof(PropSurfaceSatellite1Config), nameof(PropSurfaceSatellite1Config.CreatePrefab))]
        public class PropSurfaceSatellite1Config_CreatePrefab_Patch {
            public static void Postfix(GameObject __result) {
                AddMovable(__result);
            }
        }

        [HarmonyPatch(typeof(PropSurfaceSatellite2Config), nameof(PropSurfaceSatellite2Config.CreatePrefab))]
        public class PropSurfaceSatellite2Config_CreatePrefab_Patch {
            public static void Postfix(GameObject __result) {
                AddMovable(__result);
            }
        }

        [HarmonyPatch(typeof(PropSurfaceSatellite3Config), nameof(PropSurfaceSatellite3Config.CreatePrefab))]
        public class PropSurfaceSatellite3Config_CreatePrefab_Patch {
            public static void Postfix(GameObject __result) {
                AddMovable(__result);
            }
        }
    }

}
