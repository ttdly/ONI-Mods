using System;
using System.Collections.Generic;
using ChangeBlueprints;
using HarmonyLib;
using QuantumStorage.Database;
using QuantumStorage.Uploads;
using TUNING;
using UnityEngine;
using static DetailsScreen;


namespace QuantumStorage {
  public class Patches {
    public static void AddBuildings() {
      Array NewBuildings = new Array[] {
        new[] { UploadGConfig.ID, "Oxygen", "FarmingTech", "UG" },
        new[] { UploadSConfig.ID, "Oxygen", "FarmingTech", "US" },
        new[] { UploadLConfig.ID, "Oxygen", "FarmingTech", "UL" },
        new[] { DatabaseQConfig.ID, "Oxygen", "FarmingTech", "DQ" }
      };

      foreach (string[] building in NewBuildings) {
        ModUtil.AddBuildingToPlanScreen(building[1], building[0]);
        Db.Get().Techs.Get(building[2]).unlockedItemIDs.Add(building[0]);
        BUILDINGS.PLANSUBCATEGORYSORTING.Add(building[0], building[3]);
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