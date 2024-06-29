using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using UnityEngine;

namespace AutomaticGeyser {
  public sealed class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      // 初始化 PUtil 的文件
      PUtil.InitLibrary();
      // 检查模组版本是否更新
      new PVersionCheck().Register(this, new SteamVersionChecker());
    }
  }
  // TODO 在复制人工作之后才能有自动化端口
  // TODO 累积跳过喷发的次数
  [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
  public class GeyserGenericConfig_Patch {
    public static void Postfix(GameObject __result) {
      __result.AddOrGet<GeyserLogic>();
      GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, __result.PrefabID().ToString());
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
}