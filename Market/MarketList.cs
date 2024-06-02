using System.Collections.Generic;
using Market.ItemSpwan;
using STRINGS;
using UnityEngine;

namespace Market {
  public class MarketList {
    public enum ObjectType {
      Object,
      Element,
      NeedPack
    }

    public static List<MarketItem> marketItems = new List<MarketItem>();

    public static List<Tuple<SimHashes, int, int>> elements = new List<Tuple<SimHashes, int, int>> {
      new Tuple<SimHashes, int, int>(SimHashes.Steel, 10, 500),
      new Tuple<SimHashes, int, int>(SimHashes.SuperInsulator, 2, 300),
      new Tuple<SimHashes, int, int>(SimHashes.SuperCoolant, 1, 200)
    };

    public static List<Tuple<string, int>> objects = new List<Tuple<string, int>> {
      new Tuple<string, int>(GeneShufflerRechargeConfig.ID, 400)
    };

    public static List<Tuple<string, int>> packs = new List<Tuple<string, int>> {
      new Tuple<string, int>(OilWellConfig.ID, 500)
    };

    public static void Init() {
      foreach (var element in elements)
        marketItems.Add(new MarketItem(element.first.CreateTag(), element.first, element.second, element.third));


      foreach (var obj in objects) marketItems.Add(new MarketItem(obj.first, obj.second));

      var prefabsWithComponent = Assets.GetPrefabsWithComponent<Geyser>();
      if (prefabsWithComponent != null)
        foreach (var go in prefabsWithComponent)
          if (!go.GetComponent<KPrefabID>().HasTag(GameTags.DeprecatedContent)) {
            var tag = go.PrefabID();
            var upper = tag.ToString().ToUpper();
            upper = upper.Replace("GEYSERGENERIC_", "");
            marketItems.Add(new MarketItem(Strings.Get("STRINGS.CREATURES.SPECIES.GEYSER." + upper + ".NAME"), tag,
              1000));
          }

      prefabsWithComponent.Clear();

      foreach (var pack in packs)
        marketItems.Add(new MarketItem(Assets.GetPrefab(pack.first).GetProperName(), pack.first, pack.second));
    }

    public class MarketItem {
      public int count = 0;
      public Tag innerTag;
      public string name = "test";
      public int price;
      public SimHashes simHashes;
      public Sprite sprite;
      public Tag tag;
      public ObjectType type;
      public int unit;

      // 初始化元素商品
      public MarketItem
        (Tag tag, SimHashes simHashes = 0, int unit = 0, int price = 10, ObjectType type = ObjectType.Element) {
        this.tag = tag;
        this.price = price;
        this.type = type;
        this.simHashes = simHashes;
        this.unit = unit;
        sprite = Def.GetUISprite(tag).first;
        name = UI.StripLinkFormatting(Strings.Get("STRINGS.ELEMENTS." + tag.ToString().ToUpper() + ".NAME"));
      }

      public MarketItem(string name, Tag innerTag, int price = 10, ObjectType type = ObjectType.NeedPack) {
        tag = (Tag)ItemSpawnPackConfig.ID;
        sprite = Def.GetUISprite(innerTag).first;
        this.innerTag = innerTag;
        this.price = price;
        this.type = type;
        this.name = UI.StripLinkFormatting(name);
      }

      public MarketItem(Tag tag, int price = 10, ObjectType type = ObjectType.Object) {
        this.tag = tag;
        sprite = Def.GetUISprite(tag).first;
        this.price = price;
        this.type = type;
      }

      public override string ToString() {
        return $"\n名称：\t{name}\n标签：\t{tag}\n价格：\t{price}\n数量：\t{count}\n单位：\t{unit}\n类型：\t{type}\n内部：\t{innerTag}";
      }
    }
  }
}