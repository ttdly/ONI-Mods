using UnityEngine;
using UnityEngine.UI;

namespace PipStore.Screen;

public class PipStoreScreen : FScreen {
    public static PipStoreScreen Instance;
    public GameObject entryContainer;
    public GoodsEntry entry;
    public ConfirmScreen confirmScreen;
    public string currItemTag;
    public GoodsEntry currentGoods;

    private static void PrefabInit() {
        if (Instance != null) return;
        var screen = Util.KInstantiateUI(ModAssets.PipStoreScreenPrefab,
            PauseScreen.Instance.transform.parent.gameObject, true);
        Instance = screen.AddOrGet<PipStoreScreen>();
        Instance.Init();
    }
    public void SetTargetGoods(GoodsEntry goods) {
        currentGoods = goods;
    }
    public static void ShowWindow() {
        PrefabInit();
        Instance.Show();
        Instance.ConsumeMouseScroll = true;
        Instance.transform.SetAsLastSibling();
    }
    private void HideWindow() {
        Instance.Show(false);
    }
    public void ShowConfirmDialog() {
        confirmScreen.gameObject.SetActive(true);
    }
    public void Init() {
        var baseTransform = Instance.transform;
        baseTransform.Find("Main/Head/Title")
            .gameObject.GetComponent<LocText>().SetText(PipStoreString.Screen.WindowTitle);
        baseTransform.Find("Main/Content/Top/ModeToggleContainer/Puechase/Label")
            .gameObject.GetComponent<LocText>().SetText(PipStoreString.Screen.Buy);
        baseTransform.Find("Main/Content/Top/ModeToggleContainer/Sell/Label")
            .gameObject.GetComponent<LocText>().SetText(PipStoreString.Screen.Sell);
        
        entryContainer = baseTransform.Find("Main/Content/Middle/ScrollView/Viewport/Content").gameObject;
        entry = entryContainer.transform.Find("GoodsEntry").gameObject.AddComponent<GoodsEntry>();
        entry.goodsPriceText = entryContainer.transform.Find("GoodsEntry/Price/Text").gameObject.GetComponent<LocText>();
        entry.goodsName = entryContainer.transform.Find("GoodsEntry/Title").gameObject.GetComponent<LocText>();
        entry.goodsUnit = entryContainer.transform.Find("GoodsEntry/Unit").gameObject.GetComponent<LocText>();
        entry.goodsImage = entryContainer.transform.Find("GoodsEntry/Image").GetComponent<Image>();
        entry.button = entryContainer.transform.Find("GoodsEntry/Price").GetComponent<Button>();
        entry.gameObject.SetActive(false);

        confirmScreen = baseTransform.Find("Confirm").gameObject.AddComponent<ConfirmScreen>();
        // confirmScreen.confirmMsg = baseTransform.Find("Confirm/Msg").gameObject.GetComponent<LocText>();
        confirmScreen.fNumberInputField = baseTransform
            .Find("Confirm/NumControl/InputContent/Input")
            .gameObject.FindOrAddComponent<FNumberInputField>();
        confirmScreen.fNumberInputField.SetTextFromData("1");
        confirmScreen.fNumberInputField.minValue = 1;
        confirmScreen.gameObject.SetActive(false);

        baseTransform.Find("Main/Head/Close")
            .gameObject.GetComponent<Button>().onClick.AddListener(HideWindow);
  
        AddEntry(new Tag("IgneousRock"), 20);
    }

    public void AddEntry(Tag goodsTag, float price) {
        var entryInstance = Instantiate(entry, entryContainer.transform);
        entryInstance.SetInfo(goodsTag, price);
        entryInstance.gameObject.SetActive(true);
    }

    public void DoPurchase(int num) {
        var telepad = GetCurrTelepad();
        var carePack = new CarePackageInfo(currentGoods.goodsTag.ToString(), num, null);
        carePack.Deliver(telepad.transform.position);
        HideWindow();
    }
    
    private static Telepad GetCurrTelepad() {
        if (Components.Telepads.Count == 0) return null;
        if (!DlcManager.GetActiveDLCIds().Contains("EXPANSION1_ID")) return Components.Telepads[0];
        var list = Components.Telepads.GetWorldItems(ClusterManager.Instance.activeWorldId);
        return list.Count == 0 ? Components.Telepads[0] : list[0];
    }
    public void CancelPurchase() {}
}