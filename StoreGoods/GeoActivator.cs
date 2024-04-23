using UnityEngine;

namespace StoreGoods {
    public class GeoActivator: KMonoBehaviour{
        public void Active(Tag tag) {
            if (DlcManager.IsContentActive("EXPANSION1_ID")) {
                ClusterManager.Instance.GetWorld(gameObject.GetMyWorldId()).GetSMI<GameplaySeasonManager.Instance>().StartNewSeason(Db.Get().GameplaySeasons.SpacedOutStyleWarpMeteorShowers);
            }
            GameObject go = GameUtil.KInstantiate(Assets.GetPrefab(tag), Grid.SceneLayer.Building);
            go.SetActive(false);
            Vector3 posCbc = gameObject.transform.position;
            float num = -0.15f;
            posCbc.z += num;
            go.transform.SetPosition(posCbc);
            go.SetActive(true);
            gameObject.DeleteObject();
        }
    }
}
