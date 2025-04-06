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
                
                ModUtil.AddBuildingToPlanScreen("Conveyance", LiquidGeyserExpandConfig.ID);
                Db.Get().Techs.Get("SolidManagement").unlockedItemIDs.Add(LiquidGeyserExpandConfig.ID);
                BUILDINGS.PLANSUBCATEGORYSORTING.Add(LiquidGeyserExpandConfig.ID, "cooler");
                
                ModUtil.AddBuildingToPlanScreen("Conveyance", GasGeyserExpandConfig.ID);
                Db.Get().Techs.Get("SolidManagement").unlockedItemIDs.Add(GasGeyserExpandConfig.ID);
                BUILDINGS.PLANSUBCATEGORYSORTING.Add(GasGeyserExpandConfig.ID, "cooler");
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
                LocString.CreateLocStringKeys(typeof(ModString.UI), "STRINGS.");
            }
        }
    }
}