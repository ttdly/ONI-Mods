using KSerialization;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Overrider : KMonoBehaviour{
    [MyCmpGet]
    private KSelectable selectable;
    [MyCmpGet]
    private Rotatable rotatable;
    public int detectionRangeX = 10;
    public int detectionRangeY = 10;
    public bool wasOn;
    private bool detected = false;
    private List<GameObject> buildings = new List<GameObject>();
    private List<BuildingComplete> completeBuildings = new List<BuildingComplete>();
    private List<Tag> targetTags = new List<Tag>() {
        new Tag("MetalRefinery"),
        new Tag("RockCrusher"),
        new Tag("SuitFabricator"),
        new Tag("MicrobeMusher"),
        new Tag("SupermaterialRefinery"),
        new Tag("SludgePress"),
        new Tag("DiamondPress"),
        new Tag("EggCracker"),
        new Tag("GlassForge"),
        new Tag("GourmetCookingStation"),
        new Tag("ManualHighEnergyParticleSpawner"),
        new Tag("MissileFabricator"),
        new Tag("UraniumCentrifuge"),
        new Tag("ClothingAlterationStation"),
        new Tag("ClothingFabricator"),
        new Tag("CookingStation"),
        new Tag("CraftingTable"),
        new Tag("FossilDig"),
        new Tag("OrbitalResearchCenter")
    };
    private HandleVector<int>.Handle buildingsChangedEntry;
    private HandleVector<int>.Handle buildingsChange;
    private Extents detectionExtents;
    
    private List<int> reachableCells = new List<int>(100); 
    protected override void OnPrefabInit() {
        base.OnPrefabInit();
        this.simRenderLoadBalance = true; 
    }

    protected override void OnSpawn() {
        base.OnSpawn();
        
        Vector2I xy = Grid.CellToXY(this.NaturalBuildingCell());
        this.detectionExtents = this.initExtents(xy, this.rotatable.Orientation);
        this.buildingsChangedEntry = GameScenePartitioner.Instance.Add("Overrider.HasCompleteBuildings", (object)this.gameObject, this.detectionExtents, GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnNearbyBuildingLayerChanged));
        initComplete();
        this.UpdateVisualState(true);
        this.buildingsChange = GameScenePartitioner.Instance.Add("Overrider.CompleteBuildings", (object)this.gameObject, this.detectionExtents, GameScenePartitioner.Instance.objectLayers[2], new Action<object>(this.OnNearbyBuildingLayerChanged));
    }

    protected override void OnCleanUp() {
        GameScenePartitioner.Instance.Free(ref this.buildingsChangedEntry);
        GameScenePartitioner.Instance.Free(ref this.buildingsChange);
        MinionGroupProber.Get().ReleaseProber((object)this);
        restoreBuildings();
        base.OnCleanUp();
    }

    private void initComplete() {
        ListPool<ScenePartitionerEntry, Overrider>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, Overrider>.Allocate();
        GameScenePartitioner.Instance.GatherEntries(this.detectionExtents.x, this.detectionExtents.y, this.detectionExtents.width, this.detectionExtents.height, GameScenePartitioner.Instance.objectLayers[19], (List<ScenePartitionerEntry>)gathered_entries);
        for (int i = 0; i < gathered_entries.Count ;i++) {
            BuildingComplete buildingComplete = gathered_entries[i].obj as BuildingComplete;
            if (!(bool)buildingComplete) continue;
            if (completeBuildings.Contains(buildingComplete)) continue;
            if (buildingComplete.prefabid.HasAnyTags(targetTags)) {
                ComplexFabricator complexFabricator = buildingComplete.GetComponent<ComplexFabricator>();
                if (!(bool)complexFabricator) continue;
                complexFabricator.duplicantOperated = false;
                completeBuildings.Add(buildingComplete);
            }
        }
        wasOn = completeBuildings.Count > 0;
    }

    private Extents initExtents(Vector2I xy, Orientation orientation) {
        switch (orientation) {
            case Orientation.R90: return new Extents(xy.x, xy.y - detectionRangeX / 2, detectionRangeY + 1, detectionRangeX + 1 );
            case Orientation.R180: return new Extents(xy.x - detectionRangeX / 2, xy.y - detectionRangeY, detectionRangeX + 1, detectionRangeY + 1);
            case Orientation.R270: return new Extents(xy.x - detectionRangeY, xy.y - detectionRangeX / 2, detectionRangeY + 1, detectionRangeX + 1);
            default: return new Extents(xy.x - (detectionRangeX / 2), xy.y, detectionRangeX + 1, detectionRangeY + 1);
        }
    }

    private void OnNearbyBuildingLayerChanged(object data) {
        GameObject go = data as GameObject;
        if (!(bool)go) {
            if (buildings.Count == 0) return;
            buildings.RemoveAll(o => !o.activeSelf);
            wasOn = !(buildings.Count == 0);
            UpdateVisualState(true);
            return;
        };
        if (go.activeSelf) {
            if (buildings.Contains(go)) return;
            OverrideThisTarget(go);
            if (buildings.Count > 1) return;
            wasOn = true;
            UpdateVisualState();
        } else {
            buildings.Remove(go);
            buildings.RemoveAll(o => !o.activeSelf);
            if (buildings.Count == 0) return;
            wasOn = !(buildings.Count == 0);
            UpdateVisualState(true);
        }

    }

    private void OverrideThisTarget(GameObject go) {
        if (!IsTargetBuilding(go)) return;
        ComplexFabricator fabricator = go.GetComponent<ComplexFabricator>();
        if (!(bool)fabricator) return;
        fabricator.duplicantOperated = false;
        buildings.Add(go);
    }

    private void restoreBuildings() {
       if (buildings.Count > 0) {
            for (int i = 0; i < buildings.Count; i++) {
                buildings[i].GetComponent<ComplexFabricator>().duplicantOperated = true;
            }
        }
       if (completeBuildings.Count > 0) {
            for (int i = 0;i < completeBuildings.Count; i++) {
                completeBuildings[i].GetComponent<ComplexFabricator>().duplicantOperated = true;
            }
        }
    }

    private bool IsTargetBuilding(GameObject go) { 
        return go.GetComponent<KPrefabID>().HasAnyTags(targetTags);
    }

    private void UpdateVisualState(bool force = false) {
        if (!(wasOn | force)) return;
        KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
        component.Play((HashedString)(wasOn ? "on_pre" : "on_pst"));
        component.Queue((HashedString)(wasOn ? "on" : "off"), KAnim.PlayMode.Loop);
    }
}