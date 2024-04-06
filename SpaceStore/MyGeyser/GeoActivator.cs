

using UnityEngine;

namespace SpaceStore.MyGeyser {
    public class GeoActivator: KMonoBehaviour{
        public void Active(Tag tag) {
            ClusterManager.Instance.GetWorld(gameObject.GetMyWorldId()).GetSMI<GameplaySeasonManager.Instance>().StartNewSeason(Db.Get().GameplaySeasons.TemporalTearMeteorShowers);
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
