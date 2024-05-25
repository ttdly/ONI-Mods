using KSerialization;
using PeterHan.PLib.Core;
using System.Collections.Generic;

namespace QuantumStorage.Database {
    [SerializationConfig(MemberSerialization.OptIn)]
    public class DatabaseQ : KMonoBehaviour {
        [Serialize]
        public Dictionary<Tag, double> itemDic = new Dictionary<Tag, double>();
        [Serialize]
        public float tempHeat;
        private readonly float maxTemp = 50000f;
        [MyCmpGet]
        public Operational operational;

        #region LifeCycle
        protected override void OnSpawn() {
            base.OnSpawn(); 
            StaticVar.database = this;
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            StaticVar.database = null;
        }
        #endregion

        #region UpdateItems
        public void UpdateItems(Tag tag, double amount, float transferHeat) {
            PUtil.LogDebug($"{tag}:{amount}:{transferHeat}");
            if (itemDic.ContainsKey(tag)) {
                double buffer = itemDic[tag] + amount;
                itemDic[tag] = buffer;
            } else {
                itemDic.Add(tag, amount);
            }
            operational.SetActive(operational.IsOperational);
            ApplyHeat(transferHeat);
        }

        public void ApplyHeat(float heat) {
            tempHeat += heat;
            if (tempHeat > maxTemp) { 
                float count = tempHeat - maxTemp;
                SimMessages.ModifyEnergy(Grid.PosToCell(gameObject), count, 10000f, SimMessages.EnergySourceID.Overheatable);
            }

        }
        #endregion
    }
}
