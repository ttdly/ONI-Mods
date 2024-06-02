using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Overrider : KMonoBehaviour {
  public int detectionRangeX = 10;
  public int detectionRangeY = 10;
  public bool wasOn;
  private readonly List<GameObject> buildings = new List<GameObject>();
  private HandleVector<int>.Handle buildingsChange;
  private HandleVector<int>.Handle buildingsChangedEntry;
  private readonly List<BuildingComplete> completeBuildings = new List<BuildingComplete>();
  private bool detected = false;
  private Extents detectionExtents;

  private List<int> reachableCells = new List<int>(100);

  [MyCmpGet] private Rotatable rotatable;

  [MyCmpGet] private KSelectable selectable;

  private readonly List<Tag> targetTags = new List<Tag> {
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

  protected override void OnPrefabInit() {
    base.OnPrefabInit();
    simRenderLoadBalance = true;
  }

  protected override void OnSpawn() {
    base.OnSpawn();

    var xy = Grid.CellToXY(this.NaturalBuildingCell());
    detectionExtents = initExtents(xy, rotatable.Orientation);
    buildingsChangedEntry = GameScenePartitioner.Instance.Add("Overrider.HasCompleteBuildings", gameObject,
      detectionExtents, GameScenePartitioner.Instance.objectLayers[1], OnNearbyBuildingLayerChanged);
    initComplete();
    UpdateVisualState(true);
    buildingsChange = GameScenePartitioner.Instance.Add("Overrider.CompleteBuildings", gameObject, detectionExtents,
      GameScenePartitioner.Instance.objectLayers[2], OnNearbyBuildingLayerChanged);
  }

  protected override void OnCleanUp() {
    GameScenePartitioner.Instance.Free(ref buildingsChangedEntry);
    GameScenePartitioner.Instance.Free(ref buildingsChange);
    MinionGroupProber.Get().ReleaseProber(this);
    restoreBuildings();
    base.OnCleanUp();
  }

  private void initComplete() {
    var gathered_entries = ListPool<ScenePartitionerEntry, Overrider>.Allocate();
    GameScenePartitioner.Instance.GatherEntries(detectionExtents.x, detectionExtents.y, detectionExtents.width,
      detectionExtents.height, GameScenePartitioner.Instance.objectLayers[19], gathered_entries);
    for (var i = 0; i < gathered_entries.Count; i++) {
      var buildingComplete = gathered_entries[i].obj as BuildingComplete;
      if (!(bool)buildingComplete) continue;
      if (completeBuildings.Contains(buildingComplete)) continue;
      if (buildingComplete.prefabid.HasAnyTags(targetTags)) {
        var complexFabricator = buildingComplete.GetComponent<ComplexFabricator>();
        if (!(bool)complexFabricator) continue;
        complexFabricator.duplicantOperated = false;
        completeBuildings.Add(buildingComplete);
      }
    }

    wasOn = completeBuildings.Count > 0;
  }

  private Extents initExtents(Vector2I xy, Orientation orientation) {
    switch (orientation) {
      case Orientation.R90:
        return new Extents(xy.x, xy.y - detectionRangeX / 2, detectionRangeY + 1, detectionRangeX + 1);
      case Orientation.R180:
        return new Extents(xy.x - detectionRangeX / 2, xy.y - detectionRangeY, detectionRangeX + 1,
          detectionRangeY + 1);
      case Orientation.R270:
        return new Extents(xy.x - detectionRangeY, xy.y - detectionRangeX / 2, detectionRangeY + 1,
          detectionRangeX + 1);
      default: return new Extents(xy.x - detectionRangeX / 2, xy.y, detectionRangeX + 1, detectionRangeY + 1);
    }
  }

  private void OnNearbyBuildingLayerChanged(object data) {
    var go = data as GameObject;
    if (!(bool)go) {
      if (buildings.Count == 0) return;
      buildings.RemoveAll(o => !o.activeSelf);
      wasOn = !(buildings.Count == 0);
      UpdateVisualState(true);
      return;
    }

    ;
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
    var fabricator = go.GetComponent<ComplexFabricator>();
    if (!(bool)fabricator) return;
    fabricator.duplicantOperated = false;
    buildings.Add(go);
  }

  private void restoreBuildings() {
    if (buildings.Count > 0)
      for (var i = 0; i < buildings.Count; i++)
        buildings[i].GetComponent<ComplexFabricator>().duplicantOperated = true;
    if (completeBuildings.Count > 0)
      for (var i = 0; i < completeBuildings.Count; i++)
        completeBuildings[i].GetComponent<ComplexFabricator>().duplicantOperated = true;
  }

  private bool IsTargetBuilding(GameObject go) {
    return go.GetComponent<KPrefabID>().HasAnyTags(targetTags);
  }

  private void UpdateVisualState(bool force = false) {
    if (!(wasOn | force)) return;
    var component = GetComponent<KBatchedAnimController>();
    component.Play((HashedString)(wasOn ? "on_pre" : "on_pst"));
    component.Queue((HashedString)(wasOn ? "on" : "off"), KAnim.PlayMode.Loop);
  }
}