using System.Runtime.Serialization;
using KSerialization;
using PipStore.Screen;
using UnityEngine.Serialization;

namespace PipStore;

public class SerializeData:KMonoBehaviour {
    [Serialize] public float coinNum;

    [OnSerializing]
    internal void OnSerializing() {
        coinNum = PipStoreScreen.Instance.Coin;
    }

    [OnDeserialized]
    internal void OnDeserialized() {
        PipStoreScreen.Instance.Coin = coinNum;
#if DEBUG
        LogUtil.Info("数据已经加载，现在金币："+coinNum.ToString("0.000"));
        PipStoreScreen.Instance.Coin = 10000;
#endif
    }
}