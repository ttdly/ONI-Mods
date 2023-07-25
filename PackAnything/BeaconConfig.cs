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
            EffectorValues dector = TUNING.BUILDINGS.DECOR.BONUS.TIER1;
            EffectorValues noisy = NOISE_POLLUTION.NOISY.TIER6;
            KAnimFile anim = Assets.GetAnim((HashedString)"displacement_beacon_kanim");
            GameObject placeEntity = EntityTemplates.CreatePlacedEntity(ID, name, desc, mass, anim, "object", Grid.SceneLayer.Front, 1, 2, dector, noisy, additionalTags: new List<Tag>() { GameTags.IndustrialProduct });
            PrimaryElement component = placeEntity.GetComponent<PrimaryElement>();
            component.SetElement(SimHashes.Unobtanium);
            component.Temperature = 273.1f;
            Pickupable pickupable = placeEntity.AddOrGet<Pickupable>();
            pickupable.SetWorkTime(5f);
            pickupable.sortOrder = 0;
            placeEntity.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1] { ObjectLayer.Pickupables };
            placeEntity.AddOrGet<Movable>();
            placeEntity.AddOrGet<Beacon>();
            placeEntity.AddOrGet<UserNameable>();
            placeEntity.AddOrGet<Prioritizable>();
            return placeEntity;
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst) {
        }

        public void OnSpawn(GameObject inst) {
        }
    }
}
