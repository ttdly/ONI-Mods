using System;
using System.Collections.Generic;
using PipStore.Screen.Basic;
using UnityEngine;
using UnityEngine.UI;

namespace PipStore.Screen;

public class PipStoreScreen : FScreen {
    private float coin;
    private static readonly Color Pink = new (0.4980392f, 0.2392157f, 0.3686275f);
    private static readonly Color Blue = new (0.2431373f, 0.2627451f, 0.3411765f);
    private GameObject entryContainerPrefab;
    private GameObject viewPortPrefab;
    private GoodsEntry entry;
    private ConfirmScreen confirmScreen;
    private GameObject categoryButtonPrefab;
    private GameObject categoryPrefab;
    private LocText coinText;
    private ScrollRect scrollRect;
    private readonly Dictionary<string, GameObject> cateAndContent = new ();
    private Dictionary<string, List<GoodsEntry>> cateAndGoods = new ();
    private readonly Dictionary<string, Toggle> cateAndToggle = new ();
    private string firstCate = "null";
    public static PipStoreScreen Instance;
    public GoodsEntry currentGoods;
    
    public float Coin {
        set {

            coin = value;
#if DEBUG
            LogUtil.Info($"source {value}, coin {coin}");
#endif
            RefreshCoinText();
        }
        get => coin;
    }

    private void RefreshCoinText() {
        coinText.SetText(coin.ToString("0.00"));
    }

    private static void PrefabInit() {
        if (Instance != null) return;
        var screen = Util.KInstantiateUI(ModAssets.PipStoreScreenPrefab,
            PauseScreen.Instance.transform.parent.gameObject, true);
        Instance = screen.AddOrGet<PipStoreScreen>();
        Instance.WindowInit();
        Instance.GoodsInit();
    }

    public void SetTargetGoods(GoodsEntry goods) {
        currentGoods = goods;
    }

    public static void ShowWindow() {
        PrefabInit();
        Instance.Show();
        Instance.ConsumeMouseScroll = true;
        Instance.transform.SetAsLastSibling();
        Instance.RefreshCoinText();
    }

    private static void HideWindow() {
        Instance.Show(false);
    }

