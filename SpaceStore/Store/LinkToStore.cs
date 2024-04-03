using KSerialization;
using SpaceStore.StoreRoboPanel;
using System;
using TUNING;

namespace SpaceStore.Store {
    public class LinkToStore: Workable{
        [Serialize]
        public bool isLinked = false;
        [Serialize] 
        public bool markToLink = false;
        [MyCmpReq]
        public Storage storage;

        public Chore chore;
        private static readonly EventSystem.IntraObjectHandler<LinkToStore> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<LinkToStore>((component, data) => component.OnRefreshUserMenu(data));
        private static readonly EventSystem.IntraObjectHandler<LinkToStore> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<LinkToStore>((component, data) => component.OnStorageChange(data));

        private void OnStorageChange(object _) {
            if (!isLinked) return;
            if ((storage.MassStored() / storage.capacityKg) == 1) {
                storage.ConsumeAllIgnoringDisease();
            } 
        }

        private void OnRefreshUserMenu(object _) {
            if (isLinked || markToLink) return;
            Game.Instance.userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo("action_repair", MyString.UI.ROBO_PANEL.NAME, new System.Action(new System.Action(() => { markToLink = true; UpdateChore(); }))));
        }

        private void UpdateChore() {
            if (chore == null && markToLink) {
                chore = new WorkChore<LinkToStore>(Db.Get().ChoreTypes.Build, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
            } else if (chore != null) {
                chore.Cancel("no longer need");
                chore = null;
            }
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            isLinked = true;
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            workerStatusItem = Db.Get().DuplicantStatusItems.Building;
            workingStatusItem = null;
            attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
            attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
            minimumAttributeMultiplier = 0.75f;
            skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
            skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
            Prioritizable.AddRef(base.gameObject);
            synchronizeAnims = false;
            multitoolContext = "build";
            multitoolHitEffectTag = EffectConfigs.BuildSplashId;
            workingPstComplete = null;
            workingPstFailed = null;
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            Subscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
            UpdateChore();
        }

    }
}
