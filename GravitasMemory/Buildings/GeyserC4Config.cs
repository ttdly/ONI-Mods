

using TUNING;
using UnityEngine;

public class GeyserC4Config: IBuildingConfig {
    public const string ID = "GeyserC4";
    public const float WATER2OXYGEN_RATIO = 0.888f;
    public const float OXYGEN_TEMPERATURE = 343.15f;

    public override BuildingDef CreateBuildingDef() {
        float[] tieR3_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
        string[] allMetals = { "Special" };
        EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
        EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
        EffectorValues noise = tieR3_2;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GeyserC4", 3, 4, "geyserc4_kanim", 30, 30f, tieR3_1, allMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
        buildingDef.ExhaustKilowattsWhenActive = 0.25f;
        buildingDef.SelfHeatKilowattsWhenActive = 1f;
        buildingDef.Overheatable = false;
        buildingDef.Floodable = false;
        buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
        buildingDef.AudioCategory = "HollowMetal";
        buildingDef.InputConduitType = ConduitType.Liquid;
        buildingDef.UtilityInputOffset = new CellOffset(0, 0);
        buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
        CellOffset cellOffset = new CellOffset(0, 3);
        go.AddOrGet<DropAllWorkable>();
        go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
        Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
        electrolyzer.maxMass = 200f;
        electrolyzer.hasMeter = false;
        electrolyzer.emissionOffset = cellOffset;
        ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
        conduitConsumer.conduitType = ConduitType.Liquid;
        conduitConsumer.consumptionRate = 20f;
        conduitConsumer.capacityTag = SimHashes.CrudeOil.CreateTag();
        conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
        Storage storage = go.AddOrGet<Storage>();
        storage.capacityKg = 20f;
        storage.showInUI = true;
        ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
        elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]{
            new ElementConverter.ConsumedElement(SimHashes.CrudeOil.CreateTag(), 10f)
        };
        elementConverter.outputElements = new ElementConverter.OutputElement[1]{
        new ElementConverter.OutputElement(9.99999f, SimHashes.Methane, 343.15f, outputElementOffsetx: (float) cellOffset.x, outputElementOffsety: (float) cellOffset.y) 
        };
        Prioritizable.AddRef(go);
    }

    public override void DoPostConfigureComplete(GameObject go) {
        go.AddOrGet<LogicOperationalController>();
        go.AddOrGetDef<PoweredActiveController.Def>();
    }
}
