using KSerialization;
using System;
using UnityEngine;

namespace PackAnything {
    [AddComponentMenu("KMonoBehaviour/Workable/Packable")]
    public class Packable : Workable {
        private Chore chore;
        [Serialize]
        private bool isMarkFroPack;
        private Guid statusItemGuid;
        public bool MarkFroPack => this.isMarkFroPack;

        private static readonly EventSystem.IntraObjectHandler<Packable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Packable>((Action<Packable, object>)((component, data) => component.OnRefreshUserMenu(data)));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            this.faceTargetWhenWorking = true;
            this.synchronizeAnims = false;
            this.requiredSkillPerk = Db.Get().SkillPerks.IncreaseCarryAmountMedium.Id;
            this.workerStatusItem = MixStatusItem.PackingItem;
            this.shouldShowSkillPerkStatusItem = false;
            this.alwaysShowProgressBar = false;
            this.faceTargetWhenWorking = false;
            this.multitoolContext = (HashedString)"capture";
            this.multitoolHitEffectTag = (Tag)"fx_capture_splash";
            this.SetWorkTime(20f);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            this.Subscribe<Packable>((int)GameHashes.RefreshUserMenu, Packable.OnRefreshUserMenuDelegate);
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
            this.progressBar.barColor = new Color(126f,22f,113f);
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            this.PackIt();
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
            KSelectable kSelectable = this.GetComponent<KSelectable>();
            this.isMarkFroPack = true;
            if (this.chore != null) return;
            chore = new WorkChore<Packable>(Db.Get().ChoreTypes.StorageFetch, this, only_when_operational: false);
            if (kSelectable != null) this.statusItemGuid = kSelectable.ReplaceStatusItem(this.statusItemGuid, MixStatusItem.WaitingPack);

        }

        public void PackIt() {
            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)MagicPackConfig.ID), Grid.CellToPos(Grid.PosToCell(this.gameObject)), Grid.SceneLayer.Creatures, name: this.gameObject.name );
            go.SetActive(true);
            MagicPack magicPack = go.AddOrGet<MagicPack>();
            magicPack.storedObject = this.gameObject;
            if (this.gameObject.HasTag(GameTags.GeyserFeature)) {
                magicPack.isGeyser = true;
                DealWithNeutronium(this.NaturalBuildingCell());
            }
            go.GetComponent<KBatchedAnimController>().Queue((HashedString)"ui");
            go.FindOrAddComponent<UserNameable>().savedName = this.gameObject.name;
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
                SimMessages.ReplaceElement(gameCell: x, new_element: SimHashes.Vacuum, ev: CellEventLogger.Instance.DebugTool,mass: 100f);
            }
        }
    }
}
