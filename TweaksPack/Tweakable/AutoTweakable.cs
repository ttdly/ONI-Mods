
namespace TweaksPack.Tweakable 
{
    public class AutoTweakable : OnceTweakable{
        protected override void OnSpawn() {
            base.OnSpawn();
            materialNeeds = TweakableStaticVars.MaterialNeeds.Auto;
        }

        protected override void ToogleTweak() {
            base.ToogleTweak();
            if (isTweaked) {
                gameObject.AddTag(TweakableStaticVars.Tags.AutoTweaked);
            }
        }

        protected override void OnFetchComplete() {
            base.OnFetchComplete();
            SetWorkTime(TweakableStaticVars.WorkTime.Auto);
        }
    }
}
