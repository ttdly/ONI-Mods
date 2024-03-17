using TUNING;
using UnityEngine;

namespace SpaceStore.Store
{
    internal class SpaceStoreConfig : IBuildingConfig
    {
        public const string ID = "TrueVendingMachine";

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] minerals = MATERIALS.REFINED_METALS;
            EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
            EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
            EffectorValues noise = tieR6;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "vendingmachine_kanim", 30, 60f, tieR5, minerals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
            buildingDef.RequiresPowerInput = false;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "large";
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 1000;
            storage.showInUI = false;
        }
    }
}
