using STRINGS;

namespace GravitasMemory {
    public class BUILDINGS {
        public class PREFABS {
            public class BASEP1 {
                public static LocString NAME = UI.FormatAsLink("实验机P1", "BASEP1");
                public static LocString DESC = (LocString)("利用“时间之弓”做出来的机器，但对于" + UI.FormatAsLink("中子物质", "UNOBTANIUM") + "的产出，我想那位博士还没有找到解决方案。");
                public static LocString EFFECT = (LocString)("将殖民地的" + UI.FormatAsLink("精炼金属", "REFINEDMETAL") + "还原为" + UI.FormatAsLink("金属矿石", "RAWMETAL") + "。\n\n由于技术原因，它也会产出少量" + UI.FormatAsLink("中子物质", "UNOBTANIUM") + ".");
                public static LocString RECIPE_DESCRIPTION = ("将{1}还原为{0}。");
            }
            public class OVERRIDER {
                public static LocString NAME = UI.FormatAsLink("覆写器", "OVERRIDER");
                public static LocString DESC = (LocString)("这下就有更多时间干其他事了。");
                public static LocString EFFECT = (LocString)("对一些需要复制人操作的机器进行干扰。" );
            }
            public class GEYSERV2 {
                public static LocString NAME = UI.FormatAsLink("实验机V2", "GEYSERV2");
                public static LocString DESC = (LocString)("对火山的一次模拟，还有待改进。");
                public static LocString EFFECT = (LocString)("将水晶转换为岩浆喷出");
            }
            public class GEYSERC4 {
                public static LocString NAME = UI.FormatAsLink("实验机C4", "GEYSERC4");
                public static LocString DESC = (LocString)("完美的复刻。");
                public static LocString EFFECT = (LocString)("原油转为天然气。");
            }
            public class CONDITIONERL8 {
                public static LocString NAME = UI.FormatAsLink("实验机L8", "CONDITIONERL8");
                public static LocString DESC = (LocString)("得益于中子物质外壳，热量不会外泄，但是内部产生的微弱电量还是需要及时排出");
                public static LocString EFFECT = (LocString)("冷却液体");
            }
        }
    }
    public class CODEX {
        public class STORY_TRAITS {
            public class GRAVITAS_MEMORY {
                public static LocString NAME = (LocString)("庄严旧事");
                public static LocString DESCRIPTION = (LocString)("中子物质！");
                public static LocString DESCRIPTION_SHORT = (LocString)"将打印舱和时间之弓集合，这个机型还是有中子物质的产出。";

                public class BEGIN_POPUP {
                    public static LocString NAME = (LocString)"故事特质：庄严旧事";
                    public static LocString CODEX_NAME = (LocString)"初遇";
                    public static LocString DESCRIPTION = (LocString)"我发现了另一个打印舱，这是添加了时间之弓的实验机型。\n\n它能把精炼金属恢复成金属矿石，但是会有部分转化成中子物质。\n\n我想这些中子物质可以制作其他的机型。";
                }
            }
        }
    }
    public class CUSTOM {
        public class STORY_TRAITS {
            public class GRAVITAS_MEMORY {
                public static string ICON_NAME = "GravitasMemory_icon";
                public static string IMAGE_NAME = "GravitasMemory_image";
                public static string CDOEX_NAME = "gravitas_memory_codex_icon";
            }
        }
    }
}
