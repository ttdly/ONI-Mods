using HarmonyLib;
using System.Collections.Generic;
using static DetailsScreen;
using UnityEngine;

namespace StoreGoods {
    public class Patches {
        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class DetailsScreen_OnPrefabInit_Patch {
            public static void Postfix(List<SideScreenRef> ___sideScreens, GameObject ___sideScreenConfigContentBody) {
                GeoActivatorSideScreen.AddSideScreen(___sideScreens, ___sideScreenConfigContentBody);
            }
        }
    }
}
