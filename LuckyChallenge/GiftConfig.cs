using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace LuckyChallenge {
  public class GiftConfig : IEntityConfig {
    public const string ID = "Gift";
    public const float mass = 200f;

    public GameObject CreatePrefab() {
      var name = "Gift";
      var desc = "GiftDesc";
      var dector = BUILDINGS.DECOR.BONUS.TIER1;
      var noisy = NOISE_POLLUTION.NOISY.TIER6;
      var anim = Assets.GetAnim((HashedString)"gift_kanim");
      var placedEntity = EntityTemplates.CreatePlacedEntity(ID, name, desc, mass, anim, "idle_1",
        Grid.SceneLayer.Building, 1, 1, dector, noisy, additionalTags: new List<Tag> { GameTags.ManufacturedMaterial });
      var component = placedEntity.GetComponent<PrimaryElement>();
      component.SetElement(SimHashes.Unobtanium);
      component.Temperature = 273.1f;
      placedEntity.AddOrGet<Gift>();
      var pickupable = placedEntity.AddOrGet<Pickupable>();
      pickupable.SetWorkTime(5f);
      pickupable.sortOrder = 0;
      placedEntity.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1] { ObjectLayer.Pickupables };
      placedEntity.AddOrGet<Movable>();
      return placedEntity;
    }

    public string[] GetDlcIds() {
      return DlcManager.AVAILABLE_ALL_VERSIONS;
    }

    void IEntityConfig.OnPrefabInit(GameObject inst) {
    }

    void IEntityConfig.OnSpawn(GameObject inst) {
    }
  }
}