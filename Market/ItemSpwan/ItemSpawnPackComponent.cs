using KSerialization;
using PeterHan.PLib.Core;
using UnityEngine;

namespace Market.ItemSpwan {
    public class ItemSpawnPackComponent : KMonoBehaviour {
        [Serialize]
        public Tag spawnTag;
        public GameObject spawnPrefab;
        [MyCmpReq]
        private readonly KBatchedAnimController kBatchedAnimController;

        private static readonly EventSystem.IntraObjectHandler<ItemSpawnPackComponent> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ItemSpawnPackComponent>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            kBatchedAnimController.TintColour = Color.yellow;
        }

        private void OnRefreshUserMenu(object _) {
            Game.Instance.userMenu.AddButton(
                gameObject,
                new KIconButtonMenu.ButtonInfo(
                    "action_follow_cam",
                    MyStrings.OTHERS.OPEN_BUTTON,
                    new System.Action(SpawnObject))
                );
        }

        public void SpawnObject() {
            GameObject go = null;
            spawnPrefab = Assets.GetPrefab(spawnTag);
            if (spawnPrefab == null) {
                gameObject.DeleteObject();
            }
            go = GameUtil.KInstantiate(spawnPrefab, Grid.SceneLayer.Building);
            if (go == null) return;
            Vector3 posCbc = gameObject.transform.position;
            if (go.GetComponent<Geyser>() != null) {
                posCbc = Grid.CellToPosCBC(Grid.PosToCell(gameObject), Grid.SceneLayer.Building);
                float num = -0.15f;
                posCbc.z += num;
            }
            go.transform.SetPosition(posCbc);
            go.SetActive(true);
            
#if DEBUG
            PUtil.LogDebug($"已经生成：{go.GetProperName()}");
#endif
            gameObject.DeleteObject();
        }
    }
}
