using System;
using System.Collections.Generic;
using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;

namespace PackAnything {
  public class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      // TODO 有些东西不能跨星球移动
      // TODO 间歇泉的中子物质得重新生成一下
      // TODO 故事特质建筑移动需要同步
      // TODO OverlayMode 
      base.OnLoad(harmony);
      PUtil.InitLibrary();
      new PVersionCheck().Register(this, new SteamVersionChecker());
      new PLocalization().Register();
      LocString.CreateLocStringKeys(typeof(PackAnythingString), "");
#if DEBUG
      ModUtil.RegisterForTranslation(typeof(PackAnythingString.STRINGS));
#endif
      new POptions().RegisterOptions(this, typeof(Options));
      // ManualPatches(harmony);
    }

    private static void ManualPatches(Harmony harmony) {
      var entityPostfix = typeof(PackAnythingPatches).GetMethod(nameof(PackAnythingPatches.EntityPostfix));
      var entityPatchMap = new Dictionary<Type, string> {
        { typeof(OilWellConfig), nameof(OilWellConfig.CreatePrefab) },
        { typeof(SapTreeConfig), nameof(SapTreeConfig.CreatePrefab) },
        { typeof(PropSurfaceSatellite1Config), nameof(PropSurfaceSatellite1Config.CreatePrefab) },
        { typeof(PropSurfaceSatellite2Config), nameof(PropSurfaceSatellite2Config.CreatePrefab) },
        { typeof(PropSurfaceSatellite3Config), nameof(PropSurfaceSatellite3Config.CreatePrefab) },
        { typeof(WarpReceiverConfig), nameof(WarpReceiverConfig.CreatePrefab) },
        { typeof(WarpPortalConfig), nameof(WarpPortalConfig.CreatePrefab) },
        { typeof(CryoTankConfig), nameof(CryoTankConfig.CreatePrefab) },
        { typeof(BaseBeeHiveConfig), nameof(BaseBeeHiveConfig.CreatePrefab) },
        { typeof(GeneShufflerConfig), nameof(GeneShufflerConfig.CreatePrefab) }
      };
      foreach (var pair in entityPatchMap)
        harmony.Patch(pair.Key.GetMethod(pair.Value), postfix: new HarmonyMethod(entityPostfix));
      var buildingPostfix = typeof(PackAnythingPatches).GetMethod(nameof(PackAnythingPatches.BuildingPostfix));
      var buildingPatchMap = new Dictionary<Type, string> {
        { typeof(LonelyMinionHouseConfig), nameof(LonelyMinionHouseConfig.DoPostConfigureComplete) },
        { typeof(FossilDigSiteConfig), nameof(FossilDigSiteConfig.DoPostConfigureComplete) }, {
          typeof(GravitasCreatureManipulatorConfig), nameof(GravitasCreatureManipulatorConfig.DoPostConfigureComplete)
        },
        { typeof(GravitasContainerConfig), nameof(GravitasContainerConfig.DoPostConfigureComplete) },
        { typeof(TemporalTearOpenerConfig), nameof(TemporalTearOpenerConfig.DoPostConfigureComplete) },
        { typeof(WarpConduitSenderConfig), nameof(WarpConduitSenderConfig.DoPostConfigureComplete) },
        { typeof(WarpConduitReceiverConfig), nameof(WarpConduitReceiverConfig.DoPostConfigureComplete) },
        { typeof(HeadquartersConfig), nameof(HeadquartersConfig.DoPostConfigureComplete) },
        { typeof(MassiveHeatSinkConfig), nameof(MassiveHeatSinkConfig.DoPostConfigureComplete) },
        { typeof(ExobaseHeadquartersConfig), nameof(ExobaseHeadquartersConfig.DoPostConfigureComplete) }
      };
      foreach (var pair1 in buildingPatchMap)
        harmony.Patch(pair1.Key.GetMethod(pair1.Value), postfix: new HarmonyMethod(buildingPostfix));
    }
  }
}