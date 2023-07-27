using System;
using UnityEngine;

namespace LuckyChallenge {
    public class Gift : Workable {
        private Chore chore;
        private bool isMarkForSurvey;
        private Worker currWorker;
        private int workerCell;



        private static readonly EventSystem.IntraObjectHandler<Gift> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Gift>((Action<Gift, object>)((component, data) => component.OnRefreshUserMenu(data)));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            this.faceTargetWhenWorking = true;
            this.synchronizeAnims = false;
            this.alwaysShowProgressBar = false;
            this.alwaysShowProgressBar = false;
            this.overrideAnims = new KAnimFile[1]{
                Assets.GetAnim((HashedString) "anim_interacts_dumpable_kanim")
            };
            this.workAnims = new HashedString[1]{
                (HashedString) "working"
            };
            this.SetWorkTime(1f);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            this.Subscribe<Gift>((int)GameHashes.RefreshUserMenu, Gift.OnRefreshUserMenuDelegate);
            this.Subscribe<Gift>((int)GameHashes.StatusChange, Gift.OnRefreshUserMenuDelegate);
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
            this.progressBar.barColor = new Color(0.5f, 1f, 0.5f, 1f);
        }

        protected override void OnAbortWork(Worker worker) {
            base.OnAbortWork(worker);
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            //this.CreateTemplate(worker);
            God.RandomInAllElement(Grid.PosToCell(this.gameObject),30);
            if ((UnityEngine.Object)DetailsScreen.Instance != (UnityEngine.Object)null && DetailsScreen.Instance.CompareTargetWith(this.gameObject))
                DetailsScreen.Instance.Show(false);
            this.OnClickCancel();
            KBatchedAnimController kBatchedAnimController = this.gameObject.GetComponent<KBatchedAnimController>();
            kBatchedAnimController.Play("unwrap");
            kBatchedAnimController.destroyOnAnimComplete = true;

        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
        }


        // 自定义的方法
        public void OnRefreshUserMenu(object data) {
            Game.Instance.userMenu.AddButton(this.gameObject, this.isMarkForSurvey ? new KIconButtonMenu.ButtonInfo("action_follow_cam", "OFF", new System.Action(this.OnClickCancel), tooltipText: "OFF_DESC") : new KIconButtonMenu.ButtonInfo("action_follow_cam", "ON", new System.Action(this.OnClickSurvey), tooltipText: "ON_DESC"));
        }

        public void OnClickCancel() {
            if (this.chore == null && !this.isMarkForSurvey) {
                return;
            }
            this.isMarkForSurvey = false;
            this.chore.Cancel("Surveyable.CancelChore");
            this.chore = null;
        }

        public void OnClickSurvey() {
            Prioritizable.AddRef(this.gameObject);
            this.isMarkForSurvey = true;
            if (this.chore != null) return;
            this.chore = new WorkChore<Gift>(Db.Get().ChoreTypes.Build, this, only_when_operational: false);
        }

        public void CreateTemplate(Worker worker) {
            string dic = "hq";
            TemplateContainer template = TemplateCache.GetTemplate(dic);
            if (template == null || template.cells == null) return;
            this.workerCell = Grid.PosToCell(this.worker);
            Vector2f templateSize = template.info.size;
            int x = Mathf.FloorToInt((float)(-(double)templateSize.X / 2.0));
            int y = Mathf.FloorToInt((float)(-(double)templateSize.Y / 2.0));
            int cellTopLeft = Grid.OffsetCell(Grid.PosToCell(this.gameObject), x, y);
            cellTopLeft = Grid.CellLeft(cellTopLeft);
            Vector3 posCbc = Grid.CellToPosCBC(cellTopLeft, Grid.SceneLayer.Move);
            this.worker.transform.SetPosition(posCbc);
            int centerCell = Grid.OffsetCell(Grid.PosToCell(this.gameObject), 0, -y);
            TemplateLoader.Stamp(template, Grid.CellToXY(centerCell), (System.Action)(() => this.Complete(worker)));
            this.workerCell = centerCell;
        }

        public void Complete(Worker worker) {
            worker.transform.SetPosition(Grid.CellToPosCBC(this.workerCell, Grid.SceneLayer.Move));
            
        }
    }
}
