using TUNING;
using UnityEngine;

namespace FragmentThermostat {
  public class FragmentThermostatConfig : IBuildingConfig {
        public const string ID = "FTMod";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public override BuildingDef CreateBuildingDef() {
            var buildingDef = BuildingTemplates.CreateBuildingDef(ID,  3, 1, "fragment_thermostat_kanim",
                30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER7, MATERIALS.REFINED_METALS, 1600f, 
                BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER1);
            buildingDef.InputConduitType = ConduitType.Solid;
            buildingDef.OutputConduitType = ConduitType.Solid;
            buildingDef.Floodable = false;
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 1200f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
            buildingDef.OverheatTemperature = 473.15f;
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, ID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            go.AddOrGetDef<PoweredActiveTransitionController.Def>();
            var storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.capacityKg = 200f;
            go.AddOrGet<RequireOutputs>().ignoreFullPipe = true;
            go.AddOrGet<FragmentThermostatComponent>();
        }

        public override void DoPostConfigureComplete(GameObject go) {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveTransitionController.Def>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
            go.GetComponent<RequireInputs>().SetRequirements(true, false);
            go.AddOrGet<SolidConduitBridge>();
        }
    }
}