using System;

namespace MoveGeysers {
    public class OilWellSubscribe : KMonoBehaviour {
        public static int OilWellCapBuild = Hash.SDBMLower("OilWellCapBuild");
        public static int OilWellCapDestory = Hash.SDBMLower("OilWellCapDestory");
        public bool hasCap = false;


        protected override void OnSpawn() {
            base.OnSpawn();
            Destroy(gameObject.AddOrGet<Demolishable>());
            Game.Instance.Subscribe(OilWellCapBuild, new Action<object>(this.OnOilWellCapBuild));
            Game.Instance.Subscribe(OilWellCapDestory, new Action<object>(this.OnOilWellCapDestory));
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();

        }

        public void OnOilWellCapBuild(object data) {
            if (gameObject.GetComponent<BuildingAttachPoint>().points[0].attachedBuilding != null) {
                hasCap = true;
                gameObject.RemoveTag(GameTags.Pickupable);
                gameObject.SetActive(false);
            }
        }

        public void OnOilWellCapDestory(object data) {
            if (hasCap) {
                gameObject.SetActive(true);
                gameObject.AddTag(GameTags.Pickupable);
                hasCap = false;
            }
        }
    }
}
