using System;
using GeyserExpandMachine.Buildings;
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
            }
        }
    }
}