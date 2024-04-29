using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WirelessProject.ProwerManager {
    public class Patches {
        #region GeneratorOrBatteries
        [HarmonyPatch(typeof(GeneratorConfig), "DoPostConfigureComplete")]
        public class GeneratorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<GeneratorLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(HydrogenGeneratorConfig), "DoPostConfigureComplete")]
        public class HydrogenGeneratorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<GeneratorLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(MethaneGeneratorConfig), "DoPostConfigureComplete")]
        public class MethaneGeneratorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<GeneratorLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(NuclearReactorConfig), "DoPostConfigureComplete")]
        public class NuclearReactorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<GeneratorLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(PetroleumGeneratorConfig), "DoPostConfigureComplete")]
        public class PetroleumGeneratorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<GeneratorLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(SolarPanelConfig), "DoPostConfigureComplete")]
        public class SolarPanelConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<GeneratorLinkToProxy>();
            }
        }

        //[HarmonyPatch(typeof(SteamTurbineConfig2), "DoPostConfigureComplete")]
        //public class SteamTurbineConfig2_DoPostConfigureComplete_Patch {
        //    public static void Postfix(GameObject go) {
        //        go.AddOrGet<GeneratorLinkToProxy>();
        //    }
        //}

        [HarmonyPatch(typeof(WoodGasGeneratorConfig), "DoPostConfigureComplete")]
        public class WoodGasGeneratorConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<GeneratorLinkToProxy>();
            }
        }


        [HarmonyPatch(typeof(BaseBatteryConfig), "DoPostConfigureComplete")]
        public class BaseBatteryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<BatteryLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(BatterySmartConfig), "DoPostConfigureComplete")]
        public class BatterySmartConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<BatteryLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(BatteryConfig), "DoPostConfigureComplete")]
        public class BatteryConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<BatteryLinkToProxy>();
            }
        }

        [HarmonyPatch(typeof(BatteryMediumConfig), "DoPostConfigureComplete")]
        public class BatteryMediumConfig_DoPostConfigureComplete_Patch {
            public static void Postfix(GameObject go) {
                go.AddOrGet<BatteryLinkToProxy>();
            }
        }
        #endregion

        [HarmonyPatch(typeof(EnergyConsumer), "IsConnected", MethodType.Getter)]
        public class EnergyConsumer_IsConnected_Patch {
            public static bool Prefix(EnergyConsumer __instance, ref bool __result) {
                if (__instance.gameObject.HasTag(GlobalVar.HasProxyTag)) {
                    __result = true;
                    return false;
                }
                return true;
            }
        }



        //[HarmonyPatch(typeof(EnergyGenerator), "EnergySim200ms")]
        //public class ElectricityGenerator_EnergySim200ms_Patch {
        //    public static bool Prefix(float dt, EnergyGenerator __instance, Storage ___storage, MeterController ___meter) {
        //        if (__instance.gameObject.HasTag(GlobalVar.HasProxy)) {
        //            Type type = typeof(Generator);
        //            MethodInfo methodInfo = type.GetMethod("SetStatusItem", BindingFlags.NonPublic | BindingFlags.Instance);
        //            methodInfo.Invoke(__instance, new object[1] { null });
        //            Operational operational = __instance.gameObject.GetComponent<Operational>();

        //            if (__instance.hasMeter) {
        //                InputItem input = __instance.formula.inputs[0];
        //                ___meter.SetPositionPercent(___storage.GetMassAvailable(input.tag) / input.maxStoredMass);
        //            }
        //            bool flag = false;
        //            if (IsConvertible(__instance.formula, dt, ___storage)) {
        //                flag = true;
        //            }
        //            if (GlobalVar.wireConnectedFlag == null) {
        //                GlobalVar.InitFlagMy();
        //            }
        //            operational.SetFlag(Generator.generatorConnectedFlag, true);
        //            operational.SetFlag(GlobalVar.wireConnectedFlag, true);
        //            __instance.GetComponent<Operational>().SetActive(flag);
        //            return false;
        //        }
        //        return true;
        //    }

        //    private static bool IsConvertible(Formula formula, float dt, Storage storage) {
        //        bool flag = true;
        //        InputItem[] inputs = formula.inputs;
        //        for (int i = 0; i < inputs.Length; i++) {
        //            InputItem inputItem = inputs[i];
        //            float massAvailable = storage.GetMassAvailable(inputItem.tag);
        //            float num = inputItem.consumptionRate * dt;
        //            flag = flag && massAvailable >= num;
        //            if (!flag) {
        //                break;
        //            }
        //        }
        //        return flag;
        //    }
        //}





        [HarmonyPatch(typeof(Generator), "CheckConnectionStatus")]
        public class Generate_CheckStatues_Patch {
            public static bool Prefix(Generator __instance) {
                if (__instance.gameObject.HasTag(GlobalVar.HasProxyTag)) {
                    Type type = typeof(Generator);
                    MethodInfo methodInfo = type.GetMethod("SetStatusItem", BindingFlags.NonPublic | BindingFlags.Instance);
                    methodInfo.Invoke(__instance, new object[1] { null });
                    __instance.gameObject.GetComponent<Operational>().SetFlag(Generator.generatorConnectedFlag, true);
                    return false;
                }
                return true;
            }
        }
    }
}
