using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using System.Collections.Generic;


namespace PackAnything {
    public class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
            new PLocalization().Register();
            LocString.CreateLocStringKeys(typeof(PackAnything.PackAnythingString), "");
            LocString.CreateLocStringKeys(typeof(PackAnything.STRINGS),"");
            ModUtil.RegisterForTranslation(typeof(STRINGS));
            
            this.ManualPatchs(harmony);
        }


        public void ManualPatchs(Harmony harmony) {

            var entityPostfix = typeof(PackAnythingPatches).GetMethod(nameof(PackAnythingPatches.EntityPostfix));
            Dictionary<System.Type, string> entityPatchMap = new Dictionary<System.Type, string> {
                { typeof(OilWellConfig), nameof(OilWellConfig.CreatePrefab) },
                { typeof(SapTreeConfig), nameof(SapTreeConfig.CreatePrefab) },
                { typeof(PropSurfaceSatellite1Config), nameof(PropSurfaceSatellite1Config.CreatePrefab) },
                { typeof(PropSurfaceSatellite2Config), nameof(PropSurfaceSatellite2Config.CreatePrefab) },
                { typeof(PropSurfaceSatellite3Config), nameof(PropSurfaceSatellite3Config.CreatePrefab) },
                { typeof(GeneShufflerConfig), nameof(GeneShufflerConfig.CreatePrefab) },
                { typeof(PropDeskConfig), nameof(PropDeskConfig.CreatePrefab)},
                { typeof(PropGravitasLabTableConfig), nameof(PropGravitasLabTableConfig.CreatePrefab)},
                { typeof(FossilSiteConfig_Ice), nameof(FossilSiteConfig_Ice.CreatePrefab) },
                { typeof(FossilSiteConfig_Resin), nameof(FossilSiteConfig_Resin.CreatePrefab)},
                { typeof(FossilSiteConfig_Rock), nameof(FossilSiteConfig_Rock.CreatePrefab)},
                { typeof(SetLockerConfig), nameof(SetLockerConfig.CreatePrefab) },
                { typeof(PropTableConfig), nameof(PropTableConfig.CreatePrefab) },
                { typeof(VendingMachineConfig), nameof(VendingMachineConfig.CreatePrefab) },
                { typeof(PropGravitasDeskPodiumConfig), nameof(PropGravitasDeskPodiumConfig.CreatePrefab) },
                { typeof(PropGravitasFirstAidKitConfig), nameof(PropGravitasFirstAidKitConfig.CreatePrefab) },
                { typeof(PropGravitasCreaturePosterConfig), nameof(PropGravitasCreaturePosterConfig.CreatePrefab) },
                { typeof(PropClockConfig), nameof(PropClockConfig.CreatePrefab) },
                { typeof(PropFacilityStatueConfig), nameof(PropFacilityStatueConfig.CreatePrefab) },
                { typeof(PropFacilityChandelierConfig), nameof(PropFacilityChandelierConfig.CreatePrefab) },
                { typeof(PropTallPlantConfig), nameof(PropTallPlantConfig.CreatePrefab) },
                { typeof(PropFacilityHangingLightConfig), nameof(PropFacilityHangingLightConfig.CreatePrefab) }

            };
            foreach (KeyValuePair<System.Type, string> pair in entityPatchMap) {
                harmony.Patch(pair.Key.GetMethod(pair.Value), postfix: new HarmonyMethod(entityPostfix));
            }

            var buildingPostfix = typeof(PackAnythingPatches).GetMethod(nameof(PackAnythingPatches.BuildingPostfix));
            Dictionary<System.Type, string> buildingPatchMap = new Dictionary<System.Type, string>() {
                { typeof(LonelyMinionMailboxConfig), nameof(LonelyMinionMailboxConfig.DoPostConfigureComplete) },
                { typeof(LonelyMinionHouseConfig), nameof(LonelyMinionHouseConfig.DoPostConfigureComplete) },
                { typeof(FossilDigSiteConfig), nameof(FossilDigSiteConfig.DoPostConfigureComplete)},
                { typeof(GravitasCreatureManipulatorConfig), nameof(GravitasCreatureManipulatorConfig.DoPostConfigureComplete) },
                { typeof(GravitasContainerConfig), nameof(GravitasContainerConfig.DoPostConfigureComplete) },
            };
            foreach (KeyValuePair<System.Type, string> pair1 in buildingPatchMap) {
                harmony.Patch(pair1.Key.GetMethod(pair1.Value), postfix: new HarmonyMethod(buildingPostfix));
            }
        }
    }
}
