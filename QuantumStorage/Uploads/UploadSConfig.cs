
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace QuantumStorage.Uploads {
    public class UploadSConfig : IBuildingConfig {
        public const string ID = "UploadS";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public override BuildingDef CreateBuildingDef() {
            float[] construction_mass = new float[2] {
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
            };
            string[] construction_materials = new string[2]{
              "RefinedMetal",
              "Plastic"
            };
            EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
            EffectorValues tieR0 = BUILDINGS.DECOR.PENALTY.TIER0;
            EffectorValues noise = tieR1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "limit_valve_solid_kanim", 30, 10f, construction_mass, construction_materials, 1600f, BuildLocationRule.Anywhere, tieR0, noise);
            buildingDef.InputConduitType = ConduitType.Solid;
            buildingDef.Floodable = false;
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 10f;
            buildingDef.PowerInputOffset = new CellOffset(0, 1);
            buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            go.AddOrGetDef<PoweredActiveTransitionController.Def>();
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.storageFilters = UPLOAD_STORAGE;
            storage.capacityKg = 200f;
            go.AddOrGet<SolidConduitConsumer>();
            go.AddOrGet<UploadState>();
        }

        public static List<Tag> UPLOAD_STORAGE = new List<Tag>(){
          GameTags.Alloy,
          GameTags.RefinedMetal,
          GameTags.Metal,
          GameTags.BuildableRaw,
          GameTags.BuildableProcessed,
          GameTags.Farmable,
          GameTags.Organics,
          GameTags.Compostable,
          GameTags.Agriculture,
          GameTags.Filter,
          GameTags.ConsumableOre,
          GameTags.Liquifiable,
          GameTags.IndustrialProduct,
          GameTags.IndustrialIngredient,
          GameTags.ManufacturedMaterial,
          GameTags.RareMaterials,
          GameTags.Other,
        };  

        public override void DoPostConfigureComplete(GameObject go) {
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        }
    }
}
