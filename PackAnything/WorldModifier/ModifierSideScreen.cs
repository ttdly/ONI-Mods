using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using System.Collections.Generic;
using UnityEngine;
using STRINGS;
using UnityEngine.UI;
using System;

namespace PackAnything {

    public class ModifierSideScreen : SideScreenContent {

        private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> APPLY_BUTTON = PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(applyButton));
        private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> CLEAR_BUTTON = PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(clearButton));
        private static readonly IDetouredField<ArtableSelectionSideScreen, GameObject> STATE_BUTTON_PREFAB = PDetours.DetourField<ArtableSelectionSideScreen, GameObject>(nameof(stateButtonPrefab));
        private static readonly IDetouredField<ArtableSelectionSideScreen, RectTransform> BUTTON_CONTAINER = PDetours.DetourField<ArtableSelectionSideScreen, RectTransform>(nameof(buttonContainer));
        
        private KButton applyButton;
        private KButton clearButton;
        public GameObject stateButtonPrefab;
        [SerializeField]
        private RectTransform scrollTransoform;
        [SerializeField]
        private RectTransform buttonContainer;
        private Dictionary<int, MultiToggle> buttons = new Dictionary<int, MultiToggle>();
        private WorldModifier targetBuilding;
        private Surveyable targetSurveyable;
        private int currCount;

        protected override void OnSpawn() {
            base.OnSpawn();
            applyButton.onClick += delegate {
                PackAnythingStaticVars.targetSurveyable = targetSurveyable;
                ActiveMoveTool(targetSurveyable);
            };
        }

        public override void SetTarget(GameObject target) {
            targetBuilding = target.GetComponent<WorldModifier>();
            GenerateStateButtons();
        }

        public override string GetTitle() {
            return PackAnythingString.UI.SIDE_SCREEN.NAME;
        }

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<WorldModifier>() != null;
        }

        public void GenerateStateButtons() {
            foreach (KeyValuePair<int, MultiToggle> button in buttons) {
                Util.KDestroyGameObject(button.Value.gameObject);
            }
            int count = 0;
            buttons.Clear();
            foreach(Surveyable surveyable in PackAnythingStaticVars.SurveableCmps) {
                if(surveyable != null) {
                    GameObject obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, force_active: true);
                    Sprite sprite = Def.GetUISprite(surveyable.gameObject).first;
                    MultiToggle component = obj.GetComponent<MultiToggle>();
                    component.GetComponent<ToolTip>().SetSimpleTooltip(UI.StripLinkFormatting(surveyable.GetProperName()));
                    component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = sprite;
                    component.onClick = delegate {
                        if (PackAnythingStaticVars.targetSurveyable != surveyable) {
                            targetSurveyable = surveyable;
                            RefreshButtons();
                            component.ChangeState(1);
                        }
                    };
                    component.onDoubleClick = delegate {
                        if (surveyable != null) {
                            CameraController.Instance.CameraGoTo(surveyable.transform.GetPosition());
                            return true;
                        }
                        return false;
                    };
                    buttons.Add(count++, component);
                }
            }
        }

        private void RefreshButtons() {
            foreach (KeyValuePair<int, MultiToggle> kvp in buttons) {
                kvp.Value.gameObject.SetActive(value: true);
                kvp.Value.ChangeState(0);
            }
        }

        private static ModifierSideScreen CreateScreen(ArtableSelectionSideScreen sideScreen) {
            var template = Instantiate(sideScreen.gameObject);
            template.name = nameof(ModifierSideScreen);
            bool active = template.activeSelf;
            template.SetActive(false);
            var oldScreen = template.GetComponent<ArtableSelectionSideScreen>();
            var ours = template.AddComponent<ModifierSideScreen>();
            ours.stateButtonPrefab = STATE_BUTTON_PREFAB.Get(oldScreen);
            ours.applyButton = APPLY_BUTTON.Get(oldScreen);
            ours.applyButton.GetComponent<ToolTip>().SetSimpleTooltip(PackAnythingString.UI.SIDE_SCREEN.APPLY_BUTTON_TOOL_TIP);
            ours.clearButton = CLEAR_BUTTON.Get(oldScreen);
            TryChangeText(ours.applyButton.gameObject.transform,"Label", PackAnythingString.UI.SIDE_SCREEN.APPLY_BUTTON_TEXT);
            ours.buttonContainer = BUTTON_CONTAINER.Get(oldScreen);
            ours.clearButton = CLEAR_BUTTON.Get(oldScreen);
            ours.clearButton.gameObject.SetActive(false);
            DestroyImmediate(oldScreen);
            template.SetActive(active);
            return ours;
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
                    newScreen.name = nameof(WorldModifierSideScreen);
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

        private void ActiveMoveTool(Surveyable surveyable) {
            MoveTargetTool.Instance.Acitvate(surveyable);
        }

    }

}
