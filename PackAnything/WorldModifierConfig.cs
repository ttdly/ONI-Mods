using TUNING;
using UnityEngine;

namespace PackAnything {
  public class WorldModifierConfig : IBuildingConfig {
    public const string ID = "MovableBeacon";

    public override string[] GetDlcIds() {
      return DlcManager.AVAILABLE_EXPANSION1_ONLY;
    }

    public override BuildingDef CreateBuildingDef() {
      var tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
      var minerals = MATERIALS.REFINED_METALS;
      var tieR6 = NOISE_POLLUTION.NOISY.TIER6;
      var tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
      var noise = tieR6;
      var buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "displacement_beacon_kanim", 30, 60f, tieR5,
        minerals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
      buildingDef.RequiresPowerInput = false;
      buildingDef.AudioCategory = "HollowMetal";
      buildingDef.AudioSize = "large";
      return buildingDef;
    }

    public override void DoPostConfigureComplete(GameObject go) {
      var light2D = go.AddOrGet<Light2D>();
      light2D.Color = new Color(0.6f, 0f, 0.6f, 1f);
      light2D.Range = 3f;
      light2D.Offset = new Vector2(0, 1);
      light2D.overlayColour = new Color(0.8f, 0f, 0.8f, 1f);
      light2D.shape = LightShape.Circle;
      light2D.drawOverlay = true;
      var storage = go.AddOrGet<Storage>();
      storage.capacityKg = 200;
      storage.showInUI = true;
      go.AddOrGet<WorldModifier>();
    }
  }
}