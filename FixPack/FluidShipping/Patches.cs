using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace FixPack.FluidShipping {
    public class BuildingGenerationPatches {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                if (!SingletonOptions<Option>.Instance.ActiveFluidShipping) return;
                BottleInserterConfig.Setup();
                CanisterInserterConfig.Setup();
                BottleFillerConfig.Setup();
            }
        }


        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Techs_Patch {
            public static void Postfix() {
                if (!SingletonOptions<Option>.Instance.ActiveFluidShipping) return;
                //find tech with ImprovedLiquidPiping. Add s_bi_id and s_bf_id to that tech.
                Db.Get().Techs.Get("ImprovedLiquidPiping").unlockedItemIDs.Add(BottleInserterConfig.S_BI_ID);
                Db.Get().Techs.Get("ImprovedLiquidPiping").unlockedItemIDs.Add(BottleFillerConfig.S_BF_ID);
                //find tech with ImprovedGasPiping. Add s_ci_id to that tech.
                Db.Get().Techs.Get("ImprovedGasPiping").unlockedItemIDs.Add(CanisterInserterConfig.S_CI_ID);
            }
        }
    }
}
