using STRINGS;

namespace GravitasMemory {
  public class BUILDINGS {
    public class PREFABS {
      public class BASEP1 {
        public static LocString NAME = UI.FormatAsLink("实验机P1", "BASEP1");

        public static LocString DESC = "利用“时间之弓”做出来的机器，但对于" + UI.FormatAsLink("中子物质", "UNOBTANIUM") +
                                       "的产出，我想那位博士还没有找到解决方案。";

        public static LocString EFFECT = "将殖民地的" + UI.FormatAsLink("精炼金属", "REFINEDMETAL") + "还原为" +
                                         UI.FormatAsLink("金属矿石", "RAWMETAL") + "。\n\n由于技术原因，它也会产出少量" +
                                         UI.FormatAsLink("中子物质", "UNOBTANIUM") + ".";

        public static LocString RECIPE_DESCRIPTION = "将{1}还原为{0}。";
      }

      public class OVERRIDER {
        public static LocString NAME = UI.FormatAsLink("覆写器", "OVERRIDER");
        public static LocString DESC = "这下就有更多时间干其他事了。";
        public static LocString EFFECT = "对一些需要复制人操作的机器进行干扰。";
      }

      public class GEYSERC4 {
        public static LocString NAME = UI.FormatAsLink("实验机C4", "GEYSERC4");
        public static LocString DESC = "完美的机器。";

        public static LocString EFFECT =
          "将" + UI.FormatAsLink("原油", "CRUDEOIL") + "转化为" + UI.FormatAsLink("天然气", "METHANE") + "。";
      }

      public class CONDITIONERL8 {
        public static LocString NAME = UI.FormatAsLink("实验机L8", "CONDITIONERL8");
        public static LocString DESC = "得益于中子物质外壳，热量不会外泄，而且会输出一些电量。";

        public static LocString EFFECT = "冷却输入的" + UI.FormatAsLink("液体", "ELEMENTS_LIQUID") + "但不会向外释放" +
                                         UI.FormatAsLink("热量", "HEAT") + "。";
      }

      public class SEEDFERMENTER {
        public static LocString NAME = UI.FormatAsLink("种子酿造机", "SEEDFERMENTER");
        public static LocString DESC = "这些乙醇还是太少了。";

        public static LocString EFFECT = "将投入的种子发酵，产生" + UI.FormatAsLink("乙醇", "ETHANOL") + "和" +
                                         UI.FormatAsLink("泥土", "Dirt") + "。";
      }

      public class SONGMACHINE {
        public static LocString NAME = UI.FormatAsLink("摇篮曲", "SONGMACHINE");
        public static LocString DESC = "也许它播放的并不是摇篮曲。";
        public static LocString EFFECT = "加快周围动物蛋的孵化速度。";
      }
    }
  }

  public class ELEMENTS {
    public class CRYSTAL {
      public static string NAME = (LocString)"水晶";

      public static string DESC = (LocString)"由少量" + UI.FormatAsLink("中子物质", "UNOBTANIUM") + "和其他" +
                                  UI.FormatAsLink("矿物原料", "RAWMINERAL") + "加工而成的物质，蕴含极大能量，非常不稳定。";
    }
  }

  public class CREATURES {
    public class MODIFIERS {
      public class EGGCRAZY {
        public static LocString NAME = "疯狂";
        public static LocString DESC = "机器发出的频率让它焦躁不安，它想快点破壳而出。";
      }
    }
  }
}