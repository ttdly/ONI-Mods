using UnityEngine;

namespace LuckyChallenge {
  public class Gift : Workable {
    private static readonly EventSystem.IntraObjectHandler<Gift> OnRefreshUserMenuDelegate =
      new EventSystem.IntraObjectHandler<Gift>((component, data) => component.OnRefreshUserMenu(data));

    public int count = 10;
    public string anim = "idle_1";
    public GiftType type;
    private Chore chore;
    private WorkerBase currWorkerBase;
    private bool isMarkForSurvey;
    private int workerCell;

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
      faceTargetWhenWorking = true;
      synchronizeAnims = false;
      alwaysShowProgressBar = false;
      alwaysShowProgressBar = false;
      overrideAnims = new KAnimFile[1] {
        Assets.GetAnim((HashedString)"anim_interacts_dumpable_kanim")
      };
      workAnims = new HashedString[1] {
        (HashedString)"working"
      };
      SetWorkTime(1f);
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
      Subscribe((int)GameHashes.StatusChange, OnRefreshUserMenuDelegate);
      gameObject.GetComponent<KBatchedAnimController>().Play(anim);
    }

    protected override void OnStartWork(WorkerBase worker) {
      base.OnStartWork(worker);
      progressBar.barColor = new Color(0.5f, 1f, 0.5f, 1f);
    }

    protected override void OnAbortWork(WorkerBase worker) {
      base.OnAbortWork(worker);
    }

    protected override void OnCompleteWork(WorkerBase worker) {
      base.OnCompleteWork(worker);
      God.OpenTheGift(type, Grid.PosToCell(gameObject), count, this.worker);
      if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(gameObject))
        DetailsScreen.Instance.Show(false);
      OnClickCancel();
      var kBatchedAnimController = gameObject.GetComponent<KBatchedAnimController>();
      kBatchedAnimController.Play("unwrap");
      kBatchedAnimController.destroyOnAnimComplete = true;
    }

    protected override void OnCleanUp() {
      base.OnCleanUp();
    }


    // 自定义的方法
    public void OnRefreshUserMenu(object data) {
      Game.Instance.userMenu.AddButton(gameObject,
        isMarkForSurvey
          ? new KIconButtonMenu.ButtonInfo("action_follow_cam", "OFF", OnClickCancel, tooltipText: "OFF_DESC")
          : new KIconButtonMenu.ButtonInfo("action_follow_cam", "ON", OnClickSurvey, tooltipText: "ON_DESC"));
    }

    public void OnClickCancel() {
      if (chore == null && !isMarkForSurvey) return;
      isMarkForSurvey = false;
      chore.Cancel("Surveyable.CancelChore");
      chore = null;
    }

    public void OnClickSurvey() {
      Prioritizable.AddRef(gameObject);
      isMarkForSurvey = true;
      if (chore != null) return;
      chore = new WorkChore<Gift>(Db.Get().ChoreTypes.Build, this, only_when_operational: false);
    }

    public void CreateTemplate(WorkerBase worker) {
      var dic = "hq";
      var template = TemplateCache.GetTemplate(dic);
      if (template == null || template.cells == null) return;
      workerCell = Grid.PosToCell(this.worker);
      var templateSize = template.info.size;
      var x = Mathf.FloorToInt((float)(-(double)templateSize.X / 2.0));
      var y = Mathf.FloorToInt((float)(-(double)templateSize.Y / 2.0));
      var cellTopLeft = Grid.OffsetCell(Grid.PosToCell(gameObject), x, y);
      cellTopLeft = Grid.CellLeft(cellTopLeft);
      var posCbc = Grid.CellToPosCBC(cellTopLeft, Grid.SceneLayer.Move);
      this.worker.transform.SetPosition(posCbc);
      var centerCell = Grid.OffsetCell(Grid.PosToCell(gameObject), 0, -y);
      TemplateLoader.Stamp(template, Grid.CellToXY(centerCell), () => Complete(worker));
      workerCell = centerCell;
    }

    public void Complete(WorkerBase worker) {
      worker.transform.SetPosition(Grid.CellToPosCBC(workerCell, Grid.SceneLayer.Move));
    }
  }
}