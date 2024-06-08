using System;
using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using PeterHan.PLib.Options;
using UnityEngine;

namespace PackAnything.Movable {
  public class GeyserMovable : BaseMovable {
    private static readonly IDetouredField<Studyable, bool> studied = PDetours.DetourField<Studyable, bool>("studied");
    private static readonly Tag smallVolcanoTag = new Tag("GeyserGeneric_small_volcano");
    private int unoCount;

    protected override void OnSpawn() {
      base.OnSpawn();
      unoCount = gameObject.PrefabID() == smallVolcanoTag ? 3 : 4;
    }

    public override void Move(int targetCell) {
      base.Move(targetCell);
      var cloned = Util.KInstantiate(Assets.GetPrefab(gameObject.PrefabID()));
      // 同步间歇泉研究状态
      if (gameObject.GetComponent<Studyable>().Studied)
        studied.Set(cloned.AddOrGet<Studyable>(), true);
      // 删除原址中子物质
      DeleteNeutronium(Grid.PosToCell(gameObject.transform.position));
      // 在新址创建中子物质
      if (SingletonOptions<Options>.Instance.GenerateUnobtanium && unoCount > 0)
        CreateNeutronium(Grid.CellBelow(targetCell));
      // 同步数值
      if (SingletonOptions<Options>.Instance.ToggleGeyserAttribute) ToggleGeyser(cloned);
      // 设置间歇泉的位置
      var posCbc = Grid.CellToPosCBC(targetCell, Grid.SceneLayer.BuildingBack);
      posCbc.z -= 0.15f; // 从官方代码复制，不加 z 轴设置间歇泉的位置会异常
      cloned.transform.SetPosition(posCbc);
      cloned.SetActive(true);
      gameObject.DeleteObject();
    }

    public void ToggleGeyser(GameObject cloned) {
      if (gameObject.TryGetComponent(out UserNameable userNameable))
        cloned.AddOrGet<UserNameable>().savedName = userNameable.savedName;

      if (!gameObject.TryGetComponent(out Geyser geyser)) return;
      var clonedGeyser = cloned.AddOrGet<Geyser>();
      clonedGeyser.configuration = geyser.configuration;
    }

    public void CreateNeutronium(int cell) {
      int[] cells = {
        Grid.CellLeft(cell),
        cell,
        Grid.CellRight(cell),
        Grid.CellRight(Grid.CellRight(cell))
      };
      foreach (var x in cells) {
        if (unoCount == 0) continue;
        if (Grid.Element.Length < x || Grid.Element[x] == null) {
          PUtil.LogError("Out of index.");
          throw new IndexOutOfRangeException();
        }

        if (!Grid.IsValidCell(x)) continue;
        SimMessages.ReplaceElement(x, SimHashes.Unobtanium, CellEventLogger.Instance.DebugTool, 20000f);
        unoCount--;
      }
    }

    public void DeleteNeutronium(int cell) {
      int[] cells = {
        Grid.CellDownLeft(cell),
        Grid.CellBelow(cell),
        Grid.CellDownRight(cell),
        Grid.CellRight(Grid.CellDownRight(cell))
      };
      foreach (var x in cells) {
        if (Grid.Element.Length < x || Grid.Element[x] == null) throw new IndexOutOfRangeException();
        var e = Grid.Element[x];
        if (!e.IsSolid || !e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM")) continue;
        SimMessages.ReplaceElement(x, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 100f);
      }
    }

    #region 补丁

    [HarmonyPatch(typeof(GeyserGenericConfig), nameof(GeyserGenericConfig.CreateGeyser))]
    private class GeyserGenericConfig_Patch {
      public static void Postfix(GameObject __result) {
        __result.AddOrGet<GeyserMovable>();
      }
    }

    #endregion
  }
}