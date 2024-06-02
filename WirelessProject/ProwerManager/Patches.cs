using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace WirelessProject.ProwerManager {
  public class Patches {
    public static MethodInfo SetStatuesItem
      = typeof(Generator).GetMethod("SetStatusItem", BindingFlags.NonPublic | BindingFlags.Instance);

    [HarmonyPatch(typeof(EnergyConsumer), "IsConnected", MethodType.Getter)]
    public class EnergyConsumer_IsConnected_Patch {
      public static bool Prefix(EnergyConsumer __instance, ref bool __result) {
        if (__instance.gameObject.HasTag(StaticVar.HasProxyTag)) {
          __result = true;
          return false;
        }

        return true;
      }
    }

    [HarmonyPatch(typeof(Generator), "CheckConnectionStatus")]
    public class Generate_CheckStatues_Patch {
      public static bool Prefix(Generator __instance) {
        if (__instance.gameObject.HasTag(StaticVar.HasProxyTag)) {
          SetStatuesItem.Invoke(__instance, new object[1] { null });
          __instance.gameObject.GetComponent<Operational>().SetFlag(Generator.generatorConnectedFlag, true);
          return false;
        }

        return true;
      }
    }

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
  }
}