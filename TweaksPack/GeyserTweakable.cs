using KSerialization;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using TUNING;
using UnityEngine;


namespace TweaksPack {
    [SerializationConfig(MemberSerialization.OptIn)]
    [AddComponentMenu("KMonoBehaviour/Workable/Updatable")]
    public class GeyserTweakable : Workable {
        public Dictionary<Tag, float> materialNeeds = new Dictionary<Tag, float>();
        [Serialize]
        private bool isMarkForWork = false;
        [MyCmpAdd]
        private readonly Storage storage;
        private FetchList2 fetchList;
        private Chore chore;
        private static readonly EventSystem.IntraObjectHandler<GeyserTweakable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<GeyserTweakable>(((component, data) => component.OnRefreshUserMenu(data)));
        private CellOffset[] PlacementOffsets {
            get {
                Building component1 = GetComponent<Building>();
                if (component1 != null)
                    return component1.Def.PlacementOffsets;
                OccupyArea component2 = GetComponent<OccupyArea>();
                if (component2 != null)
                    return component2.OccupiedCellsOffsets;
                Debug.Assert(false, "Ack! We put a Updatable on something that's neither a Building nor OccupyArea!", this);
                return null;
            }
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
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
            ShowGeyserChangeDialog();
            DestroyWork();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            CellOffset[][] table = OffsetGroups.InvertedStandardTable;
            CellOffset[] filter = null;
            Building component = GetComponent<Building>();
            if (component != null && component.Def.IsTilePiece) {
                table = OffsetGroups.InvertedStandardTableWithCorners;
                filter = component.Def.ConstructionOffsetFilter;
            }
            SetOffsetTable(OffsetGroups.BuildReachabilityTable(PlacementOffsets, table, filter));
            storage.SetOffsetTable(OffsetGroups.BuildReachabilityTable(PlacementOffsets, table, filter));
            Subscribe<GeyserTweakable>((int)GameHashes.RefreshUserMenu, GeyserTweakable.OnRefreshUserMenuDelegate);
            Subscribe<GeyserTweakable>((int)GameHashes.StatusChange, GeyserTweakable.OnRefreshUserMenuDelegate);
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
            synchronizeAnims = false;
            multitoolContext = (HashedString)"build";
            multitoolHitEffectTag = (Tag)EffectConfigs.BuildSplashId;
            workingPstComplete = null;
            workingPstFailed = null;
        }

        private void OnFetchComplete() {
            PUtil.LogDebug("Fetch Complete");
            SetWorkTime(100f);
            chore = new WorkChore<GeyserTweakable>(Db.Get().ChoreTypes.Build, this, only_when_operational: false);
        }

        public void OnRefreshUserMenu(object _) {
            Game.Instance.userMenu.AddButton(gameObject, isMarkForWork ? new KIconButtonMenu.ButtonInfo("action_cancel", AbleStrings.UI.BUTTON_OFF.NAME, new System.Action(new System.Action(() => { isMarkForWork = !isMarkForWork; Toogle(); })), tooltipText: AbleStrings.UI.BUTTON_OFF.TOOL_TIP) : new KIconButtonMenu.ButtonInfo("action_repair", AbleStrings.UI.BUTTON_ON.NAME, new System.Action(() => { isMarkForWork = !isMarkForWork; Toogle(); }), tooltipText: AbleStrings.UI.BUTTON_ON.TOOLTIP));
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
                chore.Cancel("Cancel Update");
                chore = null;
            }
            if (fetchList != null) {
                fetchList.Cancel("Cancle Fetch");
                fetchList = null;
            }
            if (isMarkForWork) isMarkForWork = false;
        }

        private void Toogle() {
            if (isMarkForWork) {
                if (chore != null || fetchList != null) return;
                ActiveWork();
            } else {
                DestroyWork();
            }
        }

        public void Refresh() {
            isMarkForWork = false;
            if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(gameObject))
                DetailsScreen.Instance.Show(false);
            OnRefreshUserMenu(null);
        }

        private void ShowGeyserChangeDialog() {
            OverlayScreen.Instance?.ToggleOverlay(OverlayModes.Priorities.ID, false);
            SpeedControlScreen.Instance?.Pause(false);
            if (!(CameraController.Instance != null))
                return;
            CameraController.Instance.DisableUserCameraControl = true;
            new GeyserChangeDialog(gameObject);
        }
    }
}
