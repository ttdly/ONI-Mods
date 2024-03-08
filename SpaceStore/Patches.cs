using HarmonyLib;
using System.Collections.Generic;
using static DetailsScreen;
using UnityEngine;
using SpaceStore.MyGeyser;
using SpaceStore.StoreRoboPanel;
using TemplateClasses;

namespace SpaceStore {
    public class Patches {

        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class DetailsScreen_OnPrefabInit_Patch {
            public static void Postfix(List<SideScreenRef> ___sideScreens, GameObject ___sideScreenConfigContentBody) {
                GeoActivatorSideScreen.AddSideScreen(___sideScreens, ___sideScreenConfigContentBody);
            }
        }


        [HarmonyPatch(typeof(SmartReservoir), "OnSpawn")]
        public static class SmartReservoir_Patch {
            public static void Postfix(SmartReservoir __instance) {
                GameObject go = GameUtil.KInstantiate(Assets.GetPrefab(GeoActivatorConfig.ID), Grid.SceneLayer.Move);
                go.transform.SetPosition(__instance.gameObject.transform.position);
                go.SetActive(true);

                go = GameUtil.KInstantiate(Assets.GetPrefab(RoboPanelConfig.ID), Grid.SceneLayer.Move);
                go.transform.SetPosition(__instance.gameObject.transform.position);
                go.SetActive(true);

            }
        }

        [HarmonyPatch(typeof(RockCrusherConfig), nameof(RockCrusherConfig.DoPostConfigureComplete))]
        public static class RockCrusherConfig_Patch {
            public static void Postfix(GameObject go) {
                Autoable.MakeDupTinkerable(go, true);
                RoboPanel panel = go.AddOrGet<RoboPanel>();
            }
        }
    }
}
