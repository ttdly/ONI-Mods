using TUNING;
using UnityEngine;

namespace StoreGoods {
  internal class GeoActivatorConfig : IEntityConfig {
    public const string ID = "Store_GeoSeed";

    public static readonly Tag TAG = TagManager.Create(ID);

    public string[] GetDlcIds() {
      return DlcManager.AVAILABLE_ALL_VERSIONS;
    }

    public GameObject CreatePrefab() {
      var gameObject = EntityTemplates.CreatePlacedEntity(ID, MyString.ITEM.GEO_ACTIVATOR.NAME,
        MyString.ITEM.GEO_ACTIVATOR.DESC, 1f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim("geo_seed_kanim"),
        initialAnim: "object", sceneLayer: Grid.SceneLayer.BuildingFront, width: 1, height: 1,
        element: SimHashes.Unobtanium, additionalTags: null, defaultTemperature: 255f);
      gameObject.AddOrGet<GeoActivator>();
      gameObject.AddOrGet<Pickupable>();
      gameObject.AddOrGet<Movable>();
      return gameObject;
    }

    public void OnPrefabInit(GameObject inst) {
    }

    public void OnSpawn(GameObject inst) {
    }
  }
}