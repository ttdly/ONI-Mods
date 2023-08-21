using KSerialization;
using System.Collections.Generic;
using TUNING;

namespace TweaksPack.Tweakable 
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class BaseTweakable : Workable {
        [Serialize]
        public bool isMarkForTweak = false;
        private Storage storage;
        private FetchList2 fetchList;
        private Chore chore;
        public Dictionary<Tag, float> materialNeeds;
        private static readonly EventSystem.IntraObjectHandler<BaseTweakable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BaseTweakable>((component, data) => component.OnRefreshUserMenu(data));
        private CellOffset[] PlacementOffsets {
            get {
                Building component1 = GetComponent<Building>();
                if (component1 != null)
                    return component1.Def.PlacementOffsets;
                OccupyArea component2 = GetComponent<OccupyArea>();
                if (component2 != null)
                    return component2.OccupiedCellsOffsets;
                Debug.Assert(false, "Ack! We put a Tweakable on something that's neither a Building nor OccupyArea!", this);
                return null;
            }
        }

        protected override void OnStopWork(Worker worker) {
            base.OnStopWork(worker);
            DestroyWork();
        }

        protected override void OnAbortWork(Worker worker) {
            base.OnAbortWork(worker);
            DestroyWork();
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            storage.ConsumeAllIgnoringDisease();
            DestroyWork();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            storage = gameObject.AddComponent<Storage>();
            storage.showInUI = false;
            CellOffset[][] table = OffsetGroups.InvertedStandardTable;
            CellOffset[] filter = null;
            Building component = GetComponent<Building>();
            if (component != null && component.Def.IsTilePiece) {
                table = OffsetGroups.InvertedStandardTableWithCorners;
                filter = component.Def.ConstructionOffsetFilter;
            }
            SetOffsetTable(OffsetGroups.BuildReachabilityTable(PlacementOffsets, table, filter));
            storage.SetOffsetTable(OffsetGroups.BuildReachabilityTable(PlacementOffsets, table, filter));
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            Subscribe((int)GameHashes.StatusChange, OnRefreshUserMenuDelegate);
            Toogle();
        }

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            workerStatusItem = Db.Get().DuplicantStatusItems.Building;
            workingStatusItem = null;
            attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
            attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
            minimumAttributeMultiplier = 0.75f;
            skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
            skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
            Prioritizable.AddRef(gameObject);
            requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
            shouldShowSkillPerkStatusItem = false;
            synchronizeAnims = false;
            multitoolContext = (HashedString)"build";
            multitoolHitEffectTag = (Tag)EffectConfigs.BuildSplashId;
            workingPstComplete = null;
            workingPstFailed = null;
        }

        protected virtual void OnFetchComplete() {
            chore = new WorkChore<BaseTweakable>(Db.Get().ChoreTypes.Build, this, only_when_operational: false);
        }

        private void OnRefreshUserMenu(object _) {
            if (gameObject.HasTag(TweakableStaticVars.Tags.DontTweak)) return;
            Game.Instance.userMenu.AddButton(gameObject, isMarkForTweak ? new KIconButtonMenu.ButtonInfo("action_cancel", TweaksPackStrings.UI.BUTTON.OFF.NAME, new System.Action(new System.Action(() => { isMarkForTweak = !isMarkForTweak; Toogle(); })), tooltipText: TweaksPackStrings.UI.BUTTON.OFF.TOOL_TIP) : new KIconButtonMenu.ButtonInfo("action_repair", TweaksPackStrings.UI.BUTTON.ON.NAME, new System.Action(() => { isMarkForTweak = !isMarkForTweak; Toogle(); }), tooltipText: TweaksPackStrings.UI.BUTTON.ON.TOOLTIP));
        }

        private void ActiveWork() {
            if (fetchList == null) {
                fetchList = new FetchList2(storage, Db.Get().ChoreTypes.BuildFetch);
                foreach (KeyValuePair<Tag, float> kvp in materialNeeds) {
                    fetchList.Add(kvp.Key, amount: kvp.Value);
                    MaterialNeeds.UpdateNeed(kvp.Key, kvp.Value, gameObject.GetMyWorldId());
                }
                fetchList.Submit(new System.Action(OnFetchComplete), true);
            };
        }

        private void DestroyWork() {
            if (storage.Count > 0) {
                storage.DropAll();
            }
            if (chore != null) {
                chore.Cancel("Cancel Tweak");
                chore = null;
            }
            if (fetchList != null) {
                fetchList.Cancel("Cancle Fetch");
                fetchList = null;
            }
            if (isMarkForTweak) isMarkForTweak = false;
        }

        private void Toogle() {
            if (isMarkForTweak) {
                if (chore != null || fetchList != null) return;
                ActiveWork();
            } else {
                DestroyWork();
            }
        }

        public void Refresh() {
            isMarkForTweak = false;
            if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(gameObject))
                DetailsScreen.Instance.Show(false);
            OnRefreshUserMenu(null);
        }
    }
}
