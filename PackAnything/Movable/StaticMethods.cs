using PeterHan.PLib.Detours;
using UnityEngine;

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
  }
}