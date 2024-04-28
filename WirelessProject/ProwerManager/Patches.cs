using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Reflection;
using static EnergyGenerator;

namespace WirelessProject.ProwerManager {
    public class Patches {
        [HarmonyPatch(typeof(EnergyConsumer), "OnSpawn")]
        public class EnergyConsumer_OnSpawn_Patch {
            public static void Postfix(EnergyConsumer __instance) {
                __instance.gameObject.AddOrGet<ProxyLink>().type = ProxyLink.ProwerType.Consumer;
            }
        }

        [HarmonyPatch(typeof(EnergyConsumer), "IsConnected", MethodType.Getter)]
        public class EnergyConsumer_IsConnected_Patch {
            public static bool Prefix(EnergyConsumer __instance, ref bool __result) {
                if (__instance.gameObject.HasTag(GlobalVar.HasProxy)) {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Battery), "OnSpawn")]
        public class Battery_Patch {
            public static void Postfix(Battery __instance) {
                __instance.gameObject.AddOrGet<ProxyLink>().type = ProxyLink.ProwerType.Battery;
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



        [HarmonyPatch(typeof(Generator), "OnSpawn")]
        public class Generator_Patch {
            public static void Postfix(Generator __instance) {
                __instance.gameObject.AddOrGet<ProxyLink>().type = ProxyLink.ProwerType.Generator;
            }
        }

        [HarmonyPatch(typeof(Generator), "CheckConnectionStatus")]
        public class Generate_CheckStatues_Patch {
            public static bool Prefix(Generator __instance) {
                if (__instance.gameObject.HasTag(GlobalVar.HasProxy)) {
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
