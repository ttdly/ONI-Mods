using Klei.AI;
using KSerialization;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SongMachine : KMonoBehaviour{
    public int detectionRangeX = 10;
    public int detectionRangeY = 10;
    public bool wasOn;
    private List<Pickupable> pickup = new List<Pickupable>();
    private HandleVector<int>.Handle pickupableChange;
    private Extents detectionExtents;
    
    protected override void OnPrefabInit() {
        base.OnPrefabInit();
        this.simRenderLoadBalance = true; 
    }

    protected override void OnSpawn() {
        base.OnSpawn();
        Vector2I xy = Grid.CellToXY(this.NaturalBuildingCell());
        this.detectionExtents = new Extents(xy.x - (detectionRangeX / 2), xy.y, detectionRangeX + 1, detectionRangeY + 1); ;
        this.pickupableChange = GameScenePartitioner.Instance.Add("SongMachine.Egg", (object)this.gameObject, this.detectionExtents, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(this.OnPickupablesChanged));

    }

    protected override void OnCleanUp() {
        GameScenePartitioner.Instance.Free(ref this.pickupableChange);
        MinionGroupProber.Get().ReleaseProber((object)this);
        base.OnCleanUp();
    }


    private void OnPickupablesChanged(object data) {
        Pickupable pickupable = data as Pickupable;
        if (pickup.Contains(pickupable)) return;
        if (!(bool)(UnityEngine.Object)pickupable || !pickupable.KPrefabID.HasTag(GameTags.Egg)) return;
        GameObject go = pickupable.objectLayerListItem?.gameObject;
        if (!(bool)go) {
            clearList();
            wasOn = pickup.Count >= 1;
            UpdateVisualState();
            return;
        }
        go.AddOrGetDef<IncubationMonitor.Def>().baseIncubationRate = (float)(100.0 / (600.0 * (double) 0.1));
        go.GetComponent<Effects>().Add("EggSong", true);
        go.GetComponent<Effects>().Add("EggHug", true);
        pickup.Add(pickupable);
        wasOn = true;
        UpdateVisualState();
    }

    private void clearList() {
        pickup.RemoveAll(x => x.objectLayerListItem == null);
    }

    private void UpdateVisualState(bool force = false) {
        if (!(wasOn | force)) return;
        KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
        component.Play((HashedString)(wasOn ? "working_pre" : "working_pst"));
        component.Queue((HashedString)(wasOn ? "working_loop" : "off"), KAnim.PlayMode.Loop);
    }
}