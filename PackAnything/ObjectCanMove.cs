using KSerialization;
using STRINGS;
using UnityEngine;

namespace PackAnything {
  public class ObjectCanMove : KMonoBehaviour {
    private static readonly EventSystem.IntraObjectHandler<ObjectCanMove> OnRefreshUserMenuDelegate =
      new EventSystem.IntraObjectHandler<ObjectCanMove>((component, data) => component.OnRefreshUserMenu(data));

    [Serialize] public bool isSurveyed;

    [Serialize] public ObjectType objectType;

    private WorldModifier buffer;

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
      Subscribe((int)GameHashes.StatusChange, OnRefreshUserMenuDelegate);
      if (isSurveyed) {
        PackAnythingStaticVars.SurveableCmps.Add(this);
      }
    }

    protected override void OnCleanUp() {
      base.OnCleanUp();
      PackAnythingStaticVars.SurveableCmps.Remove(this);
    }

    // 自定义的方法
    public void OnRefreshUserMenu(object _) {
      if (gameObject.HasTag("OilWell") &&
          gameObject.GetComponent<BuildingAttachPoint>()?.points[0].attachedBuilding != null) return;
      //RemoveThisFromList();
      Game.Instance.userMenu.AddButton(
        gameObject,
        new KIconButtonMenu.ButtonInfo("action_control", UI.USERMENUACTIONS.PICKUPABLEMOVE.NAME, OnClickMove,
          tooltipText: UI.USERMENUACTIONS.PICKUPABLEMOVE.TOOLTIP));
      if (isSurveyed) return;
      if (PackAnythingStaticVars.IsDlc)
        Game.Instance.userMenu.AddButton(
          gameObject,
          new KIconButtonMenu.ButtonInfo(
            "action_follow_cam",
            PackAnythingString.UI.SURVEY.NAME,
            AddThisToList,
            tooltipText: PackAnythingString.UI.SURVEY.TOOLTIP)
        );
    }

    private void OnClickMove() {
      if (buffer == null) {
        var go = new GameObject("Proxy");
        buffer = go.AddComponent<WorldModifier>();
      }

      PackAnythingStaticVars.SetTargetObjectCanMove(this);
      PackAnythingStaticVars.SetTargetModifier(buffer);
      ActiveMoveTool(this);
      isSurveyed = false;
    }


    // ---- Copy from SideScreen ---

    public void ActiveMoveTool(ObjectCanMove objectCanMove) {
      switch (objectCanMove.gameObject.GetComponent<KPrefabID>().PrefabTag.ToString()) {
        case "LonelyMinionHouse":
          LonyMinionActive(objectCanMove);
          break;
        default:
          NormalActive(objectCanMove);
          break;
      }
    }

    private void NormalActive(ObjectCanMove objectCanMove) {
      MoveTargetTool.Instance.Acitvate(objectCanMove);
    }

    private void LonyMinionActive(ObjectCanMove objectCanMove) {
      var template = TemplateCache.GetTemplate("only_loney");
      GameObject box;
      if (template != null && template.cells != null) {
        var cell = Grid.PosToCell(objectCanMove.gameObject);
        var pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
        GameScenePartitioner.Instance.GatherEntries(new Extents(cell, 10),
          GameScenePartitioner.Instance.objectLayers[1], pooledList);
        var num = 0;
        while (num < pooledList.Count) {
          if ((pooledList[num].obj as GameObject).GetComponent<KPrefabID>().PrefabTag.GetHash() ==
              LonelyMinionMailboxConfig.IdHash.HashValue) {
            box = pooledList[num].obj as GameObject;
            MoveStoryTargetTool.Instance.Activate(template, new GameObject[2] { objectCanMove.gameObject, box },
              DeactivateOnStamp: true);
            return;
          }

          num++;
        }
      }
    }

    // ---- End Copy ---

    public void AddThisToList() {
      isSurveyed = true;
      PackAnythingStaticVars.SurveableCmps.Add(this);
    }

    public void RemoveThisFromList() {
      isSurveyed = false;
      PackAnythingStaticVars.SurveableCmps.Remove(this);
    }

    public void RefershObjectType() {
      objectType = GetObjectType();
#if DEBUG
            PUtil.LogDebug($"{gameObject.GetProperName()}::ObjectType::{objectType}");
#endif
    }

    private ObjectType GetObjectType() {
      if (objectType != ObjectType.None) return objectType;
      switch (gameObject.GetComponent<KPrefabID>().PrefabTag.ToString()) {
        case "WarpReceiver":
          return ObjectType.WarpReceiver;
        case "GravitasCreatureManipulator":
          return ObjectType.GravitasCreatureManipulator;
        case "WarpPortal":
          return ObjectType.WarpPortal;
      }

      if (gameObject.GetComponent<RadiationEmitter>() != null) return ObjectType.RadiationEmitter;
      if (gameObject.GetComponent<SetLocker>() != null) return ObjectType.HaveSetLocker;
      if (gameObject.GetComponent<LoreBearer>() != null) return ObjectType.HaveLoreBearer;
      if (gameObject.GetComponent<Activatable>() != null) return ObjectType.Activatable;
      return ObjectType.None;
    }

    public GameObject CloneOriginObject() {
      return gameObject;
    }

    public void DestoryOriginObject() {
      gameObject.SetActive(false);
      gameObject.DeleteObject();
    }
  }
}