
using STRINGS;

namespace PackAnything {
    public class PackAnythingString {
        public class UI {
            public class PACK_IT {
                public static LocString NAME = (LocString)"Pack";
                public static LocString TOOLTIP = (LocString)"Pack this object for move.";
                public static LocString NAME_OFF = (LocString)"Cancel Pack";
                public static LocString TOOLTIP_OFF = (LocString)"Cancel order to pack this object.";
            }
            public class UNPACK_IT {
                public static LocString NAME = (LocString)"UnPack";
                public static LocString TOOLTIP = (LocString)"UnPacks Magic Pack packed by Duplicants.";
                public static LocString NAME_OFF = (LocString)"Cancel UnPack";
                public static LocString TOOLTIP_OFF = (LocString)"Cancel order to unpack this object";
            }
            public class STATUS {
                public static LocString MARK_FOR_PACK = (LocString)"Mark for pack";
            }
        }

        public class MISC{
            public class MAGIC_PACK {
                public static LocString NAME = (LocString)"Magic Pack";
                public static LocString DESC = (LocString)"Can pack something.";
            }
        }
    }
}
