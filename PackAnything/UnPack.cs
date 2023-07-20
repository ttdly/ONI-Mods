using KSerialization;
using PeterHan.PLib.Core;
using System;
using UnityEngine;

namespace PackAnything {
    [AddComponentMenu("KMonoBehaviour/Workable/UnPack")]
    public class UnPack : Workable {
        private Chore chore;
        [Serialize]
        private bool isMarkFroUnPack;
        private Guid statusItemGuid;
        public bool MarkFroPack => this.isMarkFroUnPack;

        private static readonly EventSystem.IntraObjectHandler<UnPack> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<UnPack>((Action<UnPack, object>)((component, data) => component.OnRefreshUserMenu(data)));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            //this.requiredSkillPerk = Db.Get().SkillPerks.IncreaseCarryAmountMedium.Id;
            this.alwaysShowProgressBar = false;
            this.faceTargetWhenWorking = false;
            this.multitoolContext = (HashedString)"build";
            this.multitoolHitEffectTag = (Tag)EffectConfigs.BuildSplashId;
            this.SetWorkTime(1.5f);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            this.Subscribe<UnPack>((int)GameHashes.RefreshUserMenu, UnPack.OnRefreshUserMenuDelegate);
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            this.UnPackIt(worker);
            this.OnClickCancel();
        }


        // 自定义的方法
        public void OnRefreshUserMenu(object data) {
            if (gameObject.HasTag(GameTags.Stored)) return;
            Game.Instance.userMenu.AddButton(this.gameObject, this.isMarkFroUnPack ? new KIconButtonMenu.ButtonInfo("action_deconstruct", (string)PackAnythingString.UI.UNPACK_IT.NAME_OFF, new System.Action(this.OnClickCancel), tooltipText: (string)PackAnythingString.UI.UNPACK_IT.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_deconstruct", (string)PackAnythingString.UI.UNPACK_IT.NAME, new System.Action(this.OnClickPack), tooltipText: (string)PackAnythingString.UI.UNPACK_IT.TOOLTIP));
        }

        public void OnClickCancel() {
            if (this.chore == null && !this.isMarkFroUnPack) {
                return;
            }
            this.isMarkFroUnPack = false;
            this.chore.Cancel("UnPack.CancelChore");
            this.chore = null;
        }

        public void OnClickPack() {
            KSelectable kSelectable = this.GetComponent<KSelectable>();
            this.isMarkFroUnPack = true;
            if (chore == null) {
                chore = new WorkChore<UnPack>(
                    chore_type: Db.Get().ChoreTypes.EmptyStorage,
                    target: this,
                    chore_provider: null,
                    run_until_complete: true,
                    on_complete: null,
                    on_begin: null,
                    on_end: null,
                    allow_in_red_alert: true,
                    schedule_block: null,
                    ignore_schedule_block: false,
                    only_when_operational: false,
                    override_anims: null,
                    is_preemptable: false,
                    allow_in_context_menu: true,
                    allow_prioritization: true,
                    priority_class: PriorityScreen.PriorityClass.basic,
                    priority_class_value: 5,
                    ignore_building_assignment: false,
                    add_to_daily_report: true
                    );
            }
        }

        public void UnPackIt(Worker worker) {
            if (worker.HasTag(GameTags.Minion)) {
                PUtil.LogDebug("Minion");
            }
            KBatchedAnimController animController = this.gameObject.GetComponent<KBatchedAnimController>();
            MagicPack magicPack = this.gameObject.GetComponent<MagicPack>();
            GameObject storedObject = magicPack.storedObject;
            if (storedObject != null) {
                int cell = Grid.PosToCell(this.gameObject);
                if (magicPack.isGeyser) {
                    this.CreateNeutronium(cell);
                    cell = Grid.CellAbove(cell);
                }
                GameObject go = GameUtil.KInstantiate(storedObject, Grid.CellToPos(cell), Grid.SceneLayer.BuildingBack);
                go.SetActive(true);
                Util.KDestroyGameObject(storedObject);
            }
            Util.KDestroyGameObject(this.gameObject);
            PUtil.LogDebug("Work Complete");
        }

        public void CreateNeutronium(int cell) {
            int[] cells = new[]{
                Grid.CellLeft(Grid.CellLeft(cell)),
                Grid.CellLeft(cell),
                cell,
                Grid.CellRight(cell),
                Grid.CellRight(Grid.CellRight(cell))
            };
            foreach (int x in cells) {
                if (Grid.Element.Length < x || Grid.Element[x] == null) {
                    PUtil.LogError("Out of index.");
                    new IndexOutOfRangeException();
                    return;
                }
                Element e = Grid.Element[x];
                if (!Grid.IsValidCell(x)) continue;
                SimMessages.ReplaceElement(
                    gameCell: x,
                    new_element: SimHashes.Unobtanium,
                    ev: CellEventLogger.Instance.DebugTool,
                    mass: 100f,
                    temperature: 293f,
                    diseaseIdx: byte.MaxValue,
                    diseaseCount: 0,
                    callbackIdx: -1
                    );
            }
        }
    }
}
