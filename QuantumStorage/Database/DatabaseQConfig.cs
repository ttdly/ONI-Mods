using TUNING;
using UnityEngine;

namespace QuantumStorage.Database {
    public class DatabaseQConfig : IBuildingConfig {
        public const string ID = "DatabaseQ";
        public const float BASE_SECONDS_PER_POINT = 60f;
        public const float MASS_PER_POINT = 50f;
        public const float BASE_MASS_PER_SECOND = 0.8333333f;
        public const float CAPACITY = 750f;
        public static readonly Tag INPUT_MATERIAL = GameTags.Water;

        public override BuildingDef CreateBuildingDef() {
            float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] allMetals = MATERIALS.ALL_METALS;
            EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "research_center2_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 120f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "large";
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            go.AddOrGetDef<PoweredController.Def>();
            go.AddOrGet<DatabaseQ>();
        }

        public override void DoPostConfigureComplete(GameObject go) {
        }
    }
}
