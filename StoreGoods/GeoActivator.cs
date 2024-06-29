namespace StoreGoods {
  public class GeoActivator : KMonoBehaviour {
    public void Active(Tag tag) {
      if (DlcManager.IsDlcId("EXPANSION1_ID"))
        ClusterManager.Instance.GetWorld(gameObject.GetMyWorldId()).GetSMI<GameplaySeasonManager.Instance>()
          .StartNewSeason(Db.Get().GameplaySeasons.SpacedOutStyleWarpMeteorShowers);
      var go = GameUtil.KInstantiate(Assets.GetPrefab(tag), Grid.SceneLayer.Building);
      go.SetActive(false);
      var posCbc = gameObject.transform.position;
      var num = -0.15f;
      posCbc.z += num;
      go.transform.SetPosition(posCbc);
      go.SetActive(true);
      gameObject.DeleteObject();
    }
  }
}