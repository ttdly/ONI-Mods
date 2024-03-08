using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceStore.MyGeyser {
    internal class GeoActivatorConfig :IEntityConfig{
        public const string ID = "Store_GeyserPack";

        public static readonly Tag TAG = TagManager.Create(ID);

        public string[] GetDlcIds() {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public GameObject CreatePrefab() {
            GameObject gameObject = EntityTemplates.CreatePlacedEntity(ID, MyString.ITEM.GEO_ACTIVATOR.NAME, MyString.ITEM.GEO_ACTIVATOR.DESC, 1f, Assets.GetAnim("geyser_pack_kanim"), "object", Grid.SceneLayer.Move, 2, 2, TUNING.DECOR.PENALTY.TIER2 ,element: SimHashes.Unobtanium);
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
