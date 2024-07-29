using System;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace PackAnything.Movable {
  public class NeutroniumMover {
    public int[] neutroniumOffsets;

    public void Move(int cellFrom, int cellTo) {
      if (neutroniumOffsets == null) return;
      foreach (var offset in neutroniumOffsets) {
        DeleteNeutroniumOneCell(Grid.OffsetCell(cellFrom, offset, -1));
        if (!SingletonOptions<Options>.Instance.GenerateUnobtanium) continue;
        AddNeutroniumOneCell(Grid.OffsetCell(cellTo, offset, -1));
      }
    }
    
    public static bool CellIsUnobtanium(int cell) {
      var e = Grid.Element[cell];
      return e.IsSolid && e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM");
    }

    private static void DeleteNeutroniumOneCell(int cell) {
      if (Grid.Element.Length < cell || Grid.Element[cell] == null) {
        PUtil.LogError("Out of index when delete Neutronium");
        throw new IndexOutOfRangeException();
      }
      
      if (!CellIsUnobtanium(cell)) return;
      SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 100f);
    }

    private static void AddNeutroniumOneCell(int cell) {
      if (Grid.Element.Length < cell || Grid.Element[cell] == null) {
        PUtil.LogError("Out of index when add Neutronium");
        throw new IndexOutOfRangeException();
      }

      if (!Grid.IsValidCell(cell)) return;
      SimMessages.ReplaceElement(cell, SimHashes.Unobtanium, CellEventLogger.Instance.DebugTool, 20000f);
    }
  }
}