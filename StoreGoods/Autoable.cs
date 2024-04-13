using KSerialization;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace StoreGoods {
    public class Autoable : Workable {
        private Chore chore;
        public bool isComplex;
        public Tag tinkerMaterialTag = RoboPanelConfig.tag;
        public float tinkerMaterialAmount = 5f;
        
        [Serialize]
        public bool stage1 = false;
        [Serialize]
        public bool stage2 = false;
        [MyCmpGet]
        private Storage storage;
        [MyCmpGet]
        private RoboPanel panel;

        public HashedString choreTypeTinker = Db.Get().ChoreTypes.Build.IdHash;
        public HashedString choreTypeFetch = Db.Get().ChoreTypes.BuildFetch.IdHash;
        private static readonly EventSystem.IntraObjectHandler<Autoable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Autoable>((component, data) => component.OnRefreshUserMenu(data));

        private SchedulerHandle updateHandle;

        private bool hasReservedMaterial;

        public static Autoable MakeDupTinkerable(GameObject prefab, bool isComplex) {
            Autoable autoable = prefab.AddOrGet<Autoable>();
            autoable.isComplex = isComplex;
            autoable.requiredSkillPerk = PowerControlStationConfig.ROLE_PERK;
            autoable.SetWorkTime(180f);
            autoable.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
            autoable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
            autoable.choreTypeTinker = Db.Get().ChoreTypes.Build.IdHash;
            autoable.choreTypeFetch = Db.Get().ChoreTypes.BuildFetch.IdHash;
            autoable.multitoolContext = "powertinker";
            autoable.multitoolHitEffectTag = "fx_powertinker_splash";
            autoable.shouldShowSkillPerkStatusItem = false;
            prefab.AddOrGet<Storage>();
            prefab.GetComponent<KPrefabID>().prefabInitFn += delegate (GameObject inst) {
                inst.GetComponent<Autoable>().SetOffsetTable(OffsetGroups.InvertedStandardTable);
            };
            return autoable;
        }


        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_use_machine_kanim") };
            attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
            faceTargetWhenWorking = true;
            synchronizeAnims = false;
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Prioritizable.AddRef(gameObject);
        }

        protected override void OnCleanUp() {
            UpdateMaterialReservation(shouldReserve: false);
            if (updateHandle.IsValid) {
                updateHandle.ClearScheduler();
            }

            Prioritizable.RemoveRef(gameObject);
            base.OnCleanUp();
        }

        private void OnRefreshUserMenu(object _) {
            if (gameObject.HasTag(StaticVars.AutoTag) || stage1 || stage2) return;
            Game.Instance.userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo("action_repair", MyString.UI.ROBO_PANEL.NAME, new System.Action(new System.Action(() => { stage1 = true; UpdateChore();}))));
        }

        private void UpdateChore() {
            if (chore == null) {
                if (stage1) {
                    UpdateMaterialReservation(shouldReserve: true);
                    chore = new FetchChore(Db.Get().ChoreTypes.GetByHash(choreTypeFetch), storage, tinkerMaterialAmount, new HashSet<Tag> { tinkerMaterialTag }, FetchChore.MatchCriteria.MatchID, Tag.Invalid, null, null, run_until_complete: true, OnFetchComplete, null, null, Operational.State.Functional);
                } else if(stage2) {
                    chore = new WorkChore<Autoable>(Db.Get().ChoreTypes.GetByHash(choreTypeTinker), this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
                }
            } else if (chore != null) {
                chore.Cancel("Nooooooooooooo");
                chore = null;
            }
        }

 

        private void UpdateMaterialReservation(bool shouldReserve) {
            if (shouldReserve && !hasReservedMaterial) {
                MaterialNeeds.UpdateNeed(tinkerMaterialTag, tinkerMaterialAmount, gameObject.GetMyWorldId());
                hasReservedMaterial = shouldReserve;
            } else if (!shouldReserve && hasReservedMaterial) {
                MaterialNeeds.UpdateNeed(tinkerMaterialTag, 0f - tinkerMaterialAmount, gameObject.GetMyWorldId());
                hasReservedMaterial = shouldReserve;
            }
        }

        private void OnFetchComplete(Chore data) {
            UpdateMaterialReservation(shouldReserve: false);
            storage.ConsumeIgnoringDisease(tinkerMaterialTag, tinkerMaterialAmount);
            stage2 = true;
            stage1 = false;
            chore = null;
            UpdateChore();
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            panel.canAuto = true;
            panel.isComplex = isComplex;
            panel.ToogleAuto();
            UpdateMaterialReservation(shouldReserve: false);
            chore = null;
            stage2 = false;
        }
    }
}
