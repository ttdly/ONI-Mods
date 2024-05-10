namespace SpaceStore {
    public class MyString {

        public class OPTIONS {
            public static LocString ITEM_WIDTH = "Item Width";
            public static LocString COL = "Col Number";
            public static LocString DIALOG_HEIGHT = "Window Height";
        }


        public class UI {
            public static LocString COIN_NAME = "Space Coin";

            public class MENU_TOOL {
                public static LocString TITLE = "Space Store";
                public static LocString TOOL_TIP = "Open Sapce Store";
            }

            public class MENU_TOOL_2 {
                public static LocString TITLE = "Sell Objects";
                public static LocString TOOL_TIP = "Sell objects on the ground.";
            }

            public class SELL {
                public static LocString TITLE = "SELL";
                public static LocString TOOL_TIP = "Sell these {item} and you will get {coin} space coin";
            }

            public class STORE {
                public class STOREDIALOG {
                    public static LocString NAME_TEMPLATE = "{name}\n{quantity}";
                    public static LocString DISABLEED = "Can not purchase goods without a Printing Pod";
                    public static LocString REFRESH = "Reload Config";
                    public static LocString OPEN_CONFIG_FOLDER = "Open Config Folder";
                    public static LocString OPEN_WEB_PAGE = "Create a configuration file";
                }
            }
        }
    }
}
