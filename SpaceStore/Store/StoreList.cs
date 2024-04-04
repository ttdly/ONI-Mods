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
            public ObjectType type;
            public int quantity = 0;
            public CarePackageInfo info;

            // 初始化元素商品
            public MarketItem(SimHashes simHashes = 0, int count = 0, int price = 9999, ObjectType type = ObjectType.Element){
                this.price = price;
                this.type = type;
                quantity = count;
                tag = simHashes.CreateTag();
                sprite = Def.GetUISprite(tag).first;
                name = UI.StripLinkFormatting(Strings.Get("STRINGS.ELEMENTS." + tag.ToString().ToUpper() + ".NAME"));
                info = new CarePackageInfo(tag.ToString(), count, null);
            }


            public MarketItem(Tag tag, int price = 9999, int count = 1,ObjectType type = ObjectType.Object){
                this.tag = tag;
                sprite = Def.GetUISprite(tag).first;
                this.price = price;
                this.type = type;
                quantity = count;
                name = Assets.GetPrefab(tag).GetProperName();
                info = new CarePackageInfo(tag.ToString(), count, null);
            }

            public string GetDesc() => MyString.UI.STORE.STOREDIALOG.NAME_TEMPLATE.Replace("{name}", name)
                    .Replace("{quantity}", GetSpawnableQuantityOnly()).Replace("{price}", price.ToString());

            private string GetSpawnableQuantityOnly() {
                if (ElementLoader.GetElement(info.id.ToTag()) != null)
                    return string.Format((string)UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, GameUtil.GetFormattedMass(info.quantity));
                return EdiblesManager.GetFoodInfo(info.id) != null ? string.Format((string)UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, GameUtil.GetFormattedCaloriesForItem((Tag)info.id, info.quantity)) : string.Format((string)UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, info.quantity.ToString());
            }

            public override string ToString()
            {
                return $"\n名称：\t{name}\n标签：\t{tag}\n类型：\t{type}\n描述: \t {GetDesc()}";
            }
        }

        public static List<Tuple<SimHashes, int, int>> elements = new List<Tuple<SimHashes, int, int>>() {
            new Tuple<SimHashes, int, int>(SimHashes.Steel, 100, 500),
            new Tuple<SimHashes, int, int>(SimHashes.SuperInsulator, 20, 300),
            new Tuple<SimHashes, int, int>(SimHashes.SuperCoolant, 100, 200),
            new Tuple<SimHashes, int, int>(SimHashes.Steel, 100, 500),
            new Tuple<SimHashes, int, int>(SimHashes.SuperInsulator, 20, 300),
            new Tuple<SimHashes, int, int>(SimHashes.SuperCoolant, 100, 200),
            new Tuple<SimHashes, int, int>(SimHashes.Steel, 100, 500),
            new Tuple<SimHashes, int, int>(SimHashes.SuperInsulator, 20, 300),
            new Tuple<SimHashes, int, int>(SimHashes.SuperCoolant, 100, 200),
            new Tuple<SimHashes, int, int>(SimHashes.Steel, 100, 500),
            new Tuple<SimHashes, int, int>(SimHashes.SuperInsulator, 20, 300),
            new Tuple<SimHashes, int, int>(SimHashes.SuperCoolant, 100, 200),
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
                marketItems.Add(new MarketItem(element.first, element.second, element.third));
            }


            foreach (Tuple<string, int> obj in objects)
            {
                marketItems.Add(new MarketItem(obj.first, obj.second));
            }

        }
    }
}
