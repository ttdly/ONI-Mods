using PeterHan.PLib.Core;
using STRINGS;
using UnityEngine;

namespace PackAnything {
    public class DelayMove : KMonoBehaviour, ISim1000ms {
        private ProgressBar m_Progress;
        [SerializeField]
        public float orderProgress;
#if DEBUG
        public float delay = 0.5f;
#else
        public float delay = 0.01f;
#endif
        [SerializeField]
        public int cell;

        public virtual float GetProgress() => orderProgress;

        public void Sim1000ms(float dt) {
            orderProgress += delay;
            PUtil.LogDebug(orderProgress);
            ShowProgressBar();
            Complete();
        }

        private void Complete() { 
            if (orderProgress >= 1) {
                if (PackAnythingStaticVars.targetSurveyable != null) {
                    GameObject originObject = PackAnythingStaticVars.targetSurveyable.gameObject;
                    Vector3 posCbc = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
                    KSelectable selectable = originObject.GetComponent<KSelectable>();
                    OccupyArea occupyArea = originObject.GetComponent<OccupyArea>();
                    Building building = originObject.GetComponent<Building>();
                    selectable?.transform.SetPosition(posCbc);
                    occupyArea?.UpdateOccupiedArea();
                    building?.UpdatePosition();
                }
                CancelAll();
            }
        }

        private void ShowProgressBar() {
            if (m_Progress == null) {
                m_Progress = ProgressBar.CreateProgressBar(gameObject, GetProgress);
                m_Progress.enabled = true;
                m_Progress.SetVisibility(true);
                m_Progress.barColor = PackAnythingStaticVars.PrimaryColor;
            }
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe(493375141, OnRefreshUserMenu);
        }

        private void CancelAll() {
            PackAnythingStaticVars.targetSurveyable = null;
            if(m_Progress != null) {
                m_Progress.gameObject.DeleteObject();
                m_Progress = null;
            }
            KBatchedAnimController kBatchedAnimController = gameObject.GetComponent<KBatchedAnimController>();
            kBatchedAnimController.Play("destroy");
            kBatchedAnimController.destroyOnAnimComplete = true;
        }



        private void OnRefreshUserMenu(object data) {
            Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_control", UI.USERMENUACTIONS.PICKUPABLEMOVE.NAME_OFF, CancelAll, Action.NumActions, null, null, null, UI.USERMENUACTIONS.PICKUPABLEMOVE.TOOLTIP_OFF));
        }

    }
}
