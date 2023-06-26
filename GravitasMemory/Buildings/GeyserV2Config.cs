using TUNING;
using UnityEngine;

public class GeyserV2Config : IBuildingConfig {
    public const string ID = "GeyserV2";

    public override BuildingDef CreateBuildingDef() {
        float[] tieR3_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
        string[] allMetals = { "Special" };
        EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
        EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
        EffectorValues noise = tieR3_2;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GeyserV2", 4, 3, "geyserv2_kanim", 30, 30f, tieR3_1, allMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
        buildingDef.RequiresPowerInput = true;
        buildingDef.EnergyConsumptionWhenActive = 25f;
        buildingDef.ExhaustKilowattsWhenActive = 0f;
        buildingDef.SelfHeatKilowattsWhenActive = 0;
        buildingDef.Overheatable = false;
        buildingDef.Floodable = false;
        buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
        buildingDef.ViewMode = OverlayModes.Power.ID;
        buildingDef.AudioCategory = "HollowMetal";
        buildingDef.Breakable = true;
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
        CellOffset cellOffset = new CellOffset(0, 1);
        Prioritizable.AddRef(go);
        Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
        electrolyzer.maxMass = 1.8f;
        electrolyzer.hasMeter = false;
        electrolyzer.emissionOffset = cellOffset;
        Storage storage = go.AddOrGet<Storage>();
        storage.capacityKg = 330f;
        storage.showInUI = true;
        ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
        elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
        {
            new ElementConverter.ConsumedElement(GameTags.BuildableRaw, 1f)
        };
        elementConverter.outputElements = new ElementConverter.OutputElement[1]
        {
            new ElementConverter.OutputElement(2.0f, SimHashes.Magma, 2156f, outputElementOffsetx: (float) cellOffset.x, outputElementOffsety: (float) cellOffset.y)
        };
        ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
        manualDeliveryKg.SetStorage(storage);
        manualDeliveryKg.RequestedItemTag = SimHashes.SandStone.CreateTag();
        manualDeliveryKg.capacity = 330f;
        manualDeliveryKg.refillMass = 132f;
        manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    }

    public override void DoPostConfigureComplete(GameObject go) {
        go.AddOrGet<LogicOperationalController>();
        go.AddOrGetDef<PoweredActiveController.Def>();
    }
}
