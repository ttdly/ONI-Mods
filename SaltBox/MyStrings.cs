using STRINGS;

namespace SaltBox {
  internal class MyStrings {
    public class MISC {
      public static LocString StoreInSaltBoxModifer = "Stored in a salt box";
    }

    public class BUILDINGS {
      public class PREFABS {
        public class SALTBOX {
          public static LocString NAME = UI.FormatAsLink("SaltBox", "SALTBOX");
          public static LocString DESC = "My salt has finally found its purpose!";

          public static LocString EFFECT =
            "When food spoils, it uses the salt within the box to enhance its freshness.";
        }
      }
    }
  }
}