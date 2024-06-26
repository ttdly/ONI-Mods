﻿using TUNING;
using UnityEngine;

namespace WirelessProject.ProwerManager {
  public class PowerProxyConfig : IBuildingConfig {
    public const string ID = "PowerProxy";

    public override BuildingDef CreateBuildingDef() {
      var tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER7;
      var refinedMetals = MATERIALS.REFINED_METALS;
      var tieR5 = NOISE_POLLUTION.NOISY.TIER5;
      var tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
      var noise = tieR5;
      var buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 2, "power_proxy_kanim", 30, 30f, tieR3,
        refinedMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
      buildingDef.ViewMode = OverlayModes.Power.ID;
      buildingDef.AudioCategory = "Metal";
      buildingDef.ExhaustKilowattsWhenActive = 800f;
      buildingDef.SelfHeatKilowattsWhenActive = 400f;
      buildingDef.OnePerWorld = true;
      buildingDef.Overheatable = false;
      return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
      go.AddOrGet<PowerProxy>();
    }

    public override void DoPostConfigureComplete(GameObject go) {
      go.AddOrGetDef<PoweredActiveController.Def>();
    }
  }
}