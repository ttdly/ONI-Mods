using HarmonyLib;
using UnityEngine;

namespace PackAnything.Movable {
  public class CreatureMovable : BaseMovable{
    public override void Move(int targetCell) {
      base.StableMove(targetCell);
    }
    
    #region 补丁

    [HarmonyPatch(typeof(EntityTemplates), "CreateAndRegisterBaggedCreature")]
    public class Patch_1 {
      private static void Postfix(GameObject creature) {
        Destroy(creature.GetComponent<global::Movable>());
        creature.AddOrGet<CreatureMovable>();
      }
    }
    
    [HarmonyPatch(typeof(MinionConfig), "CreatePrefab")]
    public class Patch_2 {
      private static void Postfix(GameObject __result) {
        Destroy(__result.GetComponent<global::Movable>());
        __result.AddOrGet<CreatureMovable>();
      }
    }
    #endregion
  }
}