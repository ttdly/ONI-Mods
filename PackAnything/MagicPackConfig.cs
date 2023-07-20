﻿using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace PackAnything {
    public class MagicPackConfig : IEntityConfig {
        public const string ID = "MagicPack";
        public const float mass = 200f;
        public GameObject CreatePrefab() {
            string name = (string)PackAnythingString.MISC.MAGIC_PACK.NAME;
            string desc = (string)PackAnythingString.MISC.MAGIC_PACK.DESC;
            EffectorValues dector = TUNING.BUILDINGS.DECOR.BONUS.TIER1;
            EffectorValues noisy = NOISE_POLLUTION.NOISY.TIER6;
            KAnimFile anim = Assets.GetAnim((HashedString)"magic_pack_kanim");
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity(ID,name,desc,mass,anim, "portalbirth",Grid.SceneLayer.Creatures, 1, 1, dector,noisy,additionalTags:new List<Tag>(){GameTags.Other});
            PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
            component.SetElement(SimHashes.Unobtanium);
            component.Temperature = 273.1f;
            placedEntity.AddOrGet<Prioritizable>();
            Pickupable pickupable = placedEntity.AddOrGet<Pickupable>();
            pickupable.SetWorkTime(5f);
            pickupable.sortOrder = 0;
            placedEntity.AddOrGet<Movable>();
            placedEntity.AddOrGet<UnPack>();
            placedEntity.AddOrGet<MagicPack>();
            placedEntity.AddOrGet<Storage>();
            return placedEntity;
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst) {
        }

        public void OnSpawn(GameObject inst) {
        }
    }
}