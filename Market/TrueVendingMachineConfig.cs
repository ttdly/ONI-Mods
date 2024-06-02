using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Market {
  internal class TrueVendingMachineConfig : IBuildingConfig {
    public const string ID = "TrueVendingMachine";

    public override BuildingDef CreateBuildingDef() {
      var tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
      var minerals = MATERIALS.REFINED_METALS;
      var tieR6 = NOISE_POLLUTION.NOISY.TIER6;
      var tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
      var noise = tieR6;
      var buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "vendingmachine_kanim", 30, 60f, tieR5, minerals,
        2400f, BuildLocationRule.OnFloor, tieR2, noise);
      buildingDef.RequiresPowerInput = false;
      buildingDef.AudioCategory = "HollowMetal";
      buildingDef.AudioSize = "large";
      return buildingDef;
    }

    public override void DoPostConfigureComplete(GameObject go) {
      var storage = go.AddOrGet<Storage>();
      storage.capacityKg = 1000;
      storage.showInUI = true;
      storage.showDescriptor = true;
      storage.storageFilters = new List<Tag> {
        GameTags.Seed,
        GameTags.RefinedMetal,
        GameTags.BuildableRaw
      };
      storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
      storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
      storage.showCapacityStatusItem = true;
      storage.showCapacityAsMainStatus = true;
      go.AddOrGet<TrueVendingMachineComponent>();
      go.AddOrGet<TreeFilterable>();
    }
  }
}