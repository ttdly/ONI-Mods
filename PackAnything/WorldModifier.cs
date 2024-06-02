using System;
using System.Reflection;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using PeterHan.PLib.Options;
using UnityEngine;

namespace PackAnything {
  public class WorldModifier : KMonoBehaviour {
    [SerializeField] public int cell;

    [SerializeField] public int unoCount;

    private ObjectCanMove OriginCanMoveCompent => PackAnythingStaticVars.MoveStatus.watingMoveObject;

    private GameObject OriginObject => PackAnythingStaticVars.MoveStatus.watingMoveObject.gameObject;

    protected override void OnSpawn() {
      base.OnSpawn();
    }

    public void StartMoving() {
      PackAnythingStaticVars.SetMoving(true);
      OriginCanMoveCompent.RefershObjectType();
      if (OriginCanMoveCompent.objectType == ObjectType.GravitasCreatureManipulator) {
        SetFinalPosition(OriginObject);
      } else {
        CloneOriginObject(out var clonedObject);
        SetFinalPosition(clonedObject);
        clonedObject.SetActive(false);
        clonedObject.SetActive(true);
        OriginCanMoveCompent.DestoryOriginObject();
      }

      PackAnythingStaticVars.SetMoving(false);
    }

    public void SetFinalPosition(GameObject final) {
      var originCell = Grid.PosToCell(OriginObject.transform.position);
      var posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
      if (OriginCanMoveCompent.objectType == ObjectType.Geyser) {
        DeleteNeutronium(originCell);
        if (SingletonOptions<Options>.Instance.GenerateUnobtanium && unoCount > 0) {
          CreateNeutronium(cell);
          cell = Grid.CellAbove(cell);
        }

        posCbc = Grid.CellToPosCBC(cell, OriginObject.FindOrAddComponent<KBatchedAnimController>().sceneLayer);
        posCbc.z -= 0.15f;
      }

      final.transform.SetPosition(posCbc);
    }

    public void CloneOriginObject(out GameObject clonedObject) {
      if (OriginCanMoveCompent.objectType == ObjectType.Geyser ||
          OriginCanMoveCompent.objectType == ObjectType.RadiationEmitter)
        clonedObject = Util.KInstantiate(Assets.GetPrefab(OriginObject.GetComponent<KPrefabID>().PrefabTag));
      else
        clonedObject = GameUtil.KInstantiate(OriginObject, OriginObject.transform.position, Grid.SceneLayer.Building);
      var surveyable = clonedObject.GetComponent<ObjectCanMove>();
      surveyable.objectType = OriginCanMoveCompent.objectType;
      surveyable.isSurveyed = true;
      PackAnythingStaticVars.SurveableCmps.Add(surveyable);
      switch (OriginCanMoveCompent.objectType) {
        case ObjectType.Geyser:
          if (OriginObject.GetComponent<Studyable>().Studied) {
            var detoured = PDetours.DetourField<Studyable, bool>("studied");
            detoured.Set(clonedObject.AddOrGet<Studyable>(), true);
          }

          break;
        case ObjectType.RadiationEmitter:
          ToogleSetLockerr(clonedObject);
          ToogleLoreBearer(clonedObject);
          break;
        case ObjectType.HaveSetLocker:
          if (!OriginObject.GetComponent<SetLocker>().SidescreenButtonInteractable())
            PDetours.DetourField<SetLocker, bool>("used").Set(clonedObject.AddOrGet<SetLocker>(), true);
          ToogleLoreBearer(clonedObject);
          break;
        case ObjectType.CryoTank:
        case ObjectType.HaveLoreBearer:
          ToogleLoreBearer(clonedObject);
          break;
        case ObjectType.WarpPortal:
          var warp = PDetours.DetourField<WarpPortal, bool>("discovered");
          var origin = warp.Get(OriginObject.GetComponent<WarpPortal>());
          warp.Set(clonedObject.AddOrGet<WarpPortal>(), origin);
          ToogleLoreBearer(clonedObject);
          break;
        case ObjectType.WarpReceiver:
          clonedObject.AddOrGet<WarpReceiver>().Used = OriginObject.GetComponent<WarpReceiver>().Used;
          ToogleLoreBearer(clonedObject);
          break;
        case ObjectType.Activatable:
          if (OriginObject.GetComponent<Activatable>().IsActivated) {
            var activatable = clonedObject.AddOrGet<Activatable>();
            var type = typeof(Activatable);
            var methodInfo = type.GetMethod("OnCompleteWork", BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo != null) _ = methodInfo.Invoke(activatable, new object[1] { null });
          }

          break;
      }
    }

    public void ToogleSetLockerr(GameObject needToogle) {
      if (OriginObject.GetComponent<SetLocker>() != null)
        if (!OriginObject.GetComponent<SetLocker>().SidescreenButtonInteractable())
          PDetours.DetourField<SetLocker, bool>("used").Set(needToogle.AddOrGet<SetLocker>(), true);
    }

    public void ToogleLoreBearer(GameObject needToogle) {
      if (OriginObject.GetComponent<LoreBearer>() != null)
        if (!OriginObject.GetComponent<LoreBearer>().SidescreenButtonInteractable())
          PDetours.DetourField<LoreBearer, bool>("BeenClicked").Set(needToogle.AddOrGet<LoreBearer>(), true);
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
          new IndexOutOfRangeException();
          return;
        }

        if (!Grid.IsValidCell(x)) continue;
        SimMessages.ReplaceElement(x, SimHashes.Unobtanium, CellEventLogger.Instance.DebugTool, 100f);
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
        if (Grid.Element.Length < x || Grid.Element[x] == null) {
          new IndexOutOfRangeException();
          return;
        }

        var e = Grid.Element[x];
        if (!e.IsSolid || !e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM")) continue;
        SimMessages.ReplaceElement(x, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 100f);
        unoCount++;
      }
    }
  }
}