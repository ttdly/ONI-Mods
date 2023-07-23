using KSerialization;
using System;
using TUNING;
using UnityEngine;


namespace PackAnything {
    [RequireComponent(typeof(Prioritizable))]
    [AddComponentMenu("KMonoBehaviour/Workable/Packable")]
    public class Packable : Workable {
        private Chore chore;
        [Serialize]
        private bool isMarkFroPack;
        private Guid statusItemGuid;
        public bool MarkFroPack => this.isMarkFroPack;
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

        private static readonly EventSystem.IntraObjectHandler<Packable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Packable>((Action<Packable, object>)((component, data) => component.OnRefreshUserMenu(data)));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            this.faceTargetWhenWorking = false;
            this.synchronizeAnims = false;
            this.requiredSkillPerk = PackAnythingSkill.CanPack.Id;
            this.workerStatusItem = MixStatusItem.PackingItem;
            this.shouldShowSkillPerkStatusItem = false;
            this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
            this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
            this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
            this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
            this.alwaysShowProgressBar = false;
            this.faceTargetWhenWorking = false;
            this.multitoolContext = (HashedString)"capture";
            this.multitoolHitEffectTag = (Tag)"fx_capture_splash";
            this.SetWorkTime(50f);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            this.Subscribe<Packable>((int)GameHashes.RefreshUserMenu, Packable.OnRefreshUserMenuDelegate);
            this.Subscribe<Packable>((int)GameHashes.StatusChange, Packable.OnRefreshUserMenuDelegate);
            CellOffset[][] table = OffsetGroups.InvertedStandardTable;
            CellOffset[] filter = (CellOffset[])null;
            this.SetOffsetTable(OffsetGroups.BuildReachabilityTable(this.placementOffsets, table, filter));
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
            this.progressBar.barColor = new Color(0.5f, 0.7f, 1.0f, 1f);
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            this.PackIt();
            SimCellOccupier component2 = this.GetComponent<SimCellOccupier>();
            if ((UnityEngine.Object)DetailsScreen.Instance != (UnityEngine.Object)null && DetailsScreen.Instance.CompareTargetWith(this.gameObject))
                DetailsScreen.Instance.Show(false);
            this.gameObject.FindOrAddComponent<OccupyArea>().ApplyToCells = false;
            this.OnClickCancel();
        }


        // 自定义的方法
        public void OnRefreshUserMenu(object data) {
            if (this.gameObject.HasTag("OilWell") && this.gameObject.GetComponent<BuildingAttachPoint>()?.points[0].attachedBuilding != null) return;
            Game.Instance.userMenu.AddButton(this.gameObject, this.isMarkFroPack ? new KIconButtonMenu.ButtonInfo("action_capture", PackAnythingString.UI.PACK_IT.NAME_OFF, new System.Action(this.OnClickCancel), tooltipText: PackAnythingString.UI.PACK_IT.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_capture", PackAnythingString.UI.PACK_IT.NAME, new System.Action(this.OnClickPack), tooltipText: PackAnythingString.UI.PACK_IT.TOOLTIP));
        }

        public void OnClickCancel() {
            KSelectable kSelectable = this.GetComponent<KSelectable>();
            if (this.chore == null && !this.isMarkFroPack) {
                return;
            }
            this.isMarkFroPack = false;
            this.chore.Cancel("Packable.CancelChore");
            this.chore = null;
            if (kSelectable != null) this.statusItemGuid = kSelectable.RemoveStatusItem(this.statusItemGuid);
        }

        public void OnClickPack() {
            Prioritizable.AddRef(this.gameObject);
            KSelectable kSelectable = this.GetComponent<KSelectable>();
            this.isMarkFroPack = true;
            if (this.chore != null) return;
            chore = new WorkChore<Packable>(PackAnythingChoreTypes.Pack, this, only_when_operational: false);
            if (kSelectable != null) this.statusItemGuid = kSelectable.ReplaceStatusItem(this.statusItemGuid, MixStatusItem.WaitingPack);
        }

        public void PackIt() {
            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)MagicPackConfig.ID), Grid.CellToPos(Grid.PosToCell(this.gameObject)), Grid.SceneLayer.Creatures, name: this.gameObject.name);
            go.SetActive(true);
            
            GameObject goo= Grid.Objects[Grid.PosToCell(this.gameObject), 1];
            MagicPack magicPack = go.GetComponent<MagicPack>();
            magicPack.storedObject = this.gameObject;
            if (this.gameObject.HasTag(GameTags.GeyserFeature)) {
                magicPack.isGeyser = true;
                DealWithNeutronium(this.NaturalBuildingCell()); 
            }
            string name;
            go.GetComponent<KBatchedAnimController>().Queue((HashedString)"ui");
            if (magicPack.isGeyser) {
                name = this.gameObject.name;
            } else {
                name = Strings.Get("STRINGS.BUILDINGS.PREFABS." + this.gameObject.name.Replace("Complete", "").ToUpper() + ".NAME");
            }
            go.FindOrAddComponent<UserNameable>().savedName = name;
            this.gameObject.SetActive(false);
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
                if (!e.IsSolid && !e.id.ToString().ToUpperInvariant().Equals("UNOBTANIUM")) continue;
                SimMessages.ReplaceElement(gameCell: x, new_element: SimHashes.Vacuum, ev: CellEventLogger.Instance.DebugTool, mass: 100f);
            }
        }
    }
}
