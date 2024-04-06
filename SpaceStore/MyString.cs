namespace SpaceStore {
    public class MyString {

        public class OPTIONS {
            public static LocString ITEM_WIDTH = "每个条目的宽度";
            public static LocString COL = "每列的条目数";
            public static LocString DIALOG_HEIGHT = "窗口总高度";
        }

        public class ITEM {
            public class GEO_ACTIVATOR {
                // 地质种子
                public static LocString NAME = "Geo-Seed";
                // “爆炸带来地质种子，流星使之发芽。”\n 在种子的位置生成间歇泉，注意！这会召唤流星雨。"
                public static LocString DESC = "\"Explosions bring the Geo-Seed, and meteors make them germinate.\" \n Generate a geyser at the location of the seeds, caution! This will summon a meteor shower.";
            }
            public class ROBO_PANEL {
                // 机器人面板
                public static LocString NAME = "Robo-Panel";
                // 赋予你的基地自动化力量 - 将你的设备升级为独立运行，提高效率的同时释放复制人，让他们专注于更重要的任务。
                public static LocString DESC = "Empower your base with automation - upgrade your devices to operate independently, boosting efficiency and freeing up your Duplicants for more critical tasks.";
            }
        }


        public class UI {
            public class GEO_ACTIVATOR {
                // 选择一个间歇泉
                public static LocString TITLE = "Choose a Geyser";
                // 激活
                public static LocString TEXT1 = "ACTIVATE";
            }

            public class ROBO_PANEL {
                // 添加机器人面板
                public static LocString NAME = "Add Robo-Panel";
            }

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
                    public static LocString DISABLEED = "Cannot purchase items without a Printing Pod";
                    public static LocString CURR_COIN = "Current Space Coins: ";
                    public static LocString BUY = "Purchase";
                }
            }
        }
    }
}
