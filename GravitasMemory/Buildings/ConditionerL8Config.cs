// Decompiled with JetBrains decompiler
// Type: LiquidConditionerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2BFC01D3-C011-441F-99ED-A7DA2F70C2DC
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ConditionerL8Config : IBuildingConfig {
    public const string ID = "ConditionerL8";
    private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>()
    {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Insulate,
    Storage.StoredItemModifier.Seal
  };

    public override BuildingDef CreateBuildingDef() {
        float[] tieR6 = new float[2] { 1200f, 500f };
        string[] allMetals = { "Special" , "Plastic" };
        EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
        EffectorValues none = BUILDINGS.DECOR.NONE;
        EffectorValues noise = tieR2;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ConditionerL8", 2, 2, "conditionerl8_kanim", 100, 120f, tieR6, allMetals, 1600f, BuildLocationRule.Anywhere, none, noise);
        BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
        buildingDef.GeneratorWattageRating = 10f;
        buildingDef.GeneratorBaseCapacity = 20f;
        buildingDef.RequiresPowerInput = false;
        buildingDef.RequiresPowerOutput = true;
        buildingDef.PowerOutputOffset = new CellOffset(0, 0);
        buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
        buildingDef.ExhaustKilowattsWhenActive = 0.0f;
        buildingDef.InputConduitType = ConduitType.Liquid;
        buildingDef.OutputConduitType = ConduitType.Liquid;
        buildingDef.Floodable = false;
        buildingDef.UtilityInputOffset = new CellOffset(0, 0);
        buildingDef.PermittedRotations = PermittedRotations.FlipH;
        buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
        buildingDef.Overheatable = false;
        buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
        GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "ConditionerL8");
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
        go.AddOrGet<LoopingSounds>();
        AirConditioner airConditioner = go.AddOrGet<AirConditioner>();
        airConditioner.temperatureDelta = -14f;
        airConditioner.maxEnvironmentDelta = -50f;
        airConditioner.isLiquidConditioner = true;
        ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
        conduitConsumer.conduitType = ConduitType.Liquid;
        conduitConsumer.consumptionRate = 10f;
        Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
        defaultStorage.showInUI = true;
        defaultStorage.capacityKg = 2f * conduitConsumer.consumptionRate;
        defaultStorage.SetDefaultStoredItemModifiers(ConditionerL8Config.StoredItemModifiers);
    }

    public override void DoPostConfigureComplete(GameObject go) {
        go.AddOrGet<LogicOperationalController>();
        go.AddOrGetDef<PoweredActiveController.Def>();
        DevGenerator devGenerator = go.AddOrGet<DevGenerator>();
        devGenerator.powerDistributionOrder = 9;
        devGenerator.wattageRating = 10f;
        go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
    }
}
