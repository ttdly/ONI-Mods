using SpaceStore.MyGeyser;
using SpaceStore.StoreRoboPanel;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceStore.Store
{
    public class StoreList
    {

        public static List<MarketItem> marketItems = new List<MarketItem>();

        public enum ObjectType
        {
            Object,
            Element,
            NeedPack,
        }

        public class MarketItem
        {
            public Sprite sprite;
            public string name = "test";
            public int price;
            public Tag tag;
            public Tag innerTag;
            public SimHashes simHashes;
            public ObjectType type;
            public int unit = 0;
            public int count = 0;

            // 初始化元素商品
            public MarketItem
                (Tag tag, SimHashes simHashes = 0, int unit = 0, int price = 10, ObjectType type = ObjectType.Element)
            {
                this.tag = tag;
                this.price = price;
                this.type = type;
                this.simHashes = simHashes;
                this.unit = unit;
                sprite = Def.GetUISprite(tag).first;
                name = UI.StripLinkFormatting(Strings.Get("STRINGS.ELEMENTS." + tag.ToString().ToUpper() + ".NAME"));
            }


            public MarketItem(Tag tag, int price = 10, ObjectType type = ObjectType.Object)
            {
                this.tag = tag;
                sprite = Def.GetUISprite(tag).first;
                this.price = price;
                this.type = type;
            }

            public override string ToString()
            {
                return $"\n名称：\t{name}\n标签：\t{tag}\n价格：\t{price}\n数量：\t{count}\n单位：\t{unit}\n类型：\t{type}\n内部：\t{innerTag}";
            }
        }

        public static List<Tuple<SimHashes, int, int>> elements = new List<Tuple<SimHashes, int, int>>() {
            new Tuple<SimHashes, int, int>(SimHashes.Steel, 10, 500),
            new Tuple<SimHashes, int, int>(SimHashes.SuperInsulator, 2, 300),
            new Tuple<SimHashes, int, int>(SimHashes.SuperCoolant, 1, 200),
        };

        public static List<Tuple<string, int>> objects = new List<Tuple<string, int>>() {
            new Tuple<string, int>(GeneShufflerRechargeConfig.ID, 400),
            new Tuple<string, int>(RoboPanelConfig.ID,600),
            new Tuple<string, int>(GeoActivatorConfig.ID, 600),
        };

        public static List<Tuple<string, int>> packs = new List<Tuple<string, int>>() {
            new Tuple<string, int>(OilWellConfig.ID, 500),
        };

        public static void Init()
        {
            foreach (Tuple<SimHashes, int, int> element in elements)
            {
                marketItems.Add(new MarketItem(element.first.CreateTag(), element.first, element.second, element.third));
            }


            foreach (Tuple<string, int> obj in objects)
            {
                marketItems.Add(new MarketItem(obj.first, obj.second));
            }

            //foreach (Tuple<string, int> pack in packs)
            //{
            //    marketItems.Add(new MarketItem(Assets.GetPrefab(pack.first).GetProperName(), pack.first, pack.second));
            //}
        }
    }
}
