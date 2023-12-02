
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
            go.AddOrGet<WorldModifier>();
        }
    }
}
