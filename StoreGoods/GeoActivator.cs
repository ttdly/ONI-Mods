namespace StoreGoods {
  public class GeoActivator : KMonoBehaviour {
    public void Active(Tag tag) {
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