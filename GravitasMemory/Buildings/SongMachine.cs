using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SongMachine : KMonoBehaviour {
  public int detectionRangeX = 10;
  public int detectionRangeY = 10;
  public bool wasOn;
  private Extents detectionExtents;
  private readonly List<Pickupable> pickup = new List<Pickupable>();
  private HandleVector<int>.Handle pickupableChange;
  private readonly List<int> reachableCells = new List<int>(100);

  protected override void OnPrefabInit() {
    base.OnPrefabInit();
    simRenderLoadBalance = true;
  }

  protected override void OnSpawn() {
    base.OnSpawn();
    var xy = Grid.CellToXY(this.NaturalBuildingCell());
    detectionExtents = new Extents(xy.x - detectionRangeX / 2, xy.y, detectionRangeX + 1, detectionRangeY + 1);
    ;
    pickupableChange = GameScenePartitioner.Instance.Add("SongMachine.Egg", gameObject, detectionExtents,
      GameScenePartitioner.Instance.pickupablesChangedLayer, OnPickupablesChanged);
    RefreshReachableCells();
  }

  protected override void OnCleanUp() {
    GameScenePartitioner.Instance.Free(ref pickupableChange);
    MinionGroupProber.Get().ReleaseProber(this);
    base.OnCleanUp();
  }

  private void RefreshReachableCells() {
    var pooledList = ListPool<int, LogicDuplicantSensor>.Allocate(reachableCells);
    reachableCells.Clear();
    int x;
    int y;
    Grid.CellToXY(this.NaturalBuildingCell(), out x, out y);
    var num = x - detectionRangeX / 2;
    for (var index1 = y; index1 < y + detectionRangeY + 1; ++index1)
    for (var index2 = num; index2 < num + detectionRangeX + 1; ++index2) {
      var cell1 = Grid.XYToCell(index2, index1);
      var offset = new CellOffset(index2 - x, index1 - y);
      if (Grid.IsValidCell(cell1) && Grid.IsPhysicallyAccessible(x, y, index2, index1, true)) reachableCells.Add(cell1);
    }

    pooledList.Recycle();
  }

  private void OnPickupablesChanged(object data) {
    var pickupable = data as Pickupable;
    bool flag;
    ClearList();
    wasOn = pickup.Count >= 1;
    UpdateVisualState(true);
    if (!(bool)(Object)pickupable || !pickupable.KPrefabID.HasTag(GameTags.Egg)) return;
    flag = reachableCells.Contains(pickupable.cachedCell);
    if (flag && pickup.Contains(pickupable)) return;
    var go = pickupable.objectLayerListItem?.gameObject;
    if (!(bool)go) return;
    if (flag) {
      go.GetComponent<Effects>().Add("EggCrazy", true);
      pickup.Add(pickupable);
      wasOn = true;
      UpdateVisualState();
    } else {
      go.GetComponent<Effects>().Remove("EggCrazy");
      pickup.Remove(pickupable);
      wasOn = pickup.Count >= 1;
      UpdateVisualState(true);
    }
  }


  private void ClearList() {
    pickup.RemoveAll(x => x.objectLayerListItem == null);
  }

  private void UpdateVisualState(bool force = false) {
    if (!(wasOn | force)) return;
    var component = GetComponent<KBatchedAnimController>();
    component.Play((HashedString)(wasOn ? "working_pre" : "working_pst"));
    component.Queue((HashedString)(wasOn ? "working_loop" : "off"), KAnim.PlayMode.Loop);
  }
}