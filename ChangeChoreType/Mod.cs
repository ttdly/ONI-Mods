using HarmonyLib;
using System.Collections.Generic;

namespace ChangeChoreType {
    internal class Mod :KMod.UserMod2{
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
        }
    }
}

[HarmonyPatch(typeof(ChoreType), MethodType.Constructor, typeof(string), typeof(ResourceSet), typeof(string[]), typeof(string), typeof(string), typeof(string), typeof(string), typeof(IEnumerable<Tag>),typeof(int), typeof(int))]
public class ChoreType_Patch {
    public static void Prefix(string id, ref string[] chore_groups) {
        if (id == "FoodFetch") {
            chore_groups = new string[1] { "Storage" };
        }
    }
}