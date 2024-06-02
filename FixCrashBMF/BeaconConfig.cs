using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace FixCrashBMF {
  public class BeaconConfig : IEntityConfig {
    public const string ID = "Beacon";
    public const float mass = 200f;

    public GameObject CreatePrefab() {
      var name = "NULL";
      var desc = "NULL";
      var dector = BUILDINGS.DECOR.BONUS.TIER1;
      var noisy = NOISE_POLLUTION.NOISY.TIER6;
      var anim = Assets.GetAnim((HashedString)"displacement_beacon_kanim");
      var placeEntity = EntityTemplates.CreatePlacedEntity(ID, name, desc, mass, anim, "object", Grid.SceneLayer.Front,
        1, 2, dector, noisy, additionalTags: new List<Tag> { GameTags.IndustrialProduct });
      var pickupable = placeEntity.AddOrGet<Pickupable>();
      pickupable.SetWorkTime(5f);
      pickupable.sortOrder = 0;
      placeEntity.AddOrGet<Movable>();
      placeEntity.AddOrGet<Beacon>();
      placeEntity.AddOrGet<UserNameable>();
      return placeEntity;
    }

    public string[] GetDlcIds() {
      return DlcManager.AVAILABLE_ALL_VERSIONS;
    }

    public void OnPrefabInit(GameObject inst) {
    }

    public void OnSpawn(GameObject inst) {
    }
  }
}