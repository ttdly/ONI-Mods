using HarmonyLib;
using UnityEngine;

namespace WirelessProject.ProwerManager {
    public class ConsumerPatches {
        [HarmonyPatch(typeof(AdvancedDoctorStationConfig), "DoPostConfigureComplete")]
        public class AdvancedDoctorStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(AdvancedResearchCenterConfig), "DoPostConfigureComplete")]
        public class AdvancedResearchCenterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(AirConditionerConfig), "DoPostConfigureComplete")]
        public class AirConditionerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(AirFilterConfig), "DoPostConfigureComplete")]
        public class AirFilterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(AlgaeDistilleryConfig), "DoPostConfigureComplete")]
        public class AlgaeDistilleryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ApothecaryConfig), "DoPostConfigureComplete")]
        public class ApothecaryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ArcadeMachineConfig), "DoPostConfigureComplete")]
        public class ArcadeMachineConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ArtifactAnalysisStationConfig), "DoPostConfigureComplete")]
        public class ArtifactAnalysisStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(AstronautTrainingCenterConfig), "DoPostConfigureComplete")]
        public class AstronautTrainingCenterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(AtmoicGardenConfig), "DoPostConfigureComplete")]
        public class AtmoicGardenConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(AutoMinerConfig), "DoPostConfigureComplete")]
        public class AutoMinerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(BaseModularLaunchpadPortConfig), "DoPostConfigureComplete")]
        public class BaseModularLaunchpadPortConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(BunkerDoorConfig), "DoPostConfigureComplete")]
        public class BunkerDoorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(CeilingLightConfig), "DoPostConfigureComplete")]
        public class CeilingLightConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(CheckpointConfig), "DoPostConfigureComplete")]
        public class CheckpointConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ChlorinatorConfig), "DoPostConfigureComplete")]
        public class ChlorinatorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ClothingAlterationStationConfig), "DoPostConfigureComplete")]
        public class ClothingAlterationStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ClothingFabricatorConfig), "DoPostConfigureComplete")]
        public class ClothingFabricatorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ClusterTelescopeConfig), "DoPostConfigureComplete")]
        public class ClusterTelescopeConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ClusterTelescopeEnclosedConfig), "DoPostConfigureComplete")]
        public class ClusterTelescopeEnclosedConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(CO2ScrubberConfig), "DoPostConfigureComplete")]
        public class CO2ScrubberConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(CometDetectorConfig), "DoPostConfigureComplete")]
        public class CometDetectorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(CookingStationConfig), "DoPostConfigureComplete")]
        public class CookingStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(CosmicResearchCenterConfig), "DoPostConfigureComplete")]
        public class CosmicResearchCenterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(CraftingTableConfig), "DoPostConfigureComplete")]
        public class CraftingTableConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(CrewCapsuleConfig), "DoPostConfigureComplete")]
        public class CrewCapsuleConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(DesalinatorConfig), "DoPostConfigureComplete")]
        public class DesalinatorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(DiamondPressConfig), "DoPostConfigureComplete")]
        public class DiamondPressConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(DLC1CosmicResearchCenterConfig), "DoPostConfigureComplete")]
        public class DLC1CosmicResearchCenterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(EggIncubatorConfig), "DoPostConfigureComplete")]
        public class EggIncubatorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ElectrolyzerConfig), "DoPostConfigureComplete")]
        public class ElectrolyzerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(EspressoMachineConfig), "DoPostConfigureComplete")]
        public class EspressoMachineConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(EthanolDistilleryConfig), "DoPostConfigureComplete")]
        public class EthanolDistilleryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(FertilizerMakerConfig), "DoPostConfigureComplete")]
        public class FertilizerMakerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(FloorLampConfig), "DoPostConfigureComplete")]
        public class FloorLampConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GantryConfig), "DoPostConfigureComplete")]
        public class GantryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GasFilterConfig), "DoPostConfigureComplete")]
        public class GasFilterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GasLimitValveConfig), "DoPostConfigureComplete")]
        public class GasLimitValveConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GasLogicValveConfig), "DoPostConfigureComplete")]
        public class GasLogicValveConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GasMiniPumpConfig), "DoPostConfigureComplete")]
        public class GasMiniPumpConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GasPumpConfig), "DoPostConfigureComplete")]
        public class GasPumpConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GenericFabricatorConfig), "DoPostConfigureComplete")]
        public class GenericFabricatorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GeneticAnalysisStationConfig), "DoPostConfigureComplete")]
        public class GeneticAnalysisStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GeoTunerConfig), "DoPostConfigureComplete")]
        public class GeoTunerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GlassForgeConfig), "DoPostConfigureComplete")]
        public class GlassForgeConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(GourmetCookingStationConfig), "DoPostConfigureComplete")]
        public class GourmetCookingStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(HeatCompressorConfig), "DoPostConfigureComplete")]
        public class HeatCompressorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(HEPBatteryConfig), "DoPostConfigureComplete")]
        public class HEPBatteryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(HighEnergyParticleSpawnerConfig), "DoPostConfigureComplete")]
        public class HighEnergyParticleSpawnerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(HotTubConfig), "DoPostConfigureComplete")]
        public class HotTubConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(IceMachineConfig), "DoPostConfigureComplete")]
        public class IceMachineConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(JetSuitLockerConfig), "DoPostConfigureComplete")]
        public class JetSuitLockerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(JuicerConfig), "DoPostConfigureComplete")]
        public class JuicerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LandingBeaconConfig), "DoPostConfigureComplete")]
        public class LandingBeaconConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LeadSuitLockerConfig), "DoPostConfigureComplete")]
        public class LeadSuitLockerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LiquidConditionerConfig), "DoPostConfigureComplete")]
        public class LiquidConditionerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LiquidFilterConfig), "DoPostConfigureComplete")]
        public class LiquidFilterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LiquidHeaterConfig), "DoPostConfigureComplete")]
        public class LiquidHeaterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LiquidLimitValveConfig), "DoPostConfigureComplete")]
        public class LiquidLimitValveConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LiquidLogicValveConfig), "DoPostConfigureComplete")]
        public class LiquidLogicValveConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LiquidMiniPumpConfig), "DoPostConfigureComplete")]
        public class LiquidMiniPumpConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LiquidPumpConfig), "DoPostConfigureComplete")]
        public class LiquidPumpConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LogicHammerConfig), "DoPostConfigureComplete")]
        public class LogicHammerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(LonelyMinionHouseConfig), "DoPostConfigureComplete")]
        public class LonelyMinionHouseConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MassageTableConfig), "DoPostConfigureComplete")]
        public class MassageTableConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MechanicalSurfboardConfig), "DoPostConfigureComplete")]
        public class MechanicalSurfboardConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MetalRefineryConfig), "DoPostConfigureComplete")]
        public class MetalRefineryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MicrobeMusherConfig), "DoPostConfigureComplete")]
        public class MicrobeMusherConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MilkFatSeparatorConfig), "DoPostConfigureComplete")]
        public class MilkFatSeparatorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MilkPressConfig), "DoPostConfigureComplete")]
        public class MilkPressConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MineralDeoxidizerConfig), "DoPostConfigureComplete")]
        public class MineralDeoxidizerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MissileFabricatorConfig), "DoPostConfigureComplete")]
        public class MissileFabricatorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MissileLauncherConfig), "DoPostConfigureComplete")]
        public class MissileLauncherConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MissionControlClusterConfig), "DoPostConfigureComplete")]
        public class MissionControlClusterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MissionControlConfig), "DoPostConfigureComplete")]
        public class MissionControlConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(NuclearResearchCenterConfig), "DoPostConfigureComplete")]
        public class NuclearResearchCenterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ObjectDispenserConfig), "DoPostConfigureComplete")]
        public class ObjectDispenserConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(OilRefineryConfig), "DoPostConfigureComplete")]
        public class OilRefineryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(OilWellCapConfig), "DoPostConfigureComplete")]
        public class OilWellCapConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(OrbitalResearchCenterConfig), "DoPostConfigureComplete")]
        public class OrbitalResearchCenterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(OxygenMaskStationConfig), "DoPostConfigureComplete")]
        public class OxygenMaskStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(OxyliteRefineryConfig), "DoPostConfigureComplete")]
        public class OxyliteRefineryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(PhonoboxConfig), "DoPostConfigureComplete")]
        public class PhonoboxConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(PixelPackConfig), "DoPostConfigureComplete")]
        public class PixelPackConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(PolymerizerConfig), "DoPostConfigureComplete")]
        public class PolymerizerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(PressureDoorConfig), "DoPostConfigureComplete")]
        public class PressureDoorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(RadiationLightConfig), "DoPostConfigureComplete")]
        public class RadiationLightConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(RailGunConfig), "DoPostConfigureComplete")]
        public class RailGunConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(RailGunPayloadOpenerConfig), "DoPostConfigureComplete")]
        public class RailGunPayloadOpenerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(RefrigeratorConfig), "DoPostConfigureComplete")]
        public class RefrigeratorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ResearchCenterConfig), "DoPostConfigureComplete")]
        public class ResearchCenterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ResetSkillsStationConfig), "DoPostConfigureComplete")]
        public class ResetSkillsStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(RockCrusherConfig), "DoPostConfigureComplete")]
        public class RockCrusherConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(RocketInteriorGasOutputConfig), "DoPostConfigureComplete")]
        public class RocketInteriorGasOutputConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(RocketInteriorLiquidOutputConfig), "DoPostConfigureComplete")]
        public class RocketInteriorLiquidOutputConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(RocketInteriorSolidOutputConfig), "DoPostConfigureComplete")]
        public class RocketInteriorSolidOutputConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(RustDeoxidizerConfig), "DoPostConfigureComplete")]
        public class RustDeoxidizerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SaunaConfig), "DoPostConfigureComplete")]
        public class SaunaConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(ShearingStationConfig), "DoPostConfigureComplete")]
        public class ShearingStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SludgePressConfig), "DoPostConfigureComplete")]
        public class SludgePressConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SodaFountainConfig), "DoPostConfigureComplete")]
        public class SodaFountainConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SolidConduitInboxConfig), "DoPostConfigureComplete")]
        public class SolidConduitInboxConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SolidFilterConfig), "DoPostConfigureComplete")]
        public class SolidFilterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SolidLimitValveConfig), "DoPostConfigureComplete")]
        public class SolidLimitValveConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SolidLogicValveConfig), "DoPostConfigureComplete")]
        public class SolidLogicValveConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SolidTransferArmConfig), "DoPostConfigureComplete")]
        public class SolidTransferArmConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SpaceHeaterConfig), "DoPostConfigureComplete")]
        public class SpaceHeaterConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(StorageLockerSmartConfig), "DoPostConfigureComplete")]
        public class StorageLockerSmartConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SublimationStationConfig), "DoPostConfigureComplete")]
        public class SublimationStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SuitFabricatorConfig), "DoPostConfigureComplete")]
        public class SuitFabricatorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SuitLockerConfig), "DoPostConfigureComplete")]
        public class SuitLockerConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SunLampConfig), "DoPostConfigureComplete")]
        public class SunLampConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SupermaterialRefineryConfig), "DoPostConfigureComplete")]
        public class SupermaterialRefineryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SweepBotStationConfig), "DoPostConfigureComplete")]
        public class SweepBotStationConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(TelephoneConfig), "DoPostConfigureComplete")]
        public class TelephoneConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(TeleportalPadConfig), "DoPostConfigureComplete")]
        public class TeleportalPadConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(TelescopeConfig), "DoPostConfigureComplete")]
        public class TelescopeConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(TravelTubeEntranceConfig), "DoPostConfigureComplete")]
        public class TravelTubeEntranceConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(UraniumCentrifugeConfig), "DoPostConfigureComplete")]
        public class UraniumCentrifugeConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(VerticalWindTunnelConfig), "DoPostConfigureComplete")]
        public class VerticalWindTunnelConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(WaterPurifierConfig), "DoPostConfigureComplete")]
        public class WaterPurifierConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<ConsumerLinkToProxy>();
            }
        }

    }
}
