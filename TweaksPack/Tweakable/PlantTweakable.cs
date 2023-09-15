
namespace TweaksPack.Tweakable 
{
    public class PlantTweakable : OnceTweakable{

        protected override void OnSpawn() {
            base.OnSpawn();
            materialNeeds = TweakableStaticVars.MaterialNeeds.Plant;
        }

        protected override void ToogleTweak() {
            base.ToogleTweak();
            gameObject.AddTag(TweakableStaticVars.Tags.AutoHarvest);
        }

    }
}
