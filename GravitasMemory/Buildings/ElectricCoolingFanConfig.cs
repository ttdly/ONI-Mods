

using GravitasMemory;
using UnityEngine;

public class ElectricCoolingFanConfig : IBuildingConfig {
    public const string ID = "ElectricCoolingFan";
    public override BuildingDef CreateBuildingDef() {
        float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
        string[] allMetals = TUNING.MATERIALS.ALL_METALS;
        EffectorValues tieR2 = TUNING.NOISE_POLLUTION.NOISY.TIER2;
        EffectorValues tieR1 = TUNING.BUILDINGS.DECOR.BONUS.TIER1;
        EffectorValues noise = tieR2;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ElectricCoolingFan", 1, 4, "electric_cooling_fan_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
        buildingDef.RequiresPowerInput = true;
        buildingDef.EnergyConsumptionWhenActive = 60f;
        buildingDef.ExhaustKilowattsWhenActive = -64f;
        buildingDef.SelfHeatKilowattsWhenActive = 0f;
        buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
        buildingDef.ViewMode = OverlayModes.Temperature.ID;
        buildingDef.AudioCategory = "HollowMetal";
        buildingDef.OverheatTemperature = 398.15f;
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
        go.AddOrGet<LoopingSounds>();
        Storage storage = go.AddComponent<Storage>();
        storage.capacityKg = 50f;
        ElementDropper elementDropper = go.AddComponent<ElementDropper>();
        elementDropper.emitMass = 10f;
        elementDropper.emitTag = new Tag("Water");
        elementDropper.emitOffset = new Vector3(0.0f, 0.0f, 0.0f);
        //CustomDropper customDropper = go.AddComponent<CustomDropper>();
        //customDropper.emitMass = 3f;
        //customDropper.emitTag = GameTags.Gas;
        //customDropper.emitOffset = new Vector3(0.0f, 0.0f, 0.0f);

        ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
        elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]{
            new ElementConverter.ConsumedElement(GameTags.IceOre, 0.01f),
        };
        elementConverter.outputElements = new ElementConverter.OutputElement[1]{
            new ElementConverter.OutputElement(0.01f, SimHashes.Water, 272f, storeOutput: true, diseaseWeight: 0.25f)
        };
        //ElementConsumer elementConsumer = go.AddComponent<ElementConsumer>();
        //elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
        //elementConsumer.consumptionRate = 3f;
        //elementConsumer.storeOnConsume = true;
        //elementConsumer.showInStatusPanel = true;
        //elementConsumer.consumptionRadius = (byte)2;
        //elementConsumer.sampleCellOffset = new Vector3(0.0f, 1.0f, 0.0f);
        //elementConsumer.showDescriptor = false;
        ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
        manualDeliveryKg.SetStorage(storage);
        manualDeliveryKg.RequestedItemTag = GameTags.IceOre;
        manualDeliveryKg.capacity = 50f;
        manualDeliveryKg.refillMass = 10f;
        manualDeliveryKg.MinimumMass = 10f;
        manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
        go.AddOrGet<ElectricCooling>().consumeTag = GameTags.IceOre;
    }

    public override void DoPostConfigureComplete(GameObject go) {
        go.AddOrGet<LogicOperationalController>();
        go.AddOrGetDef<ActiveController.Def>();
    }
}


