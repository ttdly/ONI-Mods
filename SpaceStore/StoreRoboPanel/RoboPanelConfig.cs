using System.Collections.Generic;
using UnityEngine;

namespace SpaceStore.StoreRoboPanel {
    public class RoboPanelConfig:IEntityConfig {
        public const string ID = "RoboPanel";

        public static readonly Tag tag = TagManager.Create(ID);

        public const float MASS = 5f;

        public string[] GetDlcIds() {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public GameObject CreatePrefab() {
            return EntityTemplates.CreateLooseEntity(ID, MyString.ITEM.ROBO_PANEL.NAME, MyString.ITEM.ROBO_PANEL.DESC, 5f, unitMass: true, Assets.GetAnim("robo_panel_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, isPickupable: true, 0, SimHashes.Creature, new List<Tag> { GameTags.MiscPickupable });
        }

        public void OnPrefabInit(GameObject inst) {
        }

        public void OnSpawn(GameObject inst) {
        }
    }
}
