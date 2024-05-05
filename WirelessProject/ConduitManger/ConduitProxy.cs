using KSerialization;
using UnityEngine;
using static WirelessProject.ConduitManger.StaticVar;
using STRINGS;

namespace WirelessProject.ConduitManger {
    public class ConduitProxy : KMonoBehaviour{
        private float elapsedTime;
        private float lastUpdateTime = float.NegativeInfinity;
        public ConduitProxyContentList proxyList;
        public int ThisCell;
        [Serialize]
        bool named = false;
        [MyCmpGet]
        readonly UserNameable nameable;

        #region LifeCycle
        protected override void OnSpawn() {
            base.OnSpawn();
            ThisCell = Grid.PosToCell(gameObject.transform.GetPosition());
            GlobalIdAndProxyList.TryGetValue(ThisCell, out ConduitProxyContentList proxyList);
            if (proxyList != null) {
                this.proxyList = proxyList;
                this.proxyList.proxy = this;
            } else {
                ConduitProxyContentList new_proxyList = new ConduitProxyContentList {
                    ProxyListId = ThisCell,
                    proxy = this
                };
                GlobalIdAndProxyList.Add(ThisCell, new_proxyList);
                this.proxyList = new_proxyList;
            }
            GenerateName();

            //LinkToProxyScreen.Instance?.AddCheckBox(ThisCell, gameObject.GetProperName());
            //GetComponent<KSelectable>().AddStatusItem(ProxyMaxWattageStatus, this);
            //GetComponent<KSelectable>().AddStatusItem(ProxyCircuitStatus, this);
        }

        protected override void OnCleanUp() {
            //while (proxyList.updaters.Count > 0) { 
                
            //}
            GlobalIdAndProxyList.Remove(ThisCell);
            //LinkToProxyScreen.Instance.RemoveCheckBox(ThisCell);
            base.OnCleanUp();
        }
        #endregion

        //private void ClearProxy(GameObject go) {
        //    go.GetComponent<BaseLinkToProxy>().RestoreLink();
        //}

        public void Sim200ms(float dt) {
            if (dt <= 0f) {
                return;
            }
            elapsedTime += dt;
            if (elapsedTime < 1f) {
                return;
            }
            elapsedTime -= 1f;
            float obj = 1f;
            lastUpdateTime = Time.time;
            for (int k = 0; k < proxyList.updaters.Count; k++) {
                proxyList.updaters[k].callback(obj);
            }
        }

        private void GenerateName() {
            if (named) return;
            int cell = Grid.PosToCell(gameObject);
            Quadrant[] quadrantOfCell = gameObject.GetMyWorld().GetQuadrantOfCell(cell, 2);
            string str1 = ((int)quadrantOfCell[0]).ToString();
            int num = (int)quadrantOfCell[1];
            string str2 = num.ToString();
            string str3 = str1 + str2;
            string[] strArray1 = NAMEGEN.GEYSER_IDS.IDs.ToString().Split('\n');
            string str4 = strArray1[Random.Range(0, strArray1.Length)];
            string[] strArray2 = new string[6]
            {
              UI.StripLinkFormatting(gameObject.GetProperName()),
              " ",
              str4,
              str3,
              "‑",
              null
            };
            num = Random.Range(0, 10);
            strArray2[5] = num.ToString();
            nameable.SetName(string.Concat(strArray2));
            named = true;
        }
    }
}
