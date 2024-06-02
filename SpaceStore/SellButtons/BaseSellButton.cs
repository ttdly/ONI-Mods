using SpaceStore.Store;
using UnityEngine;

namespace SpaceStore.SellButtons {
  public class BaseSellButton : KMonoBehaviour {
    private static readonly EventSystem.IntraObjectHandler<BaseSellButton> OnRefreshUserMenuDelegate =
      new EventSystem.IntraObjectHandler<BaseSellButton>((component, data) => component.OnRefreshUserMenu(data));

    public float coin;
    public bool canSell = true;

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
    }

    protected virtual void OnRefreshUserMenu(object _) {
      if (!canSell) return;
      if (coin == 0) CountPrice();
      if (coin < 0) return;
      Game.Instance.userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo("action_move_to_storage",
        MyString.UI.SELL.TITLE, Sell,
        tooltipText: MyString.UI.SELL.TOOL_TIP.Replace("{coin}", string.Format("{0:0.00}", coin))
          .Replace("{item}", gameObject.GetProperName())));
    }

    public virtual void Sell() {
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource,
        $"{MyString.UI.COIN_NAME} {coin}", gameObject.transform, Vector3.zero);
      StoreDialog.RefreshCoin();
    }

    public virtual void CountPrice() {
    }
  }
}