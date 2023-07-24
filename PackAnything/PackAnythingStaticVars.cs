using Database;
using System.Collections.Generic;
using System.Linq;

namespace PackAnything {
    public class PackAnythingStaticVars{
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
            PackAnythingStaticVars.CanPack = Db.Get().SkillPerks.Add((SkillPerk) new SimpleSkillPerk(nameof(CanPack),PackAnythingString.MISC.SKILL.PERK_DESC));
            PackAnythingStaticVars.Pack = Db.Get().Skills.Add(new Skill(nameof(Pack), PackAnythingString.MISC.SURVEY.NAME, PackAnythingString.MISC.SURVEY.DESCRIPTION, "", 3, "hat_role_building3", "skillbadge_role_building4", Db.Get().SkillGroups.Building.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionLarge, PackAnythingStaticVars.CanPack},new List<string> { Db.Get().Skills.Building3.Id}));
            PackAnythingStaticVars.Surveyables = new Components.Cmps<Surveyable>();
            PackAnythingStaticVars.SurveyingItem = Db.Get().DuplicantStatusItems.Add(new StatusItem("SurveyItem", "DUPLICANTS","",StatusItem.IconType.Info,NotificationType.Neutral,false,OverlayModes.None.ID,true,2));
            PackAnythingStaticVars.ActivingBecaon = Db.Get().DuplicantStatusItems.Add(new StatusItem("ActiveBeacon", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID,true,2));
            PackAnythingStaticVars.WaitingSurvey = Db.Get().MiscStatusItems.Add(new StatusItem("WaitingSurvey", "MISC", "status_item_needs_furniture", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID));
            PackAnythingStaticVars.WaitingActive = Db.Get().MiscStatusItems.Add(new StatusItem("WaitingActive", "MISC", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID));
            string[] chore_grops = new string[1] { "Build" };
            PackAnythingStaticVars.Survey = Db.Get().ChoreTypes.Add(new ChoreType("Survey",Db.Get().ChoreTypes,chore_grops,"",STRINGS.DUPLICANTS.CHORES.SURVEY.NAME,STRINGS.DUPLICANTS.CHORES.SURVEY.STATUS,STRINGS.DUPLICANTS.CHORES.SURVEY.TOOLTIP, Enumerable.Empty<Tag>(), 97110,5000));
            PackAnythingStaticVars.Survey.reportName = STRINGS.DUPLICANTS.CHORES.SURVEY.REPORT_NAME;
            PackAnythingStaticVars.Active = Db.Get().ChoreTypes.Add(new ChoreType("Active", Db.Get().ChoreTypes, chore_grops, "", STRINGS.DUPLICANTS.CHORES.ACTIVE.NAME, STRINGS.DUPLICANTS.CHORES.ACTIVE.STATUS, STRINGS.DUPLICANTS.CHORES.ACTIVE.TOOLTIP, Enumerable.Empty<Tag>(), 97120, 5000));
            PackAnythingStaticVars.Active.reportName = STRINGS.DUPLICANTS.CHORES.ACTIVE.REPORT_NAME;
        }
    }
}
