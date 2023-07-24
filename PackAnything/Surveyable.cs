using KSerialization;
using PeterHan.PLib.Core;
using System;
using TUNING;
using UnityEngine;


namespace PackAnything {
    [RequireComponent(typeof(Prioritizable))]
    [AddComponentMenu("KMonoBehaviour/Workable/Packable")]
    public class Surveyable : Workable {
        private Chore chore;
        [Serialize]
        private bool isMarkForSurvey;
        private Guid statusItemGuid;
        public bool MarkFroPack => this.isMarkForSurvey;
        private CellOffset[] PlacementOffsets {
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

        private static readonly EventSystem.IntraObjectHandler<Surveyable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Surveyable>((Action<Surveyable, object>)((component, data) => component.OnRefreshUserMenu(data)));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            this.faceTargetWhenWorking = true;
            this.synchronizeAnims = false;
            this.requiredSkillPerk = PackAnythingStaticVars.CanPack.Id;
            this.workerStatusItem = MixStatusItem.PackingItem;
            this.shouldShowSkillPerkStatusItem = false;
            this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
            this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
            this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
            this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
            this.alwaysShowProgressBar = false;
            this.faceTargetWhenWorking = false;
            this.multitoolContext = (HashedString)"build";
            this.multitoolHitEffectTag = (Tag)EffectConfigs.BuildSplashId;
            this.SetWorkTime(50f);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            PackAnythingStaticVars.Surveyables.Add(this);
            this.Subscribe<Surveyable>((int)GameHashes.RefreshUserMenu, Surveyable.OnRefreshUserMenuDelegate);
            this.Subscribe<Surveyable>((int)GameHashes.StatusChange, Surveyable.OnRefreshUserMenuDelegate);
            CellOffset[][] table = OffsetGroups.InvertedStandardTable;
            CellOffset[] filter = (CellOffset[])null;
            this.SetOffsetTable(OffsetGroups.BuildReachabilityTable(this.PlacementOffsets, table, filter));
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
            this.progressBar.barColor = new Color(0.5f, 0.7f, 1.0f, 1f);
            KSelectable kSelectable = this.GetComponent<KSelectable>();
            if (kSelectable != null) this.statusItemGuid = kSelectable.RemoveStatusItem(this.statusItemGuid);
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            this.CreateBeacon();
            if ((UnityEngine.Object)DetailsScreen.Instance != (UnityEngine.Object)null && DetailsScreen.Instance.CompareTargetWith(this.gameObject))
                DetailsScreen.Instance.Show(false);
            this.gameObject.AddTag("Surveyed");
            this.OnClickCancel();
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            PackAnythingStaticVars.Surveyables.Remove(this);
        }


        // 自定义的方法
        public void OnRefreshUserMenu(object data) {
            if (this.gameObject.HasTag("Surveyed")) return;
            if (this.gameObject.HasTag("OilWell") && this.gameObject.GetComponent<BuildingAttachPoint>()?.points[0].attachedBuilding != null) return;
            Game.Instance.userMenu.AddButton(this.gameObject, this.isMarkForSurvey ? new KIconButtonMenu.ButtonInfo("action_follow_cam", PackAnythingString.UI.SURVEY.NAME_OFF, new System.Action(this.OnClickCancel), tooltipText: PackAnythingString.UI.SURVEY.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_follow_cam", PackAnythingString.UI.SURVEY.NAME, new System.Action(this.OnClickSurvey), tooltipText: PackAnythingString.UI.SURVEY.TOOLTIP));
        }

        public void OnClickCancel() {
            if (this.chore == null && !this.isMarkForSurvey) {
                return;
            }
            this.isMarkForSurvey = false;
            this.chore.Cancel("Surveyable.CancelChore");
            this.chore = null;
            KSelectable kSelectable = this.GetComponent<KSelectable>();
            if (kSelectable != null) this.statusItemGuid = kSelectable.RemoveStatusItem(this.statusItemGuid);
        }

        public void OnClickSurvey() {
            Prioritizable.AddRef(this.gameObject);
            KSelectable kSelectable = this.GetComponent<KSelectable>();
            this.isMarkForSurvey = true;
            if (this.chore != null) return;
            chore = new WorkChore<Surveyable>(PackAnythingChoreTypes.Survey, this, only_when_operational: false, is_preemptable: true);
            if (kSelectable != null) this.statusItemGuid = kSelectable.ReplaceStatusItem(this.statusItemGuid, MixStatusItem.WaitingPack);
        }

        public void CreateBeacon() {
            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)BeaconConfig.ID), Grid.CellToPos(Grid.PosToCell(this.gameObject)), Grid.SceneLayer.Creatures, name: this.gameObject.name);
            go.SetActive(true);
            Beacon becaon = go.GetComponent<Beacon>();
            becaon.originCell = Grid.PosToCell(this.gameObject);
            GameObject originGo = Grid.Objects[becaon.originCell, 1];
            if (this.gameObject.HasTag(GameTags.GeyserFeature)) {
                becaon.isGeyser = true;
            }
            string name;
            if (becaon.isGeyser) {
                name = this.gameObject.name;
            } else {
                name = Strings.Get("STRINGS.BUILDINGS.PREFABS." + this.gameObject.name.Replace("Complete", "").ToUpper() + ".NAME");
            }
            go.FindOrAddComponent<UserNameable>().savedName = name;
        }

        public void DealWithNeutronium(int cell) {
            int[] cells = new[]{
                Grid.CellDownLeft(cell),
                Grid.CellBelow(cell),
                Grid.CellDownRight(cell),
                Grid.CellRight(Grid.CellDownRight(cell))
            };
            foreach (int x in cells) {
                if (Grid.Element.Length < x || Grid.Element[x] == null) {
                    new IndexOutOfRangeException();
                    return;
                }
                Element e = Grid.Element[x];
                if (!e.IsSolid || !e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM")) continue;
                SimMessages.ReplaceElement(gameCell: x, new_element: SimHashes.Vacuum, ev: CellEventLogger.Instance.DebugTool, mass: 100f);
            }
        }
    }
}
