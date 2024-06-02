using HarmonyLib;
using Klei.AI;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;

namespace SaltBox {
  public sealed class Mod : UserMod2 {
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      // 初始化 PUtil 的文件
      PUtil.InitLibrary();
      // 检查模组版本是否更新
      new PVersionCheck().Register(this, new SteamVersionChecker());
      new PLocalization().Register();
#if DEBUG
            ModUtil.RegisterForTranslation(typeof(MyStrings));
#endif
    }
  }

  public static class StaticVars {
    public static readonly Tag StoredInSaltBox = new Tag(nameof(StoredInSaltBox));
  }

  public class Patches {
    [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
      public static void Prefix() {
        LocString.CreateLocStringKeys(typeof(MyStrings.BUILDINGS));
        ModUtil.AddBuildingToPlanScreen("Food", SaltBoxConfig.ID);
        Db.Get().Techs.Get("FineDining").unlockedItemIDs.Add(SaltBoxConfig.ID);
      }
    }

    [HarmonyPatch(typeof(Rottable.Instance), nameof(Rottable.Instance.RefreshModifiers))]
    public class Rottable_Instance_RefreshModifiers_Patch {
      public static void Postfix(Rottable.Instance __instance) {
        if (__instance.master.gameObject.HasTag(StaticVars.StoredInSaltBox)) {
          var amounts = __instance.master.gameObject.GetAmounts();
          var instance = amounts.Get("Rot");
          var modifier = new AttributeModifier(instance.amount.Id, 1.2f, MyStrings.MISC.StoreInSaltBoxModifer);
          instance.deltaAttribute.Add(modifier);
        }
      }
    }
  }
}