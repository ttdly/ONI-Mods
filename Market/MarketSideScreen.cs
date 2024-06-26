﻿using System.Collections.Generic;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Market.MarketList;
using static Market.MyStrings;

namespace Market {
  public class MarketSideScreen : SideScreenContent {
    private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> APPLY_BUTTON =
      PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(applyButton));

    private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> CLEAR_BUTTON =
      PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(clearButton));

    private static readonly IDetouredField<ArtableSelectionSideScreen, GameObject> STATE_BUTTON_PREFAB =
      PDetours.DetourField<ArtableSelectionSideScreen, GameObject>(nameof(stateButtonPrefab));

    private static readonly IDetouredField<ArtableSelectionSideScreen, RectTransform> BUTTON_CONTAINER =
      PDetours.DetourField<ArtableSelectionSideScreen, RectTransform>(nameof(buttonContainer));

    public GameObject stateButtonPrefab;

    [SerializeField] private RectTransform buttonContainer;

    public TrueVendingMachineComponent vendingMachineComponent;
    public ToolTip applyTip;
    private readonly HashSet<LocText> bufferLocTexts = new HashSet<LocText>();

    private readonly List<MarketItem> marketItemsBuffer = new List<MarketItem>();
    private KButton applyButton;
    private KButton clearButton;
    private bool created;

    public MarketItem currItem;
    private MultiToggle currMultiToggle;
    private LocText currText;
    private float needConsume;

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      RefreshApplyButton();

      applyButton.onClick += delegate {
        if (needConsume == 0) return;
        vendingMachineComponent.ClearBuffer();
        foreach (var item in marketItemsBuffer) vendingMachineComponent.marketItems.Add(item);
        vendingMachineComponent.needConsume = needConsume;
        vendingMachineComponent.smi.GoTo(vendingMachineComponent.smi.sm.open);
        needConsume = 0;
        RefreshApplyButton();
        ClearBuffer();
      };

      clearButton.onClick += () => {
        if (needConsume == 0) return;
        float price = currItem.price * currItem.count;
        currItem.count = 0;
        needConsume -= price;
        currText.text = "x0";
        RefreshApplyButton();
      };
    }

    public override void SetTarget(GameObject target) {
      vendingMachineComponent = target.GetComponent<TrueVendingMachineComponent>();
      GenerateStateButtons();
      if (marketItems.Count > 0)
        foreach (var item in marketItems)
          item.count = 0;
      ClearBuffer();
      needConsume = 0;
    }

    public override string GetTitle() {
      return SIDESCREEN.TITLE;
    }

    public override bool IsValidForTarget(GameObject target) {
      return target.GetComponent<TrueVendingMachineComponent>() != null;
    }

    private void ClearBuffer() {
      if (bufferLocTexts.Count > 0) {
        foreach (var locText in bufferLocTexts) locText.text = "x0";
        bufferLocTexts.Clear();
      }

      if (marketItemsBuffer.Count > 0) marketItemsBuffer.Clear();
    }

    public void GenerateStateButtons() {
      if (created) return;
      if (marketItems.Count == 0) Init();
      Strings.Add("MARKETITEMCOUNT", "x0");
      foreach (var marketItem in marketItems) {
        var obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);
        var tooltip = "";
        if (marketItem.name != "test") tooltip += $"{marketItem.name}\n";
        tooltip += SIDESCREEN.ITEM_TIP.Replace("{Coin}", marketItem.price.ToString());
        if (marketItem.unit != 0) tooltip += SIDESCREEN.ITEM_TIP_UNIT.Replace("{Unit}", marketItem.unit.ToString());
        var component = obj.GetComponent<MultiToggle>();
        component.GetComponent<ToolTip>().SetSimpleTooltip(tooltip);
        component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = marketItem.sprite;

        var textGameObject = new GameObject("CountText");
        textGameObject.transform.SetParent(component.transform, false);
        var textComponent = textGameObject.AddComponent<LocText>();
        textComponent.key = "MARKETITEMCOUNT";
        textComponent.color = Color.white;
        textComponent.fontSize = 14;
        textComponent.fontStyle = FontStyles.Bold;
        textComponent.text = "x0";
        textComponent.alignment = TextAlignmentOptions.Bottom;
        var rectTransform = textGameObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = new Vector2(2, 2);
        rectTransform.offsetMax = new Vector2(-2, -2);

        component.onClick = delegate {
          currMultiToggle = component;
          currText = textComponent;
          currItem = marketItem;
          FindItem(marketItem, out var findObj);
          if (findObj == null) {
            marketItemsBuffer.Add(marketItem);
            bufferLocTexts.Add(textComponent);
            findObj = marketItem;
          }

          if (!CanAdd(findObj)) return;
          findObj.count++;
          needConsume += findObj.price;
          currText.text = $"x{findObj.count}";
          RefreshApplyButton();
        };
      }

      created = true;
    }

    private void FindItem(MarketItem inItem, out MarketItem outItem) {
      if (inItem.type == ObjectType.NeedPack)
        outItem = marketItemsBuffer.Find(item => item.innerTag == inItem.innerTag);
      else
        outItem = marketItemsBuffer.Find(item => item.tag == inItem.tag);
    }

    private bool CanAdd(MarketItem item) {
      if (needConsume + item.price > vendingMachineComponent.coin) return false;
      if (item.type == ObjectType.NeedPack && item.count > 0) return false;
      return true;
    }

    private void RefreshApplyButton() {
      if (applyTip == null) applyTip = applyButton.GetComponent<ToolTip>();
      if (needConsume > 0)
        applyTip.SetSimpleTooltip(
          $"{SIDESCREEN.APPLY_BUTTON_TIP_PRE}{needConsume}{SIDESCREEN.APPLY_BUTTON_TIP_POST}");
      else
        applyTip.SetSimpleTooltip(SIDESCREEN.APPLY_BUTTON_NO_ITEM);
    }

    private static MarketSideScreen CreateScreen(ArtableSelectionSideScreen sideScreen) {
      var template = Instantiate(sideScreen.gameObject);
      template.name = nameof(MarketSideScreen);
      var active = template.activeSelf;
      template.SetActive(false);
      var oldScreen = template.GetComponent<ArtableSelectionSideScreen>();
      var newScreen = template.AddComponent<MarketSideScreen>();
      newScreen.stateButtonPrefab = STATE_BUTTON_PREFAB.Get(oldScreen);
      newScreen.applyButton = APPLY_BUTTON.Get(oldScreen);
      newScreen.applyButton.GetComponent<ToolTip>().SetSimpleTooltip("消耗");
      newScreen.clearButton = CLEAR_BUTTON.Get(oldScreen);
      TryChangeText(newScreen.applyButton.gameObject.transform, "Label", SIDESCREEN.APPLY_BUTTON);
      newScreen.buttonContainer = BUTTON_CONTAINER.Get(oldScreen);
      newScreen.clearButton = CLEAR_BUTTON.Get(oldScreen);
      newScreen.clearButton.GetComponent<ToolTip>().SetSimpleTooltip(SIDESCREEN.CLEAR_BUTTON);
      newScreen.buttonContainer.gameObject.transform.TryGetComponent(out GridLayoutGroup containerLayout);
      newScreen.stateButtonPrefab.gameObject.transform.TryGetComponent(out LayoutElement stateButtonPrefabLayout);
      if (containerLayout != null) containerLayout.cellSize = new Vector2(78, 80);
      if (stateButtonPrefabLayout != null) stateButtonPrefabLayout.minHeight = 80;
      DestroyImmediate(oldScreen);
      template.SetActive(active);
      return newScreen;
    }

    public static bool TryChangeText(Transform transform, string subCompName, string newText) {
      Transform textToChangeComp;
      if (subCompName != "")
        textToChangeComp = transform.Find(subCompName);
      else
        textToChangeComp = transform;
      if (textToChangeComp == null)
        return false;
      var textToChange = textToChangeComp.gameObject.GetComponent<LocText>();
      if (textToChange == null)
        return false;
      textToChange.key = string.Empty;
      textToChange.text = newText;
      return true;
    }

    public static void AddSideScreen(IList<DetailsScreen.SideScreenRef> existing, GameObject parent) {
      var found = false;
      foreach (var ssRef in existing)
        if (ssRef.screenPrefab is ArtableSelectionSideScreen sideScreen) {
          var newScreen = new DetailsScreen.SideScreenRef();
          var ours = CreateScreen(sideScreen);
          found = true;
          newScreen.name = nameof(ModeSelectScreen);
          newScreen.screenPrefab = ours;
          newScreen.screenInstance = ours;
          var ssTransform = ours.gameObject.transform;
          ssTransform.SetParent(parent.transform);
          ssTransform.localScale = Vector3.one;
          existing.Insert(0, newScreen);
          break;
        }

      if (!found)
        PUtil.LogWarning("Unable to find side screen!");
    }
  }
}