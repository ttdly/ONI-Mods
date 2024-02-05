﻿using HarmonyLib;
using System;
using static DetailsScreen;
using System.Collections.Generic;
using UnityEngine;


namespace Market {
    internal class Patches {

        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class DetailsScreen_OnPrefabInit_Patch {
            internal static void Postfix(List<SideScreenRef> ___sideScreens, GameObject ___sideScreenContentBody) {
                MarketSideScreen.AddSideScreen(___sideScreens, ___sideScreenContentBody);
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                LocString.CreateLocStringKeys(typeof(MyStrings.BUILDINGS));
                LocString.CreateLocStringKeys(typeof(MyStrings.MISC));
                LocString.CreateLocStringKeys(typeof(MyStrings.SIDESCREEN));
                LocString.CreateLocStringKeys(typeof(MyStrings.OTHERS));
                Array NewBuildings = (new Array[] {
                    new string[]{ TrueVendingMachineConfig.ID, "Base", "InteriorDecor", "shopping"},
                });
                foreach (string[] building in NewBuildings) {
                    AddNewBuilding(building[0], building[1], building[2], building[3]);
                }
                StaticVars.Init();
            }

        }

        public static void AddNewBuilding(string building_id, string plan_screen_cat_id, string tech_id, string string_key) {
            ModUtil.AddBuildingToPlanScreen(plan_screen_cat_id, building_id);
            Db.Get().Techs.Get(tech_id).unlockedItemIDs.Add(building_id);
            TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(building_id, string_key);
        }
    }
}
