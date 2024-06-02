namespace TweaksPack.Tweakable {
  public class AutoTweakable : OnceTweakable {
    protected override void OnSpawn() {
      base.OnSpawn();
      SetWorkTime(TweakableStaticVars.WorkTime.Auto);
    }

    protected override void ToogleTweak() {
      base.ToogleTweak();
      if (isTweaked) gameObject.AddTag(TweakableStaticVars.Tags.AutoTweaked);
    }
  }
}