using HarmonyLib;
using System;
using UnityEngine;

namespace WirelessProject {
    public class Patches {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                LocString.CreateLocStringKeys(typeof(Strings.BUILDINGS));
                ModUtil.AddBuildingToPlanScreen("Equipment", ProwerManager.PowerProxyConfig.ID);
                TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(ProwerManager.PowerProxyConfig.ID, "Prower Proxy");
                Db.Get().Techs.Get("AdvancedResearch").unlockedItemIDs.Add(ProwerManager.PowerProxyConfig.ID);
            }
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Load))]
        public class Game_Load_Patch {
            public static void Prefix() {
                ProwerManager.StaticVar.PowerInfoList.Clear();
            }
        }
    }
}
