using System;
using System.Collections.Generic;
using HarmonyLib;
using TUNING;
using UnityEngine;
using static DetailsScreen;


namespace Market {
  internal class Patches {
    public static void AddNewBuilding(string building_id, string plan_screen_cat_id, string tech_id,
      string string_key) {
      ModUtil.AddBuildingToPlanScreen(plan_screen_cat_id, building_id);
      Db.Get().Techs.Get(tech_id).unlockedItemIDs.Add(building_id);
      BUILDINGS.PLANSUBCATEGORYSORTING.Add(building_id, string_key);
    }

    [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
    public static class DetailsScreen_OnPrefabInit_Patch {
      internal static void Postfix(List<SideScreenRef> ___sideScreens, GameObject ___sideScreenConfigContentBody) {
        MarketSideScreen.AddSideScreen(___sideScreens, ___sideScreenConfigContentBody);
      }
    }


    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
      public static void Prefix() {
        LocString.CreateLocStringKeys(typeof(MyStrings.BUILDINGS));
        LocString.CreateLocStringKeys(typeof(MyStrings.MISC));
        LocString.CreateLocStringKeys(typeof(MyStrings.SIDESCREEN));
        LocString.CreateLocStringKeys(typeof(MyStrings.OTHERS));
        Array NewBuildings = new Array[] {
          new[] { TrueVendingMachineConfig.ID, "Base", "InteriorDecor", "shopping" }
        };
        foreach (string[] building in NewBuildings) AddNewBuilding(building[0], building[1], building[2], building[3]);
        StaticVars.Init();
      }
    }
  }
}