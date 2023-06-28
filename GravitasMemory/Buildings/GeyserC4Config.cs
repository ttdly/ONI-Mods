

using HarmonyLib;
using TUNING;
using UnityEngine;
using static STRINGS.ELEMENTS;

public class GeyserC4Config: IBuildingConfig {
    public const string ID = "GeyserC4";
    public const float WATER2OXYGEN_RATIO = 0.888f;
    public const float OXYGEN_TEMPERATURE = 343.15f;

    public override BuildingDef CreateBuildingDef() {
        float[] tieR3_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
        string[] allMetals = { "Special" };
        EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
        EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
        EffectorValues noise = tieR3_2;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GeyserC4", 1, 2, "geyserc4_kanim", 30, 30f, tieR3_1, allMetals, 800f, BuildLocationRule.Anywhere, tieR1, noise);
        buildingDef.ExhaustKilowattsWhenActive = 0f;
        buildingDef.SelfHeatKilowattsWhenActive = 0f;
        buildingDef.Overheatable = false;
        buildingDef.Floodable = false;
        buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
        buildingDef.AudioCategory = "HollowMetal";
        buildingDef.InputConduitType = ConduitType.Gas;
        buildingDef.UtilityInputOffset = new CellOffset(0, 0);
        buildingDef.OutputConduitType = ConduitType.Liquid;
        buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
        buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
        go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
        Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
        defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
        Prioritizable.AddRef(go);
        ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
        elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]{
            new ElementConverter.ConsumedElement(new Tag("CrudeOil"), 10f),
            new ElementConverter.ConsumedElement(new Tag("Steam"), 1f)
        };
        elementConverter.outputElements = new ElementConverter.OutputElement[2]{
            new ElementConverter.OutputElement(9.9f, SimHashes.Methane, 313.5f, outputElementOffsetx: 0f, outputElementOffsety: 1f),
            new ElementConverter.OutputElement(1f, SimHashes.DirtyWater, 0.0f, storeOutput: true, diseaseWeight: 0.25f)
        };
        ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
        elementConsumer.consumptionRate = 15f;
        elementConsumer.capacityKG = 30f;
        elementConsumer.elementToConsume = SimHashes.CrudeOil;
        elementConsumer.storeOnConsume = true;
        elementConsumer.consumptionRadius = (byte)2;
        elementConsumer.ignoreActiveChanged = true;
        ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
        conduitConsumer.conduitType = ConduitType.Gas;
        conduitConsumer.consumptionRate = 1f;
        conduitConsumer.capacityKG = 2f;
        conduitConsumer.capacityTag = SimHashes.Steam.CreateTag();
        conduitConsumer.forceAlwaysSatisfied = true;
        conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
        ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
        conduitDispenser.conduitType = ConduitType.Liquid;
        conduitDispenser.invertElementFilter = true;
        conduitDispenser.elementFilter = new SimHashes[1]{
            SimHashes.CrudeOil
        };
        go.AddOrGet<GeyserC4>().consumeTag = SimHashes.Steam.CreateTag();
    }

    public override void DoPostConfigureComplete(GameObject go) {
        go.AddOrGet<LogicOperationalController>();
        go.AddOrGetDef<PoweredActiveController.Def>();
    }

    public override void DoPostConfigureUnderConstruction(GameObject go) {
        base.DoPostConfigureUnderConstruction(go);
        go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
    }
}