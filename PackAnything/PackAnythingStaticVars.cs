using Database;
using System.Collections.Generic;

namespace PackAnything {
    public class PackAnythingStaticVars{
        public static SkillPerk CanPack;
        public static Skill Pack;
        public static Components.Cmps<Surveyable> Surveyables; 

        public static void Init() {
            PackAnythingStaticVars.CanPack = Db.Get().SkillPerks.Add((SkillPerk) new SimpleSkillPerk(nameof(CanPack),PackAnythingString.MISC.SKILL.PERK_DESC));
            PackAnythingStaticVars.Pack = Db.Get().Skills.Add(new Skill(nameof(Pack), PackAnythingString.MISC.SURVEY.NAME, PackAnythingString.MISC.SURVEY.DESCRIPTION, "", 3, "hat_role_building3", "skillbadge_role_building4", Db.Get().SkillGroups.Building.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionLarge, PackAnythingStaticVars.CanPack},new List<string> { Db.Get().Skills.Building3.Id}));
            PackAnythingStaticVars.Surveyables = new Components.Cmps<Surveyable>();
        }
    }
}
