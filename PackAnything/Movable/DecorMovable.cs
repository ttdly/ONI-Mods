using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PackAnything.Movable
{
    public class DecorMovable
    {
        public static void PatchBuildings(Harmony harmony)
        {
            PatchCommon(harmony);
            PatchDoors(harmony);
        }

        private static void PatchCommon(Harmony harmony)
        {
            var commonTypeList = new List<Type>()
            {
                typeof(PropGravitasLabWallConfig),
                typeof(PropGravitasLabWindowConfig),
                typeof(PropGravitasLabWindowHorizontalConfig),
                typeof(PropGravitasWallConfig),
                typeof(PropGravitasWallPurpleConfig),
                typeof(PropGravitasWallPurpleWhiteDiagonalConfig),
                typeof(GravitasLabLightConfig),
                typeof(TilePOIConfig),

            };

            var postfix = AccessTools.Method(typeof(CommonMovable), nameof(CommonMovable.CommonPostfix));

            foreach (var type in commonTypeList)
            {
                var targetMethod = type.GetMethod("DoPostConfigureComplete");
                harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfix));
            }
        }

        private static void PatchDoors(Harmony harmony)
        {
            var doorsTypeList = new List<Type>()
            {
                //typeof(GravitasPedestalConfig),
                typeof(GravitasDoorConfig),
                typeof(POIBunkerExteriorDoor),
                typeof(POIDlc2ShowroomDoorConfig),
                typeof(POIDoorInternalConfig),
                typeof(POIFacilityDoorConfig),
            };

            var postfix = AccessTools.Method(typeof(DecorMovable), nameof(DoorPostfix));

            foreach (var type in doorsTypeList)
            {
                var targetMethod = type.GetMethod("DoPostConfigureComplete");
                harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfix));
            }
        }

        public static void DoorPostfix(GameObject go)
        {
            go.AddOrGet<CommonMovable>().canCrossMove = false;
        }
    }
}
