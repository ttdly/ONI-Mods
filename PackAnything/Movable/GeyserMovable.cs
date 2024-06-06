using System;
using PackAnything.MoveTool;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using PeterHan.PLib.Options;

namespace PackAnything.Movable {
  public class GeyserMovable : BaseMovable {
    private static IDetouredField<Studyable, bool> studied = null;
    private int unoCount;

    protected override void OnSpawn() {
      base.OnSpawn();
      if (studied != null) return;
      studied = PDetours.DetourField<Studyable, bool>("studied");
    }

    protected override void Move(int targetCell) {
      base.MovePrepare(targetCell);
      var cloned = Util.KInstantiate(Assets.GetPrefab(gameObject.PrefabID()));
      // 同步间歇泉研究状态
      if (gameObject.GetComponent<Studyable>().Studied)
        studied.Set(cloned.AddOrGet<Studyable>(), true);
      // 删除原址中子物质
      DeleteNeutronium(Grid.PosToCell(gameObject.transform.position));
      if (SingletonOptions<Options>.Instance.GenerateUnobtanium && unoCount > 0) {
        // 在新址创建中子物质
        CreateNeutronium(targetCell);
        targetCell = Grid.CellAbove(targetCell);
      }

      // 设置间歇泉的位置
      var posCbc = Grid.CellToPosCBC(targetCell, gameObject.GetComponent<KBatchedAnimController>().sceneLayer);
      posCbc.z -= 0.15f; // 从官方代码复制，不加 z 轴设置间歇泉的位置会异常
      cloned.transform.SetPosition(posCbc);
      cloned.SetActive(true);
      gameObject.DeleteObject();
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
      unoCount = 0;
      foreach (var x in cells) {
        if (Grid.Element.Length < x || Grid.Element[x] == null) throw new IndexOutOfRangeException();
        var e = Grid.Element[x];
        if (!e.IsSolid || !e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM")) continue;
        SimMessages.ReplaceElement(x, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 100f);
        unoCount++;
      }
    }
  }
}