﻿using Database;
using PeterHan.PLib.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PackAnything {
    public class PackAnythingStaticVars {
        public static SkillPerk CanPack;
        public static Skill Pack;
        public static ChoreType Survey;
        public static ChoreType Active;
        public static StatusItem SurveyingItem;
        public static StatusItem ActivingBecaon;
        public static StatusItem WaitingSurvey;
        public static StatusItem WaitingActive;
        public static List<Surveyable> SurveableCmps;
        public static Sprite ToolIcon;
        public static Color PrimaryColor;
        public static Surveyable targetSurveyable;
        public static WorldModifier targetModifier;

        public static void Init() {
            CanPack = Db.Get().SkillPerks.Add(new SimpleSkillPerk(nameof(CanPack), PackAnythingString.MISC.SKILL.PERK_DESC));
            Pack = Db.Get().Skills.Add(new Skill(nameof(Pack), PackAnythingString.MISC.SURVEY.NAME, PackAnythingString.MISC.SURVEY.DESCRIPTION, "", 3, "hat_role_building3", "skillbadge_role_building4", Db.Get().SkillGroups.Building.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionLarge, CanPack }, new List<string> { Db.Get().Skills.Building3.Id }));
            WaitingSurvey = Db.Get().MiscStatusItems.Add(new StatusItem("WaitingSurvey", "MISC", "status_item_needs_furniture", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID));
            WaitingActive = Db.Get().MiscStatusItems.Add(new StatusItem("WaitingActive", "MISC", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID));
            string[] chore_grops = new string[1] { "Build" };
            Survey = (ChoreType)typeof(ChoreTypes).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Db.Get().ChoreTypes, new object[] { "Survey", chore_grops, "", new string[0], STRINGS.DUPLICANTS.CHORES.SURVEY.NAME.ToString(), STRINGS.DUPLICANTS.CHORES.SURVEY.STATUS.ToString(), STRINGS.DUPLICANTS.CHORES.SURVEY.TOOLTIP.ToString(), false, 5000, STRINGS.DUPLICANTS.CHORES.SURVEY.REPORT_NAME.ToString() });
            Active = (ChoreType)typeof(ChoreTypes).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Db.Get().ChoreTypes, new object[] { "Active", chore_grops, "", new string[0], STRINGS.DUPLICANTS.CHORES.ACTIVE.NAME.ToString(), STRINGS.DUPLICANTS.CHORES.ACTIVE.STATUS.ToString(), STRINGS.DUPLICANTS.CHORES.ACTIVE.TOOLTIP.ToString(), false, 5000, STRINGS.DUPLICANTS.CHORES.ACTIVE.REPORT_NAME.ToString() });
            SurveableCmps = new List<Surveyable>();
            ToolIcon = PUIUtils.LoadSprite("PackAnything.images.tooIcon.png");
            PrimaryColor = new Color(0.5f, 0.7f, 1.0f, 1f);
            targetSurveyable = null;
        }
    }
}
