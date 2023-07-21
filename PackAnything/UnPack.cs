using Klei.AI;
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
            this.alwaysShowProgressBar = false;
            this.faceTargetWhenWorking = false;
            this.multitoolContext = (HashedString)"build";
            this.multitoolHitEffectTag = (Tag)EffectConfigs.BuildSplashId;
            //this.overrideAnims = new KAnimFile[1] { Assets.GetAnim((HashedString)"anim_use_machine_kanim") };
            this.faceTargetWhenWorking = true;
            this.SetWorkTime(1f);
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
            this.gameObject.name = this.gameObject.GetComponent<MagicPack>().storedObject.name; 
            if (gameObject.HasTag(GameTags.Stored)) return;
            Game.Instance.userMenu.AddButton(this.gameObject, this.isMarkFroUnPack ? new KIconButtonMenu.ButtonInfo("action_deconstruct", PackAnythingString.UI.UNPACK_IT.NAME_OFF, new System.Action(this.OnClickCancel), tooltipText: PackAnythingString.UI.UNPACK_IT.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_deconstruct", PackAnythingString.UI.UNPACK_IT.NAME, new System.Action(this.OnClickPack), tooltipText: PackAnythingString.UI.UNPACK_IT.TOOLTIP));
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
            KBatchedAnimController animController = this.gameObject.GetComponent<KBatchedAnimController>();
            MagicPack magicPack = this.gameObject.GetComponent<MagicPack>();
            GameObject storedObject = magicPack.storedObject;
            if (storedObject != null) {
                int cell = Grid.PosToCell(this.gameObject);
                if (magicPack.isGeyser) {
                    this.CreateNeutronium(cell);
                    cell = Grid.CellAbove(cell);
                }
                Vector3 posCbc = Grid.CellToPosCBC(cell, storedObject.GetComponent<KBatchedAnimController>().sceneLayer);
                float num = -0.15f;
                posCbc.z += num;
                storedObject.transform.SetPosition(posCbc);
                storedObject.SetActive(true);
            }
            Util.KDestroyGameObject(this.gameObject);
            if (worker.HasTag(GameTags.Minion)) {
                RegisterReactEmotePair("ResearchComplete", Db.Get().Emotes.Minion.ResearchComplete, 3f, worker);
            }
        }

        public void CreateNeutronium(int cell) {
            int[] cells = new[]{
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

        private void RegisterReactEmotePair(string reactable_id, Emote emote, float max_trigger_time, Worker worker) {
            if ((UnityEngine.Object)worker.gameObject == (UnityEngine.Object)null)
                return;
            ReactionMonitor.Instance smi = worker.gameObject.GetSMI<ReactionMonitor.Instance>();
            if (smi == null)
                return;
            EmoteChore emoteChore = new EmoteChore((IStateMachineTarget)worker.gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteIdle, emote);
            SelfEmoteReactable reactable = new SelfEmoteReactable(worker.gameObject, (HashedString)reactable_id, Db.Get().ChoreTypes.Cough, max_trigger_time);
            emoteChore.PairReactable(reactable);
            reactable.SetEmote(emote);
            reactable.PairEmote(emoteChore);
            smi.AddOneshotReactable(reactable);
        }
    }
}
