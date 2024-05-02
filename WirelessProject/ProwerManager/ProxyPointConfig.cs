using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace WirelessProject.ProwerManager {
    internal class ProxyPointConfig : IBuildingConfig {
        public const string ID = "ProxyPoint";

        public override BuildingDef CreateBuildingDef() {
            float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
            EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noise = tieR5;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "dev_generator_kanim", 30, 30f, tieR3, refinedMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.ExhaustKilowattsWhenActive = 10f;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) {
            go.AddOrGet<PowerProxy>();
        }

        public override void DoPostConfigureComplete(GameObject go) {
            go.AddOrGet<UserNameable>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}
