using System;
using HarmonyLib;
using PeterHan.PLib.Core;
using UnityEngine;

namespace PipeGeyser {
    public class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
        }
    }

    public class Patch {
        [HarmonyPatch(typeof(GeyserGenericConfig))]
        [HarmonyPatch(nameof(GeyserGenericConfig.CreateGeyser))]
        [HarmonyPatch(new Type[] {
            typeof(string), typeof(string), typeof(int), typeof(int), typeof(string), typeof(string),
            typeof(HashedString), typeof(float), typeof(string[]), typeof(string[])
        })]
        public class GeyserGenericConfigCreateGeyserPatch {
            public static void Postfix(GameObject __result, HashedString presetType) {

                var storage = __result.AddOrGet<Storage>();
                storage.showInUI = true;
                storage.capacityKg = 2000f;
                storage.allowItemRemoval = false;
                storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);

                __result.AddOrGet<PipeGeyser>().storage = storage;

                var conduit = __result.AddOrGet<ConduitDispenser>();
                var geyserType = GeyserConfigurator.FindType(presetType);
                conduit.conduitType =
                    geyserType.shape == GeyserConfigurator.GeyserShape.Gas ? ConduitType.Gas : ConduitType.Liquid;
                conduit.alwaysDispense = true;
                conduit.noBuildingOutputCellOffset = new CellOffset(0, 1);
                conduit.SetOnState(true);
            }
        }


        [HarmonyPatch(typeof(ElementEmitter), "SetEmitting")]
        public class ElementEmitterSetEmittingPatch {
            public static void Postfix(ElementEmitter __instance, bool emitting) {
                __instance.GetComponent<PipeGeyser>().close = !emitting;
            }
        }

        [HarmonyPatch(typeof(ElementEmitter), "OnSimActivate")]
        public class ElementEmitterOnSimActivatePatch {
            public static bool Prefix(ElementEmitter __instance) {
                if (!__instance.outputElement.storeOutput) return true;
                var storage = __instance.GetComponent<Storage>();
                __instance.gameObject.AddComponent<PipeGeyser>().close = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(ElementEmitter), "OnSimDeactivate")]
        public class ElementEmitterOnSimDeactivatePatch {
            public static bool Prefix(ElementEmitter __instance) {
                if (!__instance.outputElement.storeOutput) return true;
                __instance.gameObject.AddComponent<PipeGeyser>().close = true;
                return false;
            }
        }


        [HarmonyPatch(typeof(Geyser), "ApplyConfigurationEmissionValues")]
        public class GeyserApplyConfigurationEmissionValuesPatch {
            public static bool Prefix(Geyser __instance, GeyserConfigurator.GeyserInstanceConfiguration config) {
                var emitter = __instance.gameObject.GetComponent<ElementEmitter>();
                var pipeGeyser = __instance.gameObject.GetComponent<PipeGeyser>();
                var output = new ElementConverter.OutputElement(
                    config.GetEmitRate(),
                    config.GetElement(),
                    config.GetTemperature(),
                    storeOutput: true,
                    addedDiseaseIdx: config.GetDiseaseIdx(),
                    addedDiseaseCount: Mathf.RoundToInt(config.GetDiseaseCount() * config.GetEmitRate())
                );
                pipeGeyser.outputElement = output;
                emitter.emitRange = 2;
                emitter.maxPressure = config.GetMaxPressure();
                emitter.outputElement = output;

                if (!emitter.IsSimActive)
                    return false;
                if (pipeGeyser.close)
                    pipeGeyser.close = false;
                emitter.SetSimActive(true);
                return false;
            }
        }


        [HarmonyPatch(typeof(Geyser), "OnSpawn")]
        public class GeyserOnSpawnPatch {
            public static void Postfix(Geyser __instance) {
                var entityCellVisualizer = __instance.gameObject.AddOrGet<EntityCellVisualizer>();
                var potType = __instance.configuration.geyserType.shape == GeyserConfigurator.GeyserShape.Gas
                    ? EntityCellVisualizer.Ports.GasOut
                    : EntityCellVisualizer.Ports.LiquidOut;
                entityCellVisualizer.AddPort(potType, new CellOffset(0, 1),
                    entityCellVisualizer.Resources.liquidIOColours.output.connected);
            }
        }
    }
}
