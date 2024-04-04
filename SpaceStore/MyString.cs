namespace SpaceStore {
    public class MyString {
        public class ITEM {
            public class GEO_ACTIVATOR {
                // 地质激活器
                public static LocString NAME = "Geo-Activator";
                // 挖掘星球核心的力量 - 利用创造间歇泉的能力，主宰资源之流。这款尖端设备允许你操控地质力量，在你最需要的地方召唤出自然能源的源泉。
                public static LocString DESC = "Tap into the planet's core - harness the power to create a geyser and dominate the resource stream. This cutting-edge device lets you manipulate geological forces, conjuring up a natural font of energy exactly where you need it.";
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
                    public static LocString NAME_TEMPLATE = "{name}{quantity} [{price} coins]";
                    public static LocString DISABLEED = "Cannot purchase items without a Printing Pod";
                    public static LocString CURR_COIN = "Current Space Coins: ";
                    public static LocString BUY = "Purchase";
                }
            }
        }
    }
}
