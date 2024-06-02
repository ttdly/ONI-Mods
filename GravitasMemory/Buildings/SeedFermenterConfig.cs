using TUNING;
using UnityEngine;

public class SeedFermenterConfig : IBuildingConfig {
  public const string ID = "SeedFermenter";
  public const float WATER2OXYGEN_RATIO = 0.888f;
  public const float OXYGEN_TEMPERATURE = 343.15f;

  public override BuildingDef CreateBuildingDef() {
    var tieR3_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    var allMetals = MATERIALS.RAW_MINERALS;
    var tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
    var tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    var noise = tieR3_2;
    var buildingDef = BuildingTemplates.CreateBuildingDef("SeedFermenter", 2, 3, "seed_fermenter_kanim", 30, 30f,
      tieR3_1, allMetals, 800f, BuildLocationRule.Anywhere, tieR1, noise);
    buildingDef.ExhaustKilowattsWhenActive = 0f;
    buildingDef.SelfHeatKilowattsWhenActive = 0f;
    buildingDef.Overheatable = false;
    buildingDef.Floodable = false;
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 5f;
    buildingDef.AudioCategory = "HollowMetal";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
    var defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
    defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    defaultStorage.capacityKg = 200f;
    Prioritizable.AddRef(go);
    var elementDropper = go.AddComponent<ElementDropper>();
    elementDropper.emitMass = 10f;
    elementDropper.emitTag = new Tag("Ethanol");
    elementDropper.emitOffset = new Vector3(0.0f, 0.0f, 0.0f);
    var elementDropper2 = go.AddComponent<ElementDropper>();
    elementDropper2.emitMass = 10f;
    elementDropper2.emitTag = new Tag("Dirt");
    elementDropper2.emitOffset = new Vector3(0.0f, 0.0f, 0.0f);
    var elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[1] {
      new ElementConverter.ConsumedElement(GameTags.Seed, 0.1f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[2] {
      new ElementConverter.OutputElement(0.01f, SimHashes.Dirt, 313.5f, storeOutput: true, diseaseWeight: 0.25f),
      new ElementConverter.OutputElement(0.09f, SimHashes.Ethanol, 313.5f, storeOutput: true, diseaseWeight: 0.25f)
    };
    var elementConsumer = go.AddOrGet<ElementConsumer>();
    elementConsumer.elementToConsume = SimHashes.Oxygen;
    elementConsumer.consumptionRate = 0.01f;
    elementConsumer.capacityKG = 0.01f;
    elementConsumer.consumptionRadius = 3;
    elementConsumer.showInStatusPanel = true;
    elementConsumer.sampleCellOffset = new Vector3(0.0f, 0.0f, 0.0f);
    elementConsumer.isRequired = false;
    elementConsumer.storeOnConsume = true;
    elementConsumer.showDescriptor = false;
    elementConsumer.ignoreActiveChanged = true;
    go.AddOrGet<SeedFermenter>().consumeTag = GameTags.Seed;
    var manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(defaultStorage);
    manualDeliveryKg.RequestedItemTag = GameTags.Seed;
    manualDeliveryKg.capacity = 200f;
    manualDeliveryKg.refillMass = 10f;
    manualDeliveryKg.MinimumMass = 1f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
  }

  public override void DoPostConfigureComplete(GameObject go) {
    go.AddOrGetDef<PoweredActiveStoppableController.Def>();
  }
}