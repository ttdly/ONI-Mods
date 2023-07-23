using Database;
using System.Collections.Generic;

namespace PackAnything {
    public class PackAnythingSkill{
        public static SkillPerk CanPack;
        public static Skill Pack;
        public static Components.Cmps<Packable> Packables; 

        public static void Init() {
            PackAnythingSkill.CanPack = Db.Get().SkillPerks.Add((SkillPerk) new SimpleSkillPerk(nameof(CanPack),PackAnythingString.MISC.SKILL.PERK_DESC));
            PackAnythingSkill.Pack = Db.Get().Skills.Add(new Skill(nameof(Pack), PackAnythingString.MISC.PACK.NAME, PackAnythingString.MISC.PACK.DESCRIPTION, "", 3, "hat_role_art1", "skillbadge_role_building4", Db.Get().SkillGroups.Building.Id, new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionLarge, PackAnythingSkill.CanPack},new List<string> { Db.Get().Skills.Building3.Id}));
            PackAnythingSkill.Packables = new Components.Cmps<Packable>();
        }
    }
}
