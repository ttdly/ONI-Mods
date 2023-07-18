namespace MoveGeysers {
    internal class Controller : KMonoBehaviour, ISim1000ms {
        public void Sim1000ms(float dt) {
            if (gameObject.GetComponent<BuildingAttachPoint>().points[0].attachedBuilding != null) {
                gameObject.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
                Destroy(gameObject.GetComponent<Movable>());
            } else {
                if (gameObject.GetComponent<Movable>() == null) gameObject.AddComponent<Movable>();
            }
        }
    }
}
