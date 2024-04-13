using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StoreGoods {
    public class GeoActivatorSideScreen : SideScreenContent {

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
        private readonly Dictionary<int, MultiToggle> buttons = new Dictionary<int, MultiToggle>();
        private GeoActivator targetGeyserPack;
        private Tag needSpawnGeyserTag;

        protected override void OnSpawn() {
            base.OnSpawn();
            applyButton.onClick += delegate {
                if (needSpawnGeyserTag != null) {
                    targetGeyserPack.Active(needSpawnGeyserTag);
                    
                } else {
                    PlaySound(GlobalAssets.GetSound("Negative"));
                }
            };
        }

        public override void SetTarget(GameObject target) {
            targetGeyserPack = target.GetComponent<GeoActivator>();
            GenerateStateButtons();
        }

        public override string GetTitle() {
            return MyString.UI.GEO_ACTIVATOR.TITLE;
        }

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<GeoActivator>() != null;
        }

        public void GenerateStateButtons() {
            if (buttons.Count > 0) return;
            int count = 0;
            List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Geyser>();
            if (prefabsWithComponent != null) {
                foreach (GameObject go in prefabsWithComponent) {
                    if (!go.GetComponent<KPrefabID>().HasTag(GameTags.DeprecatedContent)) {
                        Tag tag = go.PrefabID();
                        string upper = tag.ToString().ToUpper();
                        upper = upper.Replace("GEYSERGENERIC_", "");
                        AddOneButton(go, count++);
                    }
                }
            }

            GameObject oilWell = Assets.GetPrefab(OilWellConfig.ID);
            AddOneButton(oilWell, count++);
        }

        private void AddOneButton(GameObject go, int count) {
            Sprite sprite = Def.GetUISprite(go.gameObject).first;
            GameObject obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, force_active: true);

                MultiToggle component = obj.GetComponent<MultiToggle>();

                component.GetComponent<ToolTip>().SetSimpleTooltip(UI.StripLinkFormatting(go.GetProperName()));
                component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = sprite;
                component.onClick = delegate {
                    if (needSpawnGeyserTag != go.PrefabID()) {
                        needSpawnGeyserTag = go.PrefabID();
                        RefreshButtons();
                        component.ChangeState(1);
                    }
                };
                buttons.Add(count, component);
        }

        private void RefreshButtons() {
            foreach (KeyValuePair<int, MultiToggle> kvp in buttons) {
                kvp.Value.gameObject.SetActive(value: true);
                kvp.Value.ChangeState(0);
            }
        }

        private static GeoActivatorSideScreen CreateScreen(ArtableSelectionSideScreen sideScreen) {
            var template = Instantiate(sideScreen.gameObject);
            template.name = nameof(GeoActivatorSideScreen);
            bool active = template.activeSelf;
            template.SetActive(false);
            var oldScreen = template.GetComponent<ArtableSelectionSideScreen>();
            var newScreen = template.AddComponent<GeoActivatorSideScreen>();
            newScreen.stateButtonPrefab = STATE_BUTTON_PREFAB.Get(oldScreen);
            newScreen.applyButton = APPLY_BUTTON.Get(oldScreen);
            //newScreen.applyButton.GetComponent<ToolTip>().SetSimpleTooltip(MyString.UI.GEO_ACTIVATOR.TOOLTIP);
            newScreen.clearButton = CLEAR_BUTTON.Get(oldScreen);
            TryChangeText(newScreen.applyButton.gameObject.transform, "Label", MyString.UI.GEO_ACTIVATOR.TEXT1);
            newScreen.buttonContainer = BUTTON_CONTAINER.Get(oldScreen);
            newScreen.clearButton = CLEAR_BUTTON.Get(oldScreen);
            newScreen.clearButton.gameObject.SetActive(false);
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

