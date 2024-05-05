using TUNING;
using UnityEngine;

namespace WirelessProject.ConduitManger {
    public class ConduitProxyConfig : IBuildingConfig {
        public const string ID = "ConduitProxy";

        public override BuildingDef CreateBuildingDef() {
            float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
            EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noise = tieR5;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "filter_liquid_kanim", 30, 30f, tieR3, refinedMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.ExhaustKilowattsWhenActive = 10f;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            go.AddOrGet<ConduitProxy>();
        }

        public override void DoPostConfigureComplete(GameObject go) {
            go.AddOrGet<UserNameable>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}
