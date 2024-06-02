namespace StoreGoods {
  public class MyString {
    public class ITEM {
      public class GEO_ACTIVATOR {
        // 地质种子
        public static LocString NAME = "Geo-Seed";

        // “爆炸带来地质种子，流星使之发芽。”\n 在种子的位置生成间歇泉，注意！这会召唤流星雨。
        public static LocString DESC =
          "\"Explosions bring the Geo-Seed, and meteors make them germinate.\" \n Generate a geyser at the location of the seeds, caution! This will summon a meteor shower.";
      }
    }

    public class UI {
      public class GEO_ACTIVATOR {
        // 选择一个间歇泉
        public static LocString TITLE = "Choose a Geyser";

        // 激活
        public static LocString TEXT1 = "ACTIVATE";

        // 激活之后会在当前位置生成选定的间歇泉
        public static LocString TOOLTIP =
          "After activation, a selected geyser will be generated at the current location.";
      }
    }
  }
}