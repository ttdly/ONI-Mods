﻿using KSerialization;
using PeterHan.PLib.Core;
using System;
using TUNING;
using UnityEngine;


namespace PackAnything {
    [RequireComponent(typeof(Prioritizable))]
    public class Surveyable : Workable {
        private Chore chore;
        [Serialize]
        private bool isMarkForSurvey;
        [Serialize]
        public bool isSurveyed = false;
        private Guid statusItemGuid;
        public bool MarkFroSurvey => isMarkForSurvey;
        public CellOffset[] PlacementOffsets {
            get {
                Building component1 = GetComponent<Building>();
                if (component1 != null)
                    return component1.Def.PlacementOffsets;
                OccupyArea component2 = GetComponent<OccupyArea>();
                if (component2 != null)
                    return component2.OccupiedCellsOffsets;
                Debug.Assert(false, "Ack! We put a Surveyable on something that's neither a Building nor OccupyArea!", this);
                return null;
            }
        }

        private static readonly EventSystem.IntraObjectHandler<Surveyable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Surveyable>((component, data) => component.OnRefreshUserMenu(data));

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            faceTargetWhenWorking = true;
            synchronizeAnims = false;
            requiredSkillPerk = Db.Get().SkillPerks.CanArtGreat.Id;
            workerStatusItem = PackAnythingStaticVars.SurveyingItem;
            shouldShowSkillPerkStatusItem = false;
            attributeConverter = Db.Get().AttributeConverters.ArtSpeed;
            attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
            skillExperienceSkillGroup = Db.Get().SkillGroups.Art.Id;
            skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
            alwaysShowProgressBar = false;
            multitoolContext = (HashedString)"paint";
            multitoolHitEffectTag = (Tag)"fx_paint_splash";
            SetWorkTime(50f);
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            Subscribe((int)GameHashes.StatusChange, OnRefreshUserMenuDelegate);
            CellOffset[][] table = OffsetGroups.InvertedStandardTable;
            CellOffset[] filter = null;
            SetOffsetTable(OffsetGroups.BuildReachabilityTable(PlacementOffsets, table, filter));
            if (isSurveyed) {
                PackAnythingStaticVars.SurveableCmps.Add(this);
                return;
            }
            if (isMarkForSurvey) {
                OnClickSurvey();
            }
        }

        protected override void OnStartWork(Worker worker) {
            base.OnStartWork(worker);
            progressBar.barColor = PackAnythingStaticVars.PrimaryColor;
            RemoveStatus();
        }

        protected override void OnAbortWork(Worker worker) {
            base.OnAbortWork(worker);
            if (MarkFroSurvey) {
                AddStatus();
            }
        }

        protected override void OnCompleteWork(Worker worker) {
            base.OnCompleteWork(worker);
            if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(gameObject))
                DetailsScreen.Instance.Show(false);
            isSurveyed = true;
            OnRefreshUserMenu(null);
            PackAnythingStaticVars.SurveableCmps.Add(this);
            OnClickCancel();
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            PackAnythingStaticVars.SurveableCmps.Remove(this);
        }

        // 自定义的方法
        public void OnRefreshUserMenu(object _) {
            if (isSurveyed || gameObject.HasTag("DontShowSurveyable")) return;
            if (gameObject.HasTag("OilWell") && gameObject.GetComponent<BuildingAttachPoint>()?.points[0].attachedBuilding != null) return;
            Game.Instance.userMenu.AddButton(gameObject, isMarkForSurvey ? new KIconButtonMenu.ButtonInfo("action_follow_cam", PackAnythingString.UI.SURVEY.NAME_OFF, new System.Action(OnClickCancel), tooltipText: PackAnythingString.UI.SURVEY.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_follow_cam", PackAnythingString.UI.SURVEY.NAME, new System.Action(OnClickSurvey), tooltipText: PackAnythingString.UI.SURVEY.TOOLTIP));
        }

        public void OnClickCancel() {
            if (chore == null && !isMarkForSurvey) {
                return;
            }
            isMarkForSurvey = false;
            chore.Cancel("Surveyable.CancelChore");
            chore = null;
            RemoveStatus();
        }

        public void OnClickSurvey() {
            if (DebugHandler.InstantBuildMode) {
                OnClickCancel();
                isSurveyed = true;
                OnRefreshUserMenu(null);
                PackAnythingStaticVars.SurveableCmps.Add(this);
                return;
            }
            Prioritizable.AddRef(gameObject);
            isMarkForSurvey = true;
            if (chore != null) return;
            chore = new WorkChore<Surveyable>(PackAnythingStaticVars.Survey, this, only_when_operational: false);
            AddStatus();
        }

        public void RemoveThisFromList() {
            isSurveyed = false;
            PUtil.LogDebug(PackAnythingStaticVars.SurveableCmps.Remove(this));
        }

        private void AddStatus() {
            KSelectable kSelectable = GetComponent<KSelectable>();
            if (kSelectable != null) statusItemGuid = kSelectable.ReplaceStatusItem(statusItemGuid, PackAnythingStaticVars.WaitingSurvey);
        }

        private void RemoveStatus() {
            KSelectable kSelectable = GetComponent<KSelectable>();
            if (kSelectable != null) statusItemGuid = kSelectable.RemoveStatusItem(statusItemGuid);
        }
    }
}
