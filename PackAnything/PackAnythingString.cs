
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
                public static LocString NAME = (LocString)"Open";
                public static LocString TOOLTIP = (LocString)"Open the Magic Pack packed by Duplicants.";
                public static LocString NAME_OFF = (LocString)"Cancel Open";
                public static LocString TOOLTIP_OFF = (LocString)"Cancel order to unpack this item.";
            }
        }

        public class MISC{
            public class MAGIC_PACK {
                public static LocString NAME = (LocString)"Magic Pack";
                public static LocString DESC = (LocString)"Inside it, there are some things. Open it.";
            }
        }
    }

    public class STRINGS {
        public class MISC {
            public class STATUSITEMS {
                public class WAITINGPACK {
                    public static LocString NAME = (LocString)"Pack Pending";
                    public static LocString TOOLTIP = (LocString)"A Duplicant is in the process of packing this item.";
                }
                public class WAITINGUNPACK {
                    public static LocString NAME = (LocString)"Unpack Pending";
                    public static LocString TOOLTIP = (LocString)"A Duplicant is in the process of unpacking this magic pack.";
                }
            }
        }

        public class DUPLICANTS {
            public class STATUSITEMS {
                public class PACKINGITEM {
                    public static LocString NAME = (LocString)"Packing";
                    public static LocString TOOLTIP = (LocString)"This Duplicant is attempting to pack the item.";
                }
                public class UNPACKINGITEM {
                    public static LocString NAME = (LocString)"Unpacking";
                    public static LocString TOOLTIP = (LocString)"This Duplicant is attempting to unpack a magic pack.";
                }
            }

            public class CHORES {
                public class PACK {
                    public static LocString NAME = (LocString)"Pack";
                    public static LocString STATUS = (LocString)"Packing";
                    public static LocString TOOLTIP = (LocString)"This Duplicant is attempting to pack the item.";
                    public static LocString REPORT_NAME = (LocString)"Pack {0}";
                }
                public class UNPACK {
                    public static LocString NAME = (LocString)"Unpack";
                    public static LocString STATUS = (LocString)"Unpacking";
                    public static LocString TOOLTIP = (LocString)"This Duplicant is attempting to unpack a magic pack.";
                    public static LocString REPORT_NAME = (LocString)"Unpack {0}";
                }
            }
        }


    }
}
