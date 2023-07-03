using Klei.AI;
using KSerialization;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SongMachine : KMonoBehaviour {
    public int detectionRangeX = 10;
    public int detectionRangeY = 10;
    public bool wasOn;
    private List<Pickupable> pickup = new List<Pickupable>();
    private HandleVector<int>.Handle pickupableChange;
    private Extents detectionExtents;
    private List<int> reachableCells = new List<int>(100);

    protected override void OnPrefabInit() {
        base.OnPrefabInit();
        this.simRenderLoadBalance = true;
    }

    protected override void OnSpawn() {
        base.OnSpawn();
        Vector2I xy = Grid.CellToXY(this.NaturalBuildingCell());
        this.detectionExtents = new Extents(xy.x - (detectionRangeX / 2), xy.y, detectionRangeX + 1, detectionRangeY + 1); ;
        this.pickupableChange = GameScenePartitioner.Instance.Add("SongMachine.Egg", (object)this.gameObject, this.detectionExtents, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(this.OnPickupablesChanged));
        RefreshReachableCells();
    }

    protected override void OnCleanUp() {
        GameScenePartitioner.Instance.Free(ref this.pickupableChange);
        MinionGroupProber.Get().ReleaseProber((object)this);
        base.OnCleanUp();
    }

    private void RefreshReachableCells() {
        ListPool<int, LogicDuplicantSensor>.PooledList pooledList = ListPool<int, LogicDuplicantSensor>.Allocate(this.reachableCells);
        this.reachableCells.Clear();
        int x;
        int y;
        Grid.CellToXY(this.NaturalBuildingCell(), out x, out y);
        int num = x - this.detectionRangeX / 2;
        for (int index1 = y; index1 < y + this.detectionRangeY + 1; ++index1) {
            for (int index2 = num; index2 < num + this.detectionRangeX + 1; ++index2) {
                int cell1 = Grid.XYToCell(index2, index1);
                CellOffset offset = new CellOffset(index2 - x, index1 - y);
                if (Grid.IsValidCell(cell1) && Grid.IsPhysicallyAccessible(x, y, index2, index1, true)) this.reachableCells.Add(cell1);
            }
        }
        pooledList.Recycle();
    }

    private void OnPickupablesChanged(object data) {
        Pickupable pickupable = data as Pickupable;
        bool flag;
        ClearList();
        wasOn = pickup.Count >= 1;
        UpdateVisualState(true);
        if (!(bool)(UnityEngine.Object)pickupable || !pickupable.KPrefabID.HasTag(GameTags.Egg)) return;
        flag = reachableCells.Contains(pickupable.cachedCell);
        if (flag && pickup.Contains(pickupable)) return;
        GameObject go = pickupable.objectLayerListItem?.gameObject;
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
        KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
        component.Play((HashedString)(wasOn ? "working_pre" : "working_pst"));
        component.Queue((HashedString)(wasOn ? "working_loop" : "off"), KAnim.PlayMode.Loop);
    }
}