
using System.Collections.Generic;

namespace TweaksPack.Tweakable {
    public class PlantTweakable : OnceTweakable{
        [MyCmpGet]
        Growing growing;

        protected override void OnSpawn() {
            
            base.OnSpawn();
            materialNeeds = new Dictionary<Tag, float>() {
                { SimHashes.Sulfur.CreateTag(), 10f },
                { SimHashes.Algae.CreateTag(), 50f }
            };
        }

        protected override void ToogleTweak() {
            base.ToogleTweak();
            growing.maxAge = 10f;
        }
    }
}
