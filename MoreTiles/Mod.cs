using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using Saltbox;
using System;
using UnityEngine;

namespace MoreTiles {
    public sealed class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            // 初始化 PUtil 的文件
            PUtil.InitLibrary();
            // 检查模组版本是否更新
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }
    }

    public class Strings {
        public class BUILDINGS {
            public class PREFABS {
                public class LIGHTTILE {
                    public static LocString NAME = "Salt Box";
                    //里外都被盐覆盖的口粮箱
                    public static LocString DESC = "Ration box covered with salt both inside and out";
                    public static LocString EFFECT = "Enhancing food freshness";
                }
            }
        }
    }

    public static class Paches {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch {
            public static void Prefix() {
                LocString.CreateLocStringKeys(typeof(Strings.BUILDINGS));
                Array NewBuildings = (new Array[] {
                    new string[]{ AutoLightTileConfig.ID, "Equipment", "AdvancedResearch", "Kyyy"},
                });

                foreach (string[] building in NewBuildings) {
                    AddNewBuilding(building[0], building[1], building[2], building[3]);
                }
            }

        }

        public static void AddNewBuilding(string building_id, string plan_screen_cat_id, string tech_id, string string_key) {
            ModUtil.AddBuildingToPlanScreen(plan_screen_cat_id, building_id); // 添加到建筑栏
            Db.Get().Techs.Get(tech_id).unlockedItemIDs.Add(building_id);
            TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(building_id, string_key);
        }
    }
}
