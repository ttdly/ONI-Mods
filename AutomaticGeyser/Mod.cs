using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using UnityEngine;

namespace AutomaticGeyser {
  public sealed class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      // 初始化 PUtil 的文件
      PUtil.InitLibrary();
      // 检查模组版本是否更新
      ModUtil.RegisterForTranslation(typeof(ModStrings));
      new PVersionCheck().Register(this, new SteamVersionChecker());
      new PLocalization().Register();
    }
  }
  
  [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
  public class GeyserGenericConfig_Patch {
    public static void Postfix(GameObject __result) {
      __result.AddOrGet<GeyserLogic>();
    }
  }

  [HarmonyPatch(typeof(Geyser.States), "InitializeStates")]
  public class Geyser_States_InitializeStates {
    public static void Postfix(Geyser.States __instance) {
      
      __instance.pre_erupt.Enter(smi => {
        if (!smi.master.gameObject.TryGetComponent(out GeyserLogic geyserLogic)) return;
        geyserLogic.SendStatus(GeyserLogicStatus.OutputLogic.PreErupt);
        geyserLogic.AlwaysDormant();
      });
      
      __instance.erupt.Enter(smi => {
        if (!smi.master.gameObject.TryGetComponent(out GeyserLogic geyserLogic)) return;
        geyserLogic.SkipErupt();
      });
      
      __instance.post_erupt.Enter(smi => {
        if (!smi.master.gameObject.TryGetComponent(out GeyserLogic geyserLogic)) return;
        geyserLogic.SendStatus(GeyserLogicStatus.OutputLogic.PostErupt);
      });
      
      __instance.erupt.erupting.Enter(smi => {
        if (!smi.master.gameObject.TryGetComponent(out GeyserLogic geyserLogic)) return;
        geyserLogic.SendStatus(GeyserLogicStatus.OutputLogic.Erupting);
      });
      
      __instance.erupt.overpressure.Enter(smi => {
        if (!smi.master.gameObject.TryGetComponent(out GeyserLogic geyserLogic)) return;
        geyserLogic.SendStatus(GeyserLogicStatus.OutputLogic.OverPressure);
      });
      
      __instance.idle.Enter(smi => {
        if (!smi.master.gameObject.TryGetComponent(out GeyserLogic geyserLogic)) return;
        geyserLogic.SendStatus(GeyserLogicStatus.OutputLogic.Dormant);
        geyserLogic.SkipIdle();
      });

      __instance.dormant.Enter(smi => {
        if (!smi.master.gameObject.TryGetComponent(out GeyserLogic geyserLogic)) return;
        geyserLogic.SendStatus(GeyserLogicStatus.OutputLogic.Dormant);
        geyserLogic.SkipDormant();
      });
    }
  }

  [HarmonyPatch(typeof(LogicPorts), "OnSpawn")]
  public class LogicPorts_OnSpawn_Patches {
    public static void Prefix(LogicPorts __instance) {
      AccessTools.Field(typeof(LogicPorts), "isPhysical")
        .SetValue(__instance, __instance.gameObject.GetComponent<Geyser>() != null);
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
      var codes = new List<CodeInstruction>(instructions);
      for (var i = 0; i < codes.Count; i++)
        if (codes[i].opcode == OpCodes.Stfld && ((FieldInfo)codes[i].operand).Name == "isPhysical") {
          codes.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
          codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldfld,
            AccessTools.Field(typeof(LogicPorts), "isPhysical")));
          codes.Insert(i + 2, new CodeInstruction(OpCodes.Or));
          i += 3;
        }

      return codes.AsEnumerable();
    }
  }
  
  [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
  public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
    public static void Prefix() {
      LocString.CreateLocStringKeys(typeof(ModStrings.GEYSER));
    }
  }
}