using System.Collections.Generic;

namespace SpaceStore.SellButtons {
    public class PriceConvter {
        public static PriceConvter Instance;
        public static bool Dirty = true;
        public Dictionary<Tag, float> sellItems = new Dictionary<Tag, float>() {
            {GameTags.Agriculture, 0.001f },
            {GameTags.Metal, 0.01f },
            {GameTags.Organics,0.01f },
            {GameTags.ManufacturedMaterial, 0.1f },
            {GameTags.RefinedMetal, 0.02f},
            {GameTags.Plastic, 0.01f},
            {GameTags.Filler, 0.01f},
            {GameTags.Glass, 0.01f},
            {GameTags.PreciousRock, 0.02f},
            {GameTags.Farmable, 0.0002f},
            {GameTags.BuildableRaw, 0.0001f},
            {GameTags.Seed, 0.05f},
        };
        
        public PriceConvter() {
            Instance = this;
        }
    }
}
