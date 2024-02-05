using STRINGS;
using UnityEngine;

namespace Market.ItemSpwan {
    public class ItemSpawnPackConfig : IEntityConfig{
        public static readonly string ID = "ItemSpwanPack";

        public string[] GetDlcIds() {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public GameObject CreatePrefab() {
            return EntityTemplates.CreatePlacedEntity(ID, ITEMS.CARGO_CAPSULE.NAME, "", 1f, Assets.GetAnim("hardplastic_kanim"), "idle3", Grid.SceneLayer.Front, 1, 1, TUNING.DECOR.BONUS.TIER4);
        }

        public void OnPrefabInit(GameObject go) {
            go.AddOrGet<ItemSpawnPackComponent>();
            go.AddOrGet<UserNameable>();
            go.AddOrGet<Pickupable>();
            go.AddOrGet<Clearable>().isClearable = false;
            go.AddOrGet<Movable>();
            Light2D light2D = go.AddOrGet<Light2D>();
            light2D.Color = Color.green;
            light2D.Range = 1f;
            light2D.Offset = new Vector2(0, 0);
            light2D.overlayColour = Color.green;
            light2D.shape = LightShape.Circle;
            light2D.drawOverlay = true;
            light2D.Lux = 200;
        }

        public void OnSpawn(GameObject go) {
        }
    }
}
