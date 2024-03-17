using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SpaceStore.Store.StoreList;

namespace SpaceStore.Store {
    public class StoreSideScreen : SideScreenContent {
        private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> APPLY_BUTTON =
            PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(applyButton));
        private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> CLEAR_BUTTON =
            PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(clearButton));
        private static readonly IDetouredField<ArtableSelectionSideScreen, GameObject> STATE_BUTTON_PREFAB =
            PDetours.DetourField<ArtableSelectionSideScreen, GameObject>(nameof(stateButtonPrefab));
        private static readonly IDetouredField<ArtableSelectionSideScreen, RectTransform> BUTTON_CONTAINER =
            PDetours.DetourField<ArtableSelectionSideScreen, RectTransform>(nameof(buttonContainer));
        private KButton applyButton;
        private KButton clearButton;
        public GameObject stateButtonPrefab;
        [SerializeField]
        private RectTransform buttonContainer;

        private readonly List<MarketItem> marketItemsBuffer = new List<MarketItem>();
        private readonly HashSet<LocText> bufferLocTexts = new HashSet<LocText>();

        public SpaceStoreComponent targetStore;
        private MultiToggle currMultiToggle;
        private LocText currText;
        private float needConsume = 0;
        private bool created = false;
        public ToolTip applyTip;

        public MarketItem currItem;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
        }

        protected override void OnSpawn() {
            base.OnSpawn();
            RefreshApplyButton();
            
            applyButton.onClick += delegate {
                if (needConsume == 0) return;
                targetStore.ClearBuffer();
                foreach (MarketItem item in marketItemsBuffer) {
                    targetStore.marketItems.Add(item);
                }
                targetStore.needConsume = needConsume;
                targetStore.smi.GoTo(targetStore.smi.sm.open);
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
            targetStore = target.GetComponent<SpaceStoreComponent>();
            GenerateStateButtons();
            if (marketItems.Count > 0) {
                foreach (MarketItem item in marketItems) {
                    item.count = 0;
                }
            }
            ClearBuffer();
            needConsume = 0;
        }

        public override string GetTitle() {
            return MyString.UI.STORE.SIDESCREEN.TITLE;
        }

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<SpaceStoreComponent>() != null;
        }

        private void ClearBuffer() {
            if (bufferLocTexts.Count > 0) {
                foreach (LocText locText in bufferLocTexts) {
                    locText.text = "x0";
                }
                bufferLocTexts.Clear();
            }
            if (marketItemsBuffer.Count > 0) {
                marketItemsBuffer.Clear();
            }
        }

        public void GenerateStateButtons() {
            if (created) return;
            if (marketItems.Count == 0) Init();
            Strings.Add("MARKETITEMCOUNT", "x0");
            foreach (MarketItem marketItem in marketItems) {
                GameObject obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, force_active: true);
                string tooltip = "";
                if (marketItem.name != "test") {
                    tooltip += $"{marketItem.name}\n";
                }
                tooltip += MyString.UI.STORE.SIDESCREEN.ITEM_TIP.Replace("{Coin}", marketItem.price.ToString());
                if (marketItem.unit != 0) {
                    tooltip += MyString.UI.STORE.SIDESCREEN.ITEM_TIP_UNIT.Replace("{Unit}", marketItem.unit.ToString());
                }
                MultiToggle component = obj.GetComponent<MultiToggle>();
                component.GetComponent<ToolTip>().SetSimpleTooltip(tooltip);
                component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = marketItem.sprite;

                GameObject textGameObject = new GameObject("CountText");
                textGameObject.transform.SetParent(component.transform, false);
                LocText textComponent = textGameObject.AddComponent<LocText>();
                textComponent.key = "MARKETITEMCOUNT";
                textComponent.color = Color.white;
                textComponent.fontSize = 14;
                textComponent.fontStyle = TMPro.FontStyles.Bold;
                textComponent.text = "x0";
                textComponent.alignment = TMPro.TextAlignmentOptions.Bottom;
                RectTransform rectTransform = textGameObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.offsetMin = new Vector2(2, 2);
                rectTransform.offsetMax = new Vector2(-2, -2);

                component.onClick = delegate {
                    currMultiToggle = component;
                    currText = textComponent;
                    currItem = marketItem;
                    FindItem(marketItem, out MarketItem findObj);
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

        private void FindItem(MarketItem inItem,out MarketItem outItem) {
            if (inItem.type == ObjectType.NeedPack) {
                outItem = marketItemsBuffer.Find(item => item.innerTag == inItem.innerTag);
            } else {
                outItem = marketItemsBuffer.Find(item => item.tag == inItem.tag);
            }
        }

        private bool CanAdd(MarketItem item) {
            if ((needConsume + item.price) > targetStore.coin){
                return false;
            }
            if (item.type == ObjectType.NeedPack && item.count > 0) {
                return false;
            }
            return true;
        }

        private void RefreshApplyButton() {
            if (applyTip == null) {
                applyTip = applyButton.GetComponent<ToolTip>();
            }
            if (needConsume > 0) {
                applyTip.SetSimpleTooltip(
                $"{MyString.UI.STORE.SIDESCREEN.APPLY_BUTTON_TIP_PRE}{needConsume}{MyString.UI.STORE.SIDESCREEN.APPLY_BUTTON_TIP_POST}");
            } else {
                applyTip.SetSimpleTooltip(MyString.UI.STORE.SIDESCREEN.APPLY_BUTTON_NO_ITEM);
            }
        }

        private static StoreSideScreen CreateScreen(ArtableSelectionSideScreen sideScreen) {
            var template = Instantiate(sideScreen.gameObject);
            template.name = nameof(StoreSideScreen);
            bool active = template.activeSelf;
            template.SetActive(false);
            var oldScreen = template.GetComponent<ArtableSelectionSideScreen>();
            var newScreen = template.AddComponent<StoreSideScreen>();
            newScreen.stateButtonPrefab = STATE_BUTTON_PREFAB.Get(oldScreen);
            newScreen.applyButton = APPLY_BUTTON.Get(oldScreen);
            newScreen.applyButton.GetComponent<ToolTip>().SetSimpleTooltip("消耗");
            newScreen.clearButton = CLEAR_BUTTON.Get(oldScreen);
            TryChangeText(newScreen.applyButton.gameObject.transform, "Label", MyString.UI.STORE.SIDESCREEN.APPLY_BUTTON);
            newScreen.buttonContainer = BUTTON_CONTAINER.Get(oldScreen);
            newScreen.clearButton = CLEAR_BUTTON.Get(oldScreen);
            newScreen.clearButton.GetComponent<ToolTip>().SetSimpleTooltip(MyString.UI.STORE.SIDESCREEN.CLEAR_BUTTON);
            newScreen.buttonContainer.gameObject.transform.TryGetComponent(out GridLayoutGroup containerLayout);
            newScreen.stateButtonPrefab.gameObject.transform.TryGetComponent(out LayoutElement stateButtonPrefabLayout);
            if (containerLayout != null) {
                containerLayout.cellSize = new Vector2(78, 80);
            }
            if (stateButtonPrefabLayout != null) {
                stateButtonPrefabLayout.minHeight = 80;
            }
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
            bool found = false;
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
