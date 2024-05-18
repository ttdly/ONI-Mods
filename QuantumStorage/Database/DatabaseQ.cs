using JetBrains.Annotations;
using KSerialization;
using PeterHan.PLib.Core;
using System.Collections.Generic;

namespace QuantumStorage.Database {
    [SerializationConfig(MemberSerialization.OptIn)]
    public class DatabaseQ : KMonoBehaviour {
        [Serialize]
        List<DatabaseItem> items;

        #region LifeCycle
        protected override void OnSpawn() {
            base.OnSpawn();
            if (items == null) { items = new List<DatabaseItem>(); }
            StaticVar.database = this;
        }
        #endregion

        #region UpdateItems
        public void UpdateItems(DatabaseItem item) {
            DatabaseItem findedItem = items.Find(find => find.elementTag == item.elementTag);
            if (findedItem.elementTag != default(DatabaseItem).elementTag) {
                findedItem.count += item.count;
            } else {
                items.Add(item);
            }
            
        }
        #endregion


        [SerializationConfig(MemberSerialization.OptIn)]
        public struct DatabaseItem {
            [Serialize]
            public Tag elementTag;
            [Serialize]
            public double count;

            public DatabaseItem(Tag tag, double count) {
                elementTag = tag;
                this.count = count;
            }

            public override string ToString() {
                return $"Tag:{elementTag}; Count:{count}";
            }
        }
    }
}
