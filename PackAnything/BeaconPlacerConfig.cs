using UnityEngine;

namespace PackAnything {
    public class BeaconPlacerConfig : CommonPlacerConfig, IEntityConfig {
        public const string ID = "BeaconPlacer";
        public const float mass = 200f;
        public GameObject CreatePrefab() {
            string name = PackAnythingString.MISC.DISPLACEMENT_BEACON.NAME;
            Material material = new Material(Assets.instance.movePickupToPlacerAssets.material.shader) {
                color = Color.white,
                mainTexture = PackAnythingStaticVars.ToolIcon.texture
            };
            GameObject prefab = CreatePrefab(ID, name, material);
            prefab.AddOrGet<DelayMove>();
            prefab.AddOrGet<UserNameable>();
            return prefab;
        }

        public void OnPrefabInit(GameObject inst) {
        }

        public void OnSpawn(GameObject inst) {
        }
    }
}
