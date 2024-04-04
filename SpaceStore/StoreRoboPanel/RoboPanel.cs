using KSerialization;
using System;
using UnityEngine;

namespace SpaceStore.StoreRoboPanel {
    public class RoboPanel : KMonoBehaviour {
        [Serialize]
        public bool canAuto = false;
        [Serialize]
        public bool isComplex = false;
        public SuperProductiveFX.Instance fx;

        protected override void OnSpawn() {
            base.OnSpawn();
            ToogleAuto();
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            fx.sm.destroyFX.Trigger(fx);
        }

        private void AddFx() {
            fx = new SuperProductiveFX.Instance(gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, Grid.GetLayerZ(Grid.SceneLayer.FXFront)));
            fx.StartSM();
        }

        public void MakeComplexAuto() {
            if (gameObject.GetComponent<ComplexFabricator>() != null) { gameObject.GetComponent<ComplexFabricator>().duplicantOperated = false; }
        }

        public void Addtag() {
            gameObject.AddTag(StaticVars.AutoTag);
        }

        public void ToogleAuto() {
            if (!canAuto) return;
            Addtag();
            if (isComplex) {
                MakeComplexAuto();
            }
            AddFx();
        }

    }
}
