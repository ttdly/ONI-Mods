using UnityEngine;

namespace PackAnything {
    public class BeaconPlacerConfig : IEntityConfig {
        public const string ID = "BeaconPlacer";
        public const float mass = 200f;
        public GameObject CreatePrefab() {
            string name = PackAnythingString.MISC.DISPLACEMENT_BEACON.NAME;
            string desc = PackAnythingString.MISC.DISPLACEMENT_BEACON.DESC;
            KAnimFile anim = Assets.GetAnim((HashedString)"displacement_beacon_kanim");
            GameObject placeEntity = EntityTemplates.CreateLooseEntity(ID, name, desc, mass, false, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE ,1, 2);
            placeEntity.AddOrGet<DelayMove>();
            return placeEntity;
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst) {
        }

        public void OnSpawn(GameObject inst) {
        }
    }
}
