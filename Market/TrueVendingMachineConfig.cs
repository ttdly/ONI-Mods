using UnityEngine;

namespace Market {
    internal class TrueVendingMachineConfig : IBuildingConfig {
        public const string ID = "TrueVendingMachine";

        public override BuildingDef CreateBuildingDef() {
            float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] minerals = TUNING.MATERIALS.REFINED_METALS;
            EffectorValues tieR6 = TUNING.NOISE_POLLUTION.NOISY.TIER6;
            EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
            EffectorValues noise = tieR6;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "vendingmachine_kanim", 30, 60f, tieR5, minerals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
            buildingDef.RequiresPowerInput = false;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "large";
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go) {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 1100;
            storage.showInUI = true;
            go.AddOrGet<TrueVendingMachineComponent>();
            RangeVisualizer rangeVisualizer = go.AddOrGet<RangeVisualizer>();
            rangeVisualizer.OriginOffset = new Vector2I(0, 0);
            rangeVisualizer.RangeMin.x = -1;
            rangeVisualizer.RangeMin.y = 0;
            rangeVisualizer.RangeMax.x = 1;
            rangeVisualizer.RangeMax.y = 0;
            rangeVisualizer.BlockingTileVisible = true;
        }
    }
}
