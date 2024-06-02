using System.Collections.Generic;
using Database;
using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using STRINGS;

namespace PackAnything {
  public class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      PUtil.InitLibrary();
      new PVersionCheck().Register(this, new SteamVersionChecker());
    }
  }

  public class StaticVars {
    public static SkillPerk CanPack;
    public static Skill Pack;

    public static void Init() {
      CanPack = Db.Get().SkillPerks.Add(new SimpleSkillPerk(nameof(CanPack), "NULL"));
      Pack = Db.Get().Skills.Add(new Skill(nameof(Pack), "NULL", "NULL", "", 3, "hat_role_building3",
        "skillbadge_role_building3", Db.Get().SkillGroups.Building.Id,
        new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionLarge, CanPack },
        new List<string> { Db.Get().Skills.Building3.Id }));
    }
  }

  public class Patches {
    [HarmonyPatch(typeof(MinionResume), "OnSpawn")]
    public static class MinionResume_OnSpawn_Patch {
      public static void Postfix(MinionResume __instance) {
        if (__instance.HasPerk(StaticVars.CanPack)) {
          __instance.UnmasterSkill(StaticVars.Pack.Id);
          __instance.SetHats(__instance.CurrentHat, null);
          __instance.ApplyTargetHat();
          var notification = new Notification(MISC.NOTIFICATIONS.RESETSKILL.NAME, NotificationType.Good,
            (notificationList, data) => MISC.NOTIFICATIONS.RESETSKILL.TOOLTIP + notificationList.ReduceMessages(false));
          __instance.GetComponent<Notifier>().Add(notification);
        }
      }
    }

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
      public static void Prefix() {
        StaticVars.Init();
      }
    }
  }
}