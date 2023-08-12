using Database;
using System.Collections.Generic;
using System.Reflection;

namespace PackAnything {
    public class PackAnythingStaticVars {
        public static SkillPerk CanPack;
        public static Skill Pack;
        public static Components.Cmps<Surveyable> Surveyables;
        public static ChoreType Survey;
        public static ChoreType Active;
        public static StatusItem SurveyingItem;
        public static StatusItem ActivingBecaon;
        public static StatusItem WaitingSurvey;
        public static StatusItem WaitingActive;

        public static void Init() {
            PackAnythingStaticVars.CanPack = Db.Get().SkillPerks.Add((SkillPerk)new SimpleSkillPerk(nameof(CanPack), PackAnythingString.MISC.SKILL.PERK_DESC));
            PackAnythingStaticVars.Pack = Db.Get().Skills.Add(new Skill(nameof(Pack), PackAnythingString.MISC.SURVEY.NAME, PackAnythingString.MISC.SURVEY.DESCRIPTION, "", 3, "hat_role_building3", "skillbadge_role_building4", Db.Get().SkillGroups.Building.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionLarge, PackAnythingStaticVars.CanPack }, new List<string> { Db.Get().Skills.Building3.Id }));
            PackAnythingStaticVars.Surveyables = new Components.Cmps<Surveyable>();
            PackAnythingStaticVars.WaitingSurvey = Db.Get().MiscStatusItems.Add(new StatusItem("WaitingSurvey", "MISC", "status_item_needs_furniture", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID));
            PackAnythingStaticVars.WaitingActive = Db.Get().MiscStatusItems.Add(new StatusItem("WaitingActive", "MISC", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID));
            string[] chore_grops = new string[1] { "Build" };
            PackAnythingStaticVars.Survey = (ChoreType)typeof(ChoreTypes).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Db.Get().ChoreTypes, new object[] { "Survey", chore_grops, "", new string[0], STRINGS.DUPLICANTS.CHORES.SURVEY.NAME.ToString(), STRINGS.DUPLICANTS.CHORES.SURVEY.STATUS.ToString(), STRINGS.DUPLICANTS.CHORES.SURVEY.TOOLTIP.ToString(), false, 5000, STRINGS.DUPLICANTS.CHORES.SURVEY.REPORT_NAME.ToString() });
            PackAnythingStaticVars.Active = (ChoreType)typeof(ChoreTypes).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Db.Get().ChoreTypes, new object[] { "Active", chore_grops, "", new string[0], STRINGS.DUPLICANTS.CHORES.ACTIVE.NAME.ToString(), STRINGS.DUPLICANTS.CHORES.ACTIVE.STATUS.ToString(), STRINGS.DUPLICANTS.CHORES.ACTIVE.TOOLTIP.ToString(), false, 5000, STRINGS.DUPLICANTS.CHORES.ACTIVE.REPORT_NAME.ToString() });
        }
    }
}
