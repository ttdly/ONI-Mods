using UnityEngine;

namespace PackAnything {
    public class WorldModifierConfig : IBuildingConfig {
        public const string ID = "MovableBeacon";

        public override BuildingDef CreateBuildingDef() {
            float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] minerals = TUNING.MATERIALS.REFINED_METALS;
            EffectorValues tieR6 = TUNING.NOISE_POLLUTION.NOISY.TIER6;
            EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
            EffectorValues noise = tieR6;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "displacement_beacon_kanim", 30, 60f, tieR5, minerals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
            buildingDef.RequiresPowerInput = false;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "large";
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go) {
            Light2D light2D = go.AddOrGet<Light2D>();
            light2D.Color = new Color(0.6f, 0f, 0.6f, 1f);
            light2D.Range = 3f;
            light2D.Offset = new Vector2(0, 1);
            light2D.overlayColour = new Color(0.8f, 0f, 0.8f, 1f);
            light2D.shape = LightShape.Circle;
            light2D.drawOverlay = true;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 200;
            storage.showInUI = true;
            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.RequestedItemTag = SimHashes.Steel.CreateTag();
            manualDeliveryKg.capacity = 200f;
            manualDeliveryKg.refillMass = 10f;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
            go.AddOrGet<WorldModifier>();
        }
    }
}
