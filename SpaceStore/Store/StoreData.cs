using KSerialization;
using PeterHan.PLib.Core;
using System.Runtime.Serialization;

namespace SpaceStore.Store
{
    [SerializationConfig(MemberSerialization.OptIn)]
    internal class StoreData : KMonoBehaviour, ISaveLoadable
    {
        [Serialize]
        public float coin = 0;

        [OnSerializing]
        internal void OnSerializing()
        {
            coin = StaticVars.Coin;
        }
        [OnDeserialized]
        internal void OnDeserialized()
        {
            StaticVars.Coin = coin;
#if DEBUG
            PUtil.LogDebug("数据已经加载，现在金币："+coin.ToString());
#endif
        }
    }
}
