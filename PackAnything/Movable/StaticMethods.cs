using PeterHan.PLib.Detours;
using UnityEngine;

namespace PackAnything.Movable {
  public class StaticMethods {
    private static readonly IDetouredField<LoreBearer, bool> LoreBareUsed =
      PDetours.DetourField<LoreBearer, bool>("BeenClicked");

    private static readonly IDetouredField<SetLocker, bool> SetLockerUsed =
      PDetours.DetourField<SetLocker, bool>("used");

    public static void ToggleLoreBearer(GameObject origin, GameObject cloned) {
      var originComponent = origin.GetComponent<LoreBearer>();
      var clonedComponent = cloned.GetComponent<LoreBearer>();
      if (originComponent == null || clonedComponent == null) return;
      if (originComponent.SidescreenButtonInteractable()) return;
      LoreBareUsed.Set(clonedComponent, true);
    }
    
    public static Vector3 GetBuildingPosCbc(int cell) => Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);

    public static void MoveOver(GameObject origin, GameObject cloned) {
      cloned.SetActive(true);
      origin.SetActive(false);
      origin.DeleteObject();
    }
  }
}