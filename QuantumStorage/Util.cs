using QuantumStorage.Uploads;
using UnityEngine;

namespace QuantumStorage {
    public class Util {
        public static void ConfigConduitUpload(GameObject go) {
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.capacityKg = 200f;
            go.AddOrGet<UploadState>();
            go.AddOrGet<ConduitConsumer>().forceAlwaysSatisfied = true;
        }
    }
}
