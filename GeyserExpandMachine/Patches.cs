using System;
using GeyserExpandMachine.Buildings;
using GeyserExpandMachine.Screen;
using HarmonyLib;
using TUNING;
using UnityEngine;

namespace GeyserExpandMachine {
    public class Patches {
        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildingsPatch {
            public static void Prefix() {
                LocString.CreateLocStringKeys(typeof(ModString));
                
                ModUtil.AddBuildingToPlanScreen("Plumbing", LiquidGeyserExpandConfig.ID);
                Db.Get().Techs.Get("LiquidPiping").unlockedItemIDs.Add(LiquidGeyserExpandConfig.ID);
                BUILDINGS.PLANSUBCATEGORYSORTING.Add(LiquidGeyserExpandConfig.ID, "LiquidPump");
                
                ModUtil.AddBuildingToPlanScreen("HVAC", GasGeyserExpandConfig.ID);
                Db.Get().Techs.Get("GasPiping").unlockedItemIDs.Add(GasGeyserExpandConfig.ID);
                BUILDINGS.PLANSUBCATEGORYSORTING.Add(GasGeyserExpandConfig.ID, "GasPump");
            }
        }

        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public class OnPrefabInitPatch {
            public static void Postfix() {
                ExpandSideScreen.AddSideScreenContent();
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildingsLoadGeneratedBuildingsPatch {
            public static void Prefix() {
                LocString
                    .CreateLocStringKeys(typeof(ModString.TOGGLEGROUP), 
                        "STRINGS.UI.GEYSEREXPANDSIDESCREEN.MAINCONTENT.");
                LocString
                    .CreateLocStringKeys(typeof(ModString.GASGEYSEREXPAND), "STRINGS.BUILDINGS.PREFABS.");
                LocString
                    .CreateLocStringKeys(typeof(ModString.LIQUIDGEYSEREXPAND), "STRINGS.BUILDINGS.PREFABS.");
                LocString
                    .CreateLocStringKeys(typeof(ModString.SIDESCREEN), "STRINGS.CAL");
            }
        }
    }
}