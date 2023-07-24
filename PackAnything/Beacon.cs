using Klei.AI;
using KSerialization;
using PeterHan.PLib.Core;
using System;
using TUNING;
using UnityEngine;

namespace PackAnything {
    [SerializationConfig(MemberSerialization.OptIn)]
    [AddComponentMenu("KMonoBehaviour/Workable/Becaon")]
    public class Beacon : Workable {
        private Chore chore;
        [Serialize]
        private bool isMarkForActive;
        [Serialize]
        public bool isGeyser = false;
        [Serialize]
        public int originCell;
        private int unoCount;
        private Guid statusItemGuid;
        public bool MarkFroPack => this.isMarkForActive;
        private CellOffset[] placementOffsets {
            get {
                Building component1 = this.GetComponent<Building>();
                if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
                    return component1.Def.PlacementOffsets;
                OccupyArea component2 = this.GetComponent<OccupyArea>();
                if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
                    return component2.OccupiedCellsOffsets;
                Debug.Assert(false, (object)"Ack! We put a Packable on something that's neither a Building nor OccupyArea!", (UnityEngine.Object)this);
                return (CellOffset[])null;
            }
        }

        private static readonly EventSystem.IntraObjectHandler<Beacon> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Beacon>((Action<Beacon, object>)((component, data) => component.OnRefreshUserMenu(data)));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            this.workerStatusItem = PackAnythingStaticVars.ActivingBecaon;
            this.faceTargetWhenWorking = true;
            this.synchronizeAnims = false;
            this.requiredSkillPerk = PackAnythingStaticVars.CanPack.Id;
            this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
            this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
            this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
            this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
            this.multitoolContext = (HashedString)"demolish";
            this.multitoolHitEffectTag = (Tag)EffectConfigs.DemolishSplashId;
            this.faceTargetWhenWorking = true;
            this.SetWorkTime(50f);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            this.Subscribe<Beacon>((int)GameHashes.RefreshUserMenu, Beacon.OnRefreshUserMenuDelegate);
            this.Subscribe<Beacon>((int)GameHashes.StatusChange, Beacon.OnRefreshUserMenuDelegate);
            CellOffset[][] table = OffsetGroups.InvertedStandardTable;
            CellOffset[] filter = (CellOffset[])null;
            this.SetOffsetTable(OffsetGroups.BuildReachabilityTable(this.placementOffsets, table, filter));
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            this.ActiveIt(worker);
            this.OnClickCancel();
        }

        protected override void OnAbortWork(Worker worker) {
            base.OnAbortWork(worker);
            if (this.isMarkForActive) {
                this.AddStatus();
            }
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
            this.progressBar.barColor = new Color(0.5f, 0.7f, 1.0f, 1f);
            this.RemoveStatus();
        }

        // 自定义的方法
        public void OnRefreshUserMenu(object data) {
            if (gameObject.HasTag(GameTags.Stored)) return;
            Game.Instance.userMenu.AddButton(this.gameObject, this.isMarkForActive ? new KIconButtonMenu.ButtonInfo("action_empty_contents", PackAnythingString.UI.ACTIVATE.NAME_OFF, new System.Action(this.OnClickCancel), tooltipText: PackAnythingString.UI.ACTIVATE.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_empty_contents", PackAnythingString.UI.ACTIVATE.NAME, new System.Action(this.OnClickActive), tooltipText: PackAnythingString.UI.ACTIVATE.TOOLTIP));
        }

        public void OnClickCancel() {
            if (this.chore == null && !this.isMarkForActive) {
                return;
            }
            this.isMarkForActive = false;
            this.chore.Cancel("Active.CancelChore");
            this.chore = null;
            this.RemoveStatus();
        }

        public void OnClickActive() {
            this.isMarkForActive = true;
            if (this.chore != null) return;
            this.chore = new WorkChore<Beacon>(Db.Get().ChoreTypes.Deconstruct, this, only_when_operational: false);
            this.chore.choreType.statusItem = PackAnythingStaticVars.Active.statusItem;
            this.chore.choreType.Name = PackAnythingStaticVars.Active.Name;
            this.chore.choreType.reportName = PackAnythingStaticVars.Active.reportName;
            AddStatus();
        }

        public void ActiveIt(Worker worker) {
            GameObject originObject = null;
            foreach ( Surveyable surveyable in PackAnythingStaticVars.Surveyables.Items) {
                if (Grid.PosToCell(surveyable) == this.originCell) {
                    originObject = surveyable.gameObject;
                }
            }
            if (originObject != null) {
                int cell = Grid.PosToCell(this.gameObject);
                if (this.isGeyser) {
                    this.DeleteNeutronium(Grid.PosToCell(originObject));
                    this.CreateNeutronium(cell);
                    cell = Grid.CellAbove(cell);
                }
                Vector3 posCbc = Grid.CellToPosCBC(cell, originObject.FindOrAddComponent<KBatchedAnimController>().sceneLayer);
                float num = -0.15f;
                posCbc.z += num;
                originObject.transform.SetPosition(posCbc);
                originObject.SetActive(true);
                originObject.FindOrAddComponent<OccupyArea>().UpdateOccupiedArea();
                originObject.RemoveTag("Surveyed");
            }
            Util.KDestroyGameObject(this.gameObject);
            if (worker.HasTag(GameTags.Minion)) {
                RegisterReactEmotePair("ActiveBecaon", Db.Get().Emotes.Minion.ResearchComplete, 3f, worker);
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
                if (this.unoCount == 0) continue;
                if (Grid.Element.Length < x || Grid.Element[x] == null) {
                    PUtil.LogError("Out of index.");
                    new IndexOutOfRangeException();
                    return;
                }
                Element e = Grid.Element[x];
                if (!Grid.IsValidCell(x)) continue;
                SimMessages.ReplaceElement( gameCell: x, new_element: SimHashes.Unobtanium, ev: CellEventLogger.Instance.DebugTool, mass: 100f);
                this.unoCount--;
            }
        }

        public void DeleteNeutronium(int cell) {
            int[] cells = new[]{
                Grid.CellDownLeft(cell),
                Grid.CellBelow(cell),
                Grid.CellDownRight(cell),
                Grid.CellRight(Grid.CellDownRight(cell))
            };
            this.unoCount = 0;
            foreach (int x in cells) {
                if (Grid.Element.Length < x || Grid.Element[x] == null) {
                    new IndexOutOfRangeException();
                    return;
                }
                Element e = Grid.Element[x];
                if (!e.IsSolid || !e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM")) continue;
                SimMessages.ReplaceElement(gameCell: x, new_element: SimHashes.Vacuum, ev: CellEventLogger.Instance.DebugTool, mass: 100f);
                this.unoCount++;
            }
        }

        private void RegisterReactEmotePair(string reactable_id, Emote emote, float max_trigger_time, Worker worker) {
            if ((UnityEngine.Object)worker.gameObject == (UnityEngine.Object)null)
                return;
            ReactionMonitor.Instance smi = worker.gameObject.GetSMI<ReactionMonitor.Instance>();
            if (smi == null)
                return;
            EmoteChore emoteChore = new EmoteChore((IStateMachineTarget)worker.gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteIdle, emote);
            SelfEmoteReactable reactable = new SelfEmoteReactable(worker.gameObject, (HashedString)reactable_id, Db.Get().ChoreTypes.Emote);
            emoteChore.PairReactable(reactable);
            reactable.SetEmote(emote);
            reactable.PairEmote(emoteChore);
            smi.AddOneshotReactable(reactable);
        }

        private void AddStatus() {
            KSelectable kSelectable = this.GetComponent<KSelectable>();
            if (kSelectable != null) this.statusItemGuid = kSelectable.ReplaceStatusItem(this.statusItemGuid, PackAnythingStaticVars.WaitingActive);
        }

        private void RemoveStatus() {
            KSelectable kSelectable = this.GetComponent<KSelectable>();
            if (kSelectable != null) this.statusItemGuid = kSelectable.RemoveStatusItem(this.statusItemGuid);
        }
    }
}
