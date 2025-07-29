using System;
using STRINGS;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PipStore.Screen {
    public class GoodsEntry : KScreen {
        private Image coinIcon;
        public Button button;
        public Tag goodsTag;
        public float goodsPrice;
        public LocText goodsName;
        public LocText goodsUnit;
        public LocText goodsPriceText;
        public Image goodsImage;
        public string goodsProperName;
        
        protected override void OnSpawn() {
            base.OnSpawn();
            // LogUtil.Info($"BUTTON {button == null}");
            button.onClick.AddListener(OpenConfirmDialog);
            
        }
        private void OpenConfirmDialog() {
            PipStoreScreen.Instance.SetTargetGoods(this);
            PipStoreScreen.Instance.ShowConfirmDialog();
        }
        public void SetInfo(Tag thisTag, float price) {
            goodsTag = thisTag;
            goodsPrice = price;
            var go = Assets.GetPrefab(goodsTag);
            if (go == null) {
                LogUtil.Warning($"prefab {goodsTag} not found");
                Destroy(gameObject);
            }
            var sprite = Def.GetUISprite(goodsTag);
            goodsProperName = go.GetProperName();
            goodsName.SetText(goodsProperName);
            goodsUnit.SetText(GetSpawnableQuantityOnly());
            goodsPriceText.SetText(goodsPrice.ToString("0.00"));
            goodsImage.sprite = sprite.first;
            goodsImage.color = sprite.second;
            goodsImage.preserveAspect = true;
        }
        
        public string GetSpawnableQuantityOnly() {
            if (ElementLoader.GetElement(goodsTag) != null)
                return string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY,
                    GameUtil.GetFormattedMass(1));
            return EdiblesManager.GetFoodInfo(goodsTag.ToString()) != null
                ? string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY,
                    GameUtil.GetFormattedCaloriesForItem(goodsTag, 1))
                : string.Format(UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, 1);
        }
    }
}