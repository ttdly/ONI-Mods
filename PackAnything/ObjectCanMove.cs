using KSerialization;
using UnityEngine;

namespace PackAnything {
    public class ObjectCanMove: KMonoBehaviour {
        [Serialize]
        public bool isSurveyed = false;
        [Serialize]
        public ObjectType objectType;

        public ObjectType ObjectType {
            get { return objectType; }
        }
        private static readonly EventSystem.IntraObjectHandler<ObjectCanMove> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ObjectCanMove>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            Subscribe((int)GameHashes.StatusChange, OnRefreshUserMenuDelegate);
            if (isSurveyed) {
                PackAnythingStaticVars.SurveableCmps.Add(this);
                return;
            }

        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            PackAnythingStaticVars.SurveableCmps.Remove(this);
        }

        // 自定义的方法
        public void OnRefreshUserMenu(object _) {
            if (isSurveyed) return;
            if (gameObject.HasTag("OilWell") && gameObject.GetComponent<BuildingAttachPoint>()?.points[0].attachedBuilding != null) return;
            Game.Instance.userMenu.AddButton(
                gameObject,
                new KIconButtonMenu.ButtonInfo(
                    "action_follow_cam", 
                    PackAnythingString.UI.SURVEY.NAME, 
                    new System.Action(AddThisToList), 
                    tooltipText: PackAnythingString.UI.SURVEY.TOOLTIP)
                );
        }

        public void AddThisToList() {
            isSurveyed = true;
            PackAnythingStaticVars.SurveableCmps.Add(this);
        }

        public void RemoveThisFromList() {
            isSurveyed = false;
            PackAnythingStaticVars.SurveableCmps.Remove(this);
        }

        public void RefershObjectType() {
            objectType = GetObjectType();
        }

        private ObjectType GetObjectType() {
            if (objectType != ObjectType.None) return objectType;
            switch (gameObject.GetComponent<KPrefabID>().PrefabTag.ToString()) {
                case "WarpReceiver":
                    return ObjectType.WarpReceiver;
                case "GravitasCreatureManipulator":
                    return ObjectType.GravitasCreatureManipulator;
                case "WarpPortal":
                    return ObjectType.WarpPortal;
            }
            if (gameObject.GetComponent<SetLocker>() != null) return ObjectType.HaveSetLocker;
            if (gameObject.GetComponent<LoreBearer>() != null) return ObjectType.HaveLoreBearer;
            if (gameObject.GetComponent<Activatable>() != null) return ObjectType.Activatable;
            return ObjectType.None;
        }

        public GameObject CloneOriginObject() {
            return gameObject;
        }

        public void DestoryOriginObject() {
            gameObject.SetActive(false);
            gameObject.DeleteObject();
        }
    }
}
