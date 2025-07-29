using System.Collections.Generic;

namespace PipStore;

public enum CategoryKey {
    Solid,
    Liquid,
    Gas,
    Food,
    Seed,
    Industrial,
    Egg,
    Animal,
    Artifacts,
}

public class Category {
    public static readonly Dictionary<string, Info> Categories = new() {
        { CategoryKey.Solid.ToString(), new Info("STRINGS.ELEMENTS.STATE.SOLID", "Sand")},
        { CategoryKey.Liquid.ToString(), new Info("STRINGS.ELEMENTS.STATE.LIQUID", "Water") },
        { CategoryKey.Gas.ToString(), new Info("STRINGS.ELEMENTS.STATE.GAS", "Oxygen") },
        { CategoryKey.Food.ToString(), new Info("STRINGS.CODEX.HEADERS.DIET", MushBarConfig.ID) },
        { CategoryKey.Seed.ToString(), new Info("STRINGS.MISC.TAGS.SEED", PrickleFlowerConfig.SEED_ID) },
        { CategoryKey.Industrial.ToString(), new Info("STRINGS.UI.NEWBUILDCATEGORIES.INDUSTRIALSTATION.BUILDMENUTITLE", "BasicFabric") },
        { CategoryKey.Egg.ToString(), new Info("STRINGS.UI.SANDBOXTOOLS.FILTERS.ENTITIES.CREATURE_EGG", HatchConfig.EGG_ID) },
        { CategoryKey.Animal.ToString(), new Info("STRINGS.UI.CODEX.SUBWORLDS.CRITTERS", HatchConfig.ID) },
        { CategoryKey.Artifacts.ToString(), new Info("STRINGS.UI.SANDBOXTOOLS.FILTERS.ENTITIES.ARTIFACTS", "ui_coffee_mug") },
    };

    public class Info (string stringsKey, string spriteTag) {
        public readonly string StringsKey = stringsKey;
        public Tag SpriteTag = new (spriteTag);
    }
}