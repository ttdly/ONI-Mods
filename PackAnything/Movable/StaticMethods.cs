using System;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PackAnything.Movable {
  public class StaticMethods {
    private static readonly IDetouredField<LoreBearer, bool> LoreBareUsed =
      PDetours.DetourField<LoreBearer, bool>("BeenClicked");

    private static readonly IDetouredField<SetLocker, bool> SetLockerUsed =
      PDetours.DetourField<SetLocker, bool>("used");

    public static void ToggleLoreBearer(GameObject origin, GameObject cloned) {
      if (origin == null || cloned == null) return;
      var originComponent = origin.GetComponent<LoreBearer>();
      var clonedComponent = cloned.GetComponent<LoreBearer>();
      ToggleLoreBearer(originComponent, clonedComponent);
    }

    public static void ToggleLoreBearer(LoreBearer origin, LoreBearer cloned) {
      if (origin == null || cloned == null) return;
      if (origin.SidescreenButtonInteractable()) return;
      LoreBareUsed.Set(cloned, true);
    }

    public static void ToggleSetLocker(GameObject origin, GameObject cloned) {
      if (origin == null || cloned == null) return;
      var originComponent = origin.GetComponent<SetLocker>();
      var clonedComponent = cloned.GetComponent<SetLocker>();
      ToggleSetLocker(originComponent, clonedComponent);
    }

    public static void ToggleSetLocker(SetLocker origin, SetLocker cloned) {
      if (origin == null || cloned == null) return;
      if (origin.SidescreenButtonInteractable()) return;
      SetLockerUsed.Set(cloned, true);
    }

    public static Vector3 GetBuildingPosCbc(int cell) {
      return Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
    }

    public static void MoveOver(GameObject origin, GameObject cloned) {
      cloned.SetActive(false);
      cloned.SetActive(true);
      origin.SetActive(false);
      origin.DeleteObject();
    }

    public static void RemoveGravitiesAndAddMovable<T>(GameObject go) where T : BaseMovable {
      if (go.TryGetComponent(out GravitiesMovable component)) Object.DestroyImmediate(component);
      go.AddOrGet<T>();
    }

    public static void DeleteNeutroniumOneCell(int cell) {
      if (Grid.Element.Length < cell || Grid.Element[cell] == null) {
        PUtil.LogError("Out of index when delete Neutronium");
        throw new IndexOutOfRangeException();
      }

      var e = Grid.Element[cell];
      if (!e.IsSolid || !e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM")) return;
      SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 100f);
    }

    public static void AddNeutroniumOneCell(int cell) {
      if (Grid.Element.Length < cell || Grid.Element[cell] == null) {
        PUtil.LogError("Out of index when add Neutronium");
        throw new IndexOutOfRangeException();
      }

      if (!Grid.IsValidCell(cell)) return;
      SimMessages.ReplaceElement(cell, SimHashes.Unobtanium, CellEventLogger.Instance.DebugTool, 20000f);
    }
  }
}