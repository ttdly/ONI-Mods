using PeterHan.PLib.Core;
using System;

namespace MoreTiles {
    internal class FarmTileLightable : KMonoBehaviour {
        [MyCmpReq]
        private readonly PlantablePlot plantablePlot;
        [MyCmpReq]
        private readonly Light2D light2D;
        [MyCmpReq]
        private readonly Operational operational;
        private IEnergyConsumer energy;
        private static readonly EventSystem.IntraObjectHandler<FarmTileLightable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<FarmTileLightable>((component, data) => component.OnStorage(data));

        private static readonly EventSystem.IntraObjectHandler<FarmTileLightable> OnconDelegate = new EventSystem.IntraObjectHandler<FarmTileLightable>((component, data) => component.OnConn(data));

        private void OnConn(object data) {
            PUtil.LogDebug(data);
        }

        private void OnStorage(object _) {
            bool light = CanLight();
            if (light != light2D.enabled){
                light2D.enabled = light;
            }
            if (light != operational.IsActive) {
                operational.SetActive(light);
            }
        }

        private bool CanLight() {
            if (plantablePlot.plant == null) return false;
            if (plantablePlot.plant?.name != "PrickleFlower") return false;
            if (!energy.IsConnected || !energy.IsPowered) return false;
            return true;
        }

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            energy = GetComponent<IEnergyConsumer>();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            Subscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
            Subscribe((int)GameHashes.ConnectWiring, OnconDelegate);
        }
    }
}
