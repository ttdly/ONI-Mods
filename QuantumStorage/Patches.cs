using HarmonyLib;
using QuantumStorage.Database;
using QuantumStorage.Uploads;
using System;
using static DetailsScreen;
using System.Collections.Generic;
using UnityEngine;
using ChangeBlueprints;


namespace QuantumStorage {
    public class Patches {
        public static void AddBuildings() {
            Array NewBuildings = new Array[] {
                new string[]{ UploadGConfig.ID, "Oxygen", "FarmingTech", "UG"},
                new string[]{ UploadSConfig.ID, "Oxygen", "FarmingTech", "US"},
                new string[]{ UploadLConfig.ID, "Oxygen", "FarmingTech", "UL"},
                new string[]{ DatabaseQConfig.ID, "Oxygen", "FarmingTech", "DQ" }
            };

            foreach (string[] building in NewBuildings) {
                ModUtil.AddBuildingToPlanScreen(building[1], building[0]);
                Db.Get().Techs.Get(building[2]).unlockedItemIDs.Add(building[0]);
                TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(building[0], building[3]);
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                AddBuildings();
                LocString.CreateLocStringKeys(typeof(ModString.BUILDINGS));
            }
        }

        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class DetailsScreen_OnPrefabInit_Patch {
            internal static void Postfix(List<SideScreenRef> ___sideScreens, GameObject ___sideScreenConfigContentBody) {
                DatabaseQSideScreen.AddSideScreen(___sideScreens, ___sideScreenConfigContentBody);
            }
        }
    }
}
