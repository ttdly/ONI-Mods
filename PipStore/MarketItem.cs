using System;
using System.Security.Permissions;
using STRINGS;
using UnityEngine;

namespace PipStore;

public class MarketItem {
    public readonly string Name;
    public int Price;
    public Tuple<Sprite, Color> Sprite;
    private Tag tag;

    public MarketItem(Tag tag, int price = 9999, int count = 1) {
        var go = Assets.GetPrefab(tag) ?? throw new Exception($"Tag {tag} not exist");
        this.tag = tag;
        Sprite = Def.GetUISprite(tag);
        Price = price;
        Name = go.GetProperName();
    }
    
    public string GetSpawnableQuantityOnly() {
        if (ElementLoader.GetElement(tag) != null)
            return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY,
                GameUtil.GetFormattedMass(1));
        return EdiblesManager.GetFoodInfo(tag.ToString()) != null
            ? string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY,
                GameUtil.GetFormattedCaloriesForItem(tag, 1))
            : string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, 1);
    }

    public override string ToString() {
        return $"\n名称：\t{Name}\n标签：\t{tag}\n描述: \t {GetSpawnableQuantityOnly()}";
    }
}