using HarmonyLib;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using System;

namespace MoreTiles {
    public sealed class Mod : KMod.UserMod2 {
        public override void OnLoad(Harmony harmony) {
            base.OnLoad(harmony);
            // 初始化 PUtil 的文件
            PUtil.InitLibrary();
            // 检查模组版本是否更新
            new PVersionCheck().Register(this, new SteamVersionChecker());
            new PLocalization().Register();
#if DEBUG
            ModUtil.RegisterForTranslation(typeof(Strings));
#endif
        }
    }

    public class Strings {
        public class BUILDINGS {
            public class PREFABS {
                public class AUTOLIGHTTILE {
                    public static LocString NAME = "Auto Light Tile";
                    public static LocString DESC = " The tile will automatically detect whether there are duplicates working on the tile, and if so, they will light up for the duplicates.";
                    public static LocString EFFECT = "Provide illumination automatically for the duplicates working.";
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
                    new string[]{ AutoLightTileConfig.ID, "Base", "InteriorDecor", "tiles"},
                });
                foreach (string[] building in NewBuildings) {
                    AddNewBuilding(building[0], building[1], building[2], building[3]);
                }
            }

        }

        public static void AddNewBuilding(string building_id, string plan_screen_cat_id, string tech_id, string string_key) {
            ModUtil.AddBuildingToPlanScreen(plan_screen_cat_id, building_id);
            Db.Get().Techs.Get(tech_id).unlockedItemIDs.Add(building_id);
            TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(building_id, string_key);
        }
    }
}
