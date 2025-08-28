using System.Collections.Generic;
using UnityEngine;

namespace PipStore;

public class ResourcesCounter : KMonoBehaviour, ISaveLoadable {
    public static ResourcesCounter Instance;
    private readonly Dictionary<Tag, float> resources = new();
    
    protected override void OnPrefabInit() {
        base.OnPrefabInit();
        Instance = this;
        Subscribe(Game.Instance.gameObject, -1588644844, OnAddedFetchable);
        Subscribe(Game.Instance.gameObject, -1491270284, OnRemovedFetchable);
    }

    protected override void OnCleanUp() {
        Unsubscribe(Game.Instance.gameObject, -1588644844, OnAddedFetchable);
        Unsubscribe(Game.Instance.gameObject, -1491270284, OnRemovedFetchable);
        base.OnCleanUp();
    }

    private static Tuple<Tag, float> ConvertData(object data) {
        var go = (GameObject)data;
        var prefabId = go.GetComponent<KPrefabID>();
        var pickup = go.GetComponent<Pickupable>();
        var prefabTag = prefabId.PrefabID();
        return new Tuple<Tag, float>(prefabTag, pickup.TotalAmount);
    }
    
    private void AddAmountToResources(object data, bool isSub = false) {
        var tagAndAmount = ConvertData(data);
        var targetTag = tagAndAmount.first;
        var amount = tagAndAmount.second;
        if (isSub) {
            amount = -amount;
        }
        LogUtil.Info("添加资源：", targetTag, "数量：", amount, "是否负数：", isSub);
        if (resources.ContainsKey(targetTag)) {
            resources[targetTag] += amount;
            return;
        }

        if (isSub) {
            LogUtil.Warning("资源添加异常：", targetTag);
        }
        resources.Add(targetTag, amount);
    }


    private void OnAddedFetchable(object data) {
        AddAmountToResources(data);
    }


    private void OnRemovedFetchable(object data) {
        AddAmountToResources(data, true);
    }
}