using System;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace SpaceStore.SellButtons {
  public class NeutroniumMover {
    
    public static void Delete(int cell) {
      var neutroniumOffsets = new []{-1, 0, 1, 2};
      foreach (var offset in neutroniumOffsets) {
        DeleteNeutroniumOneCell(Grid.OffsetCell(cell, offset, -1));
      }
    }
    
    private static bool CellIsUnobtanium(int cell) {
      var e = Grid.Element[cell];
      return e.IsSolid && e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM");
    }

    private static void DeleteNeutroniumOneCell(int cell) {
      if (Grid.Element.Length < cell || Grid.Element[cell] == null) {
        PUtil.LogError("Out of index when delete Neutronium");
        throw new IndexOutOfRangeException();
      }
      
      if (!CellIsUnobtanium(cell)) return;
      SimMessages.ReplaceElement(cell, SimHashes.Sand, CellEventLogger.Instance.DebugTool, 20000f);
    }
  }
}