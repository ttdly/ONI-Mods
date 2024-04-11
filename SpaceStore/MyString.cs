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

            public class SELL {
                public static LocString TITLE = "SELL";
                public static LocString TOOL_TIP = "Selling this will earn you {coin} Space Coin";
            }

            public class STORE {
                public class STOREDIALOG {
                    public static LocString NAME_TEMPLATE = "{name}\n{quantity}";
                    public static LocString DISABLEED = "Can not purchase items without a Printing Pod";
                    public static LocString REFRESH = "Reload Config";
                }
            }
        }
    }
}
