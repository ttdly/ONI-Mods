using System.Collections.Generic;

namespace TweaksPack {
  public class TweakableStaticVars {
    public static class Tags {
      public static Tag AutoTweaked = new Tag("AutoTweaked");
      public static Tag DontTweak = new Tag("Tweaked");
      public static Tag AutoHarvest = new Tag("AutoHarvest");
    }

    public static class WorkTime {
#if DEBUG
            public static float debug = 1f;
            public static float Auto = debug;
            public static float ComplexFabricator = debug;
            public static float Plant = debug;
            public static float Geyser = debug;
#else
      public static float Auto = 100f;
      public static float ComplexFabricator = 100f;
      public static float Plant = 100f;
      public static float Geyser = 100f;
#endif
    }

    public static class MaterialNeeds {
#if DEBUG
            public static Dictionary<Tag, float> debug = new Dictionary<Tag, float>() {
                {SimHashes.SandStone.CreateTag(), 10f }
            };
            public static Dictionary<Tag, float> Auto = debug;
            public static Dictionary<Tag, float> ComplexFabricator = debug;
            public static Dictionary<Tag, float> Plant = debug;
            public static Dictionary<Tag, float> Geyser = debug;
#else
      public static Dictionary<Tag, float> Auto = new Dictionary<Tag, float> {
        { SimHashes.Glass.CreateTag(), 200f },
        { SimHashes.Steel.CreateTag(), 100f }
      };

      public static Dictionary<Tag, float> ComplexFabricator = new Dictionary<Tag, float> {
        { SimHashes.Glass.CreateTag(), 200f },
        { SimHashes.Steel.CreateTag(), 100f }
      };

      public static Dictionary<Tag, float> Plant = new Dictionary<Tag, float> {
        { SimHashes.Sulfur.CreateTag(), 10f },
        { SimHashes.Algae.CreateTag(), 50f }
      };

      public static Dictionary<Tag, float> Geyser = new Dictionary<Tag, float> {
        { SimHashes.Katairite.CreateTag(), 100 },
        { SimHashes.Ceramic.CreateTag(), 800 }
      };
#endif
    }
  }
}