using Database;
using System.Collections.Generic;
using System.Linq;

namespace PackAnything {
    public class PackAnythingStaticVars{
        public static SkillPerk CanSurvey;
        public static Skill SurveySkill;
        public static Components.Cmps<Surveyable> Surveyables;
        public static ChoreType Survey;
        public static ChoreType Active;
        public static StatusItem SurveyingItem;
        public static StatusItem ActivingBecaon;
        public static StatusItem WaitingSurvey;
        public static StatusItem WaitingActive;
        
        public static void Init() {
            PackAnythingStaticVars.CanSurvey = Db.Get().SkillPerks.Add((SkillPerk) new SimpleSkillPerk(nameof(CanSurvey),PackAnythingString.MISC.SKILL.PERK_DESC));
            PackAnythingStaticVars.SurveySkill = Db.Get().Skills.Add(new Skill(nameof(SurveySkill), PackAnythingString.MISC.SURVEY.NAME, PackAnythingString.MISC.SURVEY.DESCRIPTION, "", 3, "hat_role_building3", "skillbadge_role_building4", Db.Get().SkillGroups.Building.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionLarge, PackAnythingStaticVars.CanSurvey},new List<string> { Db.Get().Skills.Building3.Id}));
            PackAnythingStaticVars.Surveyables = new Components.Cmps<Surveyable>();
            PackAnythingStaticVars.WaitingSurvey = Db.Get().MiscStatusItems.Add(new StatusItem("WaitingSurvey", "MISC", "status_item_needs_furniture", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID));
            PackAnythingStaticVars.WaitingActive = Db.Get().MiscStatusItems.Add(new StatusItem("WaitingActive", "MISC", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID));

        }
    }
}
