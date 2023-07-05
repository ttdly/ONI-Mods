using HarmonyLib;
using PeterHan.PLib.Options;
using static SkyLib.OniUtils;

namespace FixPack.StoragePod {
    public class StoragePodPatch {
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Initialize_Patch {
            public static void Prefix() {
                if (!SingletonOptions<Option>.Instance.ActiveStoragePod) return;
                AddBuildingStrings(
                    StoragePodConfig.ID,
                    StoragePodConfig.DisplayName,
                    StoragePodConfig.Description,
                    StoragePodConfig.Effect);
                AddBuildingStrings(
                    CoolPodConfig.ID,
                    CoolPodConfig.DisplayName,
                    CoolPodConfig.Description,
                    CoolPodConfig.Effect);
            }

            public static void Postfix() {
                if (!SingletonOptions<Option>.Instance.ActiveStoragePod) return;
                AddBuildingToBuildMenu("Base", StoragePodConfig.ID);
                AddBuildingToBuildMenu("Food", CoolPodConfig.ID);
                AddBuildingToTech("RefinedObjects", StoragePodConfig.ID);
                AddBuildingToTech("Agriculture", CoolPodConfig.ID);
            }
        }
    }
}
