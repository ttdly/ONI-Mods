using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Market {
    internal class TrueVendingMachineConfig : IBuildingConfig {
        public const string ID = "TrueVendingMachine";

        public override BuildingDef CreateBuildingDef() {
            float[] tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] minerals = MATERIALS.REFINED_METALS;
            EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
            EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
            EffectorValues noise = tieR6;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "vendingmachine_kanim", 30, 60f, tieR5, minerals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
            buildingDef.RequiresPowerInput = false;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "large";
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go) {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 1000;
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.storageFilters = new List<Tag>() {
                GameTags.Seed,
                GameTags.RefinedMetal,
                GameTags.BuildableRaw,
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
