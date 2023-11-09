using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace PackAnything {
    public class BeaconConfig : IEntityConfig {
        public const string ID = "Beacon";
        public const float mass = 200f;
        public GameObject CreatePrefab() {
            string name = PackAnythingString.MISC.DISPLACEMENT_BEACON.NAME;
            string desc = PackAnythingString.MISC.DISPLACEMENT_BEACON.DESC;
            EffectorValues dector = BUILDINGS.DECOR.BONUS.TIER1;
            EffectorValues noisy = NOISE_POLLUTION.NOISY.TIER6;
            KAnimFile anim = Assets.GetAnim((HashedString)"displacement_beacon_kanim");
            GameObject placeEntity = EntityTemplates.CreatePlacedEntity(ID, name, desc, mass, anim, "object", Grid.SceneLayer.Front, 1, 2, dector, noisy, additionalTags: new List<Tag>() { GameTags.IndustrialProduct });
            Pickupable pickupable = placeEntity.AddOrGet<Pickupable>();
            pickupable.SetWorkTime(5f);
            pickupable.sortOrder = 0;
            placeEntity.AddOrGet<Movable>();
            placeEntity.AddOrGet<Beacon>();
            placeEntity.AddOrGet<UserNameable>();
            return placeEntity;
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst) {
        }

        public void OnSpawn(GameObject inst) {
        }
    }
}