    public void ShowConfirmDialog() {
        confirmScreen.Show();
    }
    public void WindowInit() {
        var baseTransform = Instance.transform;
        baseTransform.Find("Main/Title/Label")
            .gameObject.GetComponent<LocText>().SetText(PipStoreString.Screen.WindowTitle);
        coinText = baseTransform.Find("Main/Info/Coin/Label").GetComponent<LocText>();
        baseTransform.Find("Main/Info/Coin/Image").GetComponent<Image>().sprite = 
            Assets.GetSprite("ui_buildingwood");
        viewPortPrefab = baseTransform.Find("Main/Goods/ScrollView/Viewport").gameObject;
        scrollRect = baseTransform.Find("Main/Goods/ScrollView").GetComponent<ScrollRect>();
        entryContainerPrefab = baseTransform.Find("Main/Goods/ScrollView/Viewport/Content").gameObject;
        entry = entryContainerPrefab.transform.Find("GoodsEntry").gameObject.AddComponent<GoodsEntry>();
        entry.goodsPriceText =
            entryContainerPrefab.transform.Find("GoodsEntry/Price").gameObject.GetComponent<LocText>();
        entry.goodsName = entryContainerPrefab.transform.Find("GoodsEntry/Name").gameObject.GetComponent<LocText>();
        entry.goodsUnit = entryContainerPrefab.transform.Find("GoodsEntry/Unit").gameObject.GetComponent<LocText>();
        entry.goodsImage = entryContainerPrefab.transform.Find("GoodsEntry/Image").GetComponent<Image>();
        entry.button = entryContainerPrefab.transform.Find("GoodsEntry").GetComponent<Button>();
        entry.gameObject.SetActive(false);
        entryContainerPrefab.SetActive(false);
        confirmScreen = baseTransform.Find("Confirm").gameObject.AddComponent<ConfirmScreen>();
        confirmScreen.fNumberInputField = baseTransform.Find("Confirm/Content/NumControl/Input")
            .gameObject.FindOrAddComponent<FNumberInputField>();
        confirmScreen.fNumberInputField.SetTextFromData("1");
        confirmScreen.fNumberInputField.minValue = 1;
        confirmScreen.confirmMsg = baseTransform.Find("Confirm/Content/Hint").gameObject.GetComponent<LocText>();
        confirmScreen.gameObject.SetActive(false);
        baseTransform.Find("Main/Title/Close")
            .gameObject.GetComponent<Button>().onClick.AddListener(HideWindow);
        categoryPrefab = baseTransform.Find("Main/Categories").gameObject;
        categoryButtonPrefab = baseTransform.Find("Main/Categories/Category").gameObject;
        categoryButtonPrefab.SetActive(false);
        foreach (var keyValuePair in Category.Categories) {
            if (firstCate == "null") {
                firstCate = keyValuePair.Key;
            }
            var newGoodsContainer = Util.KInstantiateUI(entryContainerPrefab, viewPortPrefab);
            newGoodsContainer.name = keyValuePair.Key;
            cateAndContent[keyValuePair.Key] = newGoodsContainer;
            newGoodsContainer.SetActive(false);
            var newCategoryToggle = Util.KInstantiateUI(categoryButtonPrefab, categoryPrefab);
            newCategoryToggle.name = keyValuePair.Key;
            newCategoryToggle.AddComponent<ToolTip>().toolTip = Strings.Get(keyValuePair.Value.StringsKey);
            var toggle = newCategoryToggle.GetComponent<Toggle>();
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(value => ToggleValueChange(value, toggle));
            var uiGo = Assets.GetPrefab(keyValuePair.Value.SpriteTag);
            LogUtil.Info($"{uiGo == null}");
            var sprite = (uiGo==null) ? 
                new Tuple<Sprite, Color>(Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(keyValuePair.Value.SpriteTag.ToString())),Color.white) : Def.GetUISprite(uiGo);
            LogUtil.Info(sprite.first.name);
            newCategoryToggle.transform.Find("Image").gameObject.GetComponent<Image>().sprite = sprite.first;
            newCategoryToggle.transform.Find("Image").gameObject.GetComponent<Image>().color = sprite.second;
            cateAndToggle[keyValuePair.Key] = toggle;
            newCategoryToggle.SetActive(true);
        }
        cateAndToggle[firstCate].Select();
        cateAndContent[firstCate].SetActive(true);
    }

    private static bool IsSeed(KPrefabID prefabID) {
        var go = prefabID.gameObject;
        return (go != null && go.GetComponent<PlantableSeed>() != null) ? true : false;
    }

    private static bool IsFood(KPrefabID kprefabId) {
        var idString = kprefabId.PrefabID().ToString();
        return !kprefabId.HasTag(GameTags.Egg) && EdiblesManager.GetAllFoodTypes()
                   .Find((match => match.Id == idString)) != null ||
               kprefabId.HasTag(GameTags.Dehydrated);
    }

    private static bool IsIndustrial(KPrefabID restrictions) {
        if (!Game.IsCorrectDlcActiveForCurrentSave(restrictions)) return false;
        return restrictions.HasTag(GameTags.IndustrialIngredient) || restrictions.HasTag(GameTags.IndustrialProduct) || restrictions.HasTag(GameTags.Medicine) || restrictions.HasTag(GameTags.MedicalSupplies) || restrictions.HasTag(GameTags.ChargedPortableBattery);
    }

    public void GoodsInit() {
        foreach (var kPrefabID in Assets.Prefabs) {
            if (kPrefabID == null) continue;
            // 遗迹
            if (kPrefabID.HasTag(GameTags.Solid) && kPrefabID.gameObject.GetComponent<Element>()!=null) {
                AddEntry(kPrefabID.PrefabTag, 1, CategoryKey.Solid.ToString());
                continue;
            }

            if (kPrefabID.HasTag(GameTags.Liquid)) {
                AddEntry(kPrefabID.PrefabTag, 2, CategoryKey.Liquid.ToString());
                continue;
            }

            if (kPrefabID.HasTag(GameTags.Gas)) {
                AddEntry(kPrefabID.PrefabTag, 3, CategoryKey.Gas.ToString());
                continue;
            }

            if (IsSeed(kPrefabID)) {
                AddEntry(kPrefabID.PrefabTag, 4, CategoryKey.Seed.ToString());
                continue;
            }

            if (IsFood(kPrefabID)) {
                AddEntry(kPrefabID.PrefabTag, 5, CategoryKey.Food.ToString());
                continue;
            }

            if (kPrefabID.HasTag(GameTags.Egg)) {
                AddEntry(kPrefabID.PrefabTag, 6, CategoryKey.Egg.ToString());
                continue;
            }

            if (IsIndustrial(kPrefabID)) {
                AddEntry(kPrefabID.PrefabTag, 7, CategoryKey.Industrial.ToString());
                continue;
            }

            if (kPrefabID.HasTag(GameTags.CreatureBrain)) {
                AddEntry(kPrefabID.PrefabTag, 8, CategoryKey.Animal.ToString());
            }
            
        }
    }
    
    public void AddEntry(Tag goodsTag, float price, string category="null") {
        if (!cateAndContent.TryGetValue(category, out var cateEntryContainer)) {
            LogUtil.Error($"没找到类别{category}");
            return;
        }
        var newGoodsEntry = Instantiate(entry, cateEntryContainer.transform);
        newGoodsEntry.SetInfo(goodsTag, price);
        newGoodsEntry.gameObject.SetActive(true);
    }

    private void ToggleValueChange(bool value, Toggle toggle) {
        var image = toggle.gameObject.GetComponent<Image>();
        image.color = value ? Pink : Blue;
        foreach (var kv in cateAndToggle) {
            var currContent = cateAndContent[kv.Key];
            currContent.gameObject.SetActive(kv.Value.isOn);
            if (kv.Value.isOn) {
                scrollRect.content = currContent.gameObject.GetComponent<RectTransform>();
            }
        }
    }
    
    public void DoPurchase(int num) {
        var telepad = GetCurrTelepad();
        var carePack = new CarePackageInfo(currentGoods.goodsTag.ToString(), num, null);
        carePack.Deliver(telepad.transform.position);
        Coin -= num * currentGoods.goodsPrice;
        HideWindow();
    }

    private static Telepad GetCurrTelepad() {
        if (Components.Telepads.Count == 0) return null;
        if (!DlcManager.GetActiveDLCIds().Contains("EXPANSION1_ID")) return Components.Telepads[0];
        var list = Components.Telepads.GetWorldItems(ClusterManager.Instance.activeWorldId);
        return list.Count == 0 ? Components.Telepads[0] : list[0];
    }

    public void CancelPurchase() {
        //Todo
    }
}