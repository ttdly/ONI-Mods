using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using System.Collections.Generic;
using UnityEngine;
using STRINGS;
using UnityEngine.UI;

namespace PackAnything {

    public class ModifierSideScreen : SideScreenContent {

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
        private WorldModifier targetBuilding;
        private Surveyable targetSurveyable;

        protected override void OnSpawn() {
            base.OnSpawn();
            applyButton.onClick += delegate {
                if (targetSurveyable != null) {
                    PackAnythingStaticVars.SetTargetSurveyable(targetSurveyable);
                    PackAnythingStaticVars.SetTargetModifier(targetBuilding);
                    ActiveMoveTool(targetSurveyable);
                    targetSurveyable = null;
                } else {
                    PlaySound(GlobalAssets.GetSound("Negative"));
                }
            };
            clearButton.onClick += () => {
                if (targetSurveyable != null) {
                    targetSurveyable.RemoveThisFromList();
                    targetSurveyable = null;
                    GenerateStateButtons();
                } else {
                    PlaySound(GlobalAssets.GetSound("Negative"));
                }
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

            if (PackAnythingStaticVars.SurveableCmps.Count == 0 || PackAnythingStaticVars.MoveStatus.HaveAnObjectMoving) {
                GameObject obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, force_active: true);
                Sprite sprite = Assets.GetSprite((HashedString)"action_building_disabled");
                MultiToggle component = obj.GetComponent<MultiToggle>();
                component.GetComponent<ToolTip>().SetSimpleTooltip(PackAnythingString.UI.SIDE_SCREEN.TOOL_TIP);
                component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = sprite;
                buttons.Add(count++, component);
                return;
            }

            foreach(Surveyable surveyable in PackAnythingStaticVars.SurveableCmps) {
                if(surveyable != null) {
                    if (surveyable.gameObject.HasTag("OilWell") 
                        && surveyable.gameObject.gameObject.GetComponent<BuildingAttachPoint>()?.points[0].attachedBuilding != null) continue;
                    GameObject obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, force_active: true);
                    Sprite sprite = Def.GetUISprite(surveyable.gameObject).first;
                    if (sprite.name ==  "unknown") {
                        sprite = Def.GetUISprite(surveyable.gameObject, "place").first;
                    }
                    MultiToggle component = obj.GetComponent<MultiToggle>();
                    Surveyable tempSurveyable = PackAnythingStaticVars.MoveStatus.surveyable;
                    if (tempSurveyable != null && tempSurveyable == surveyable) {
                        component.ChangeState(1);
                        targetSurveyable = surveyable;
                    }
                    component.GetComponent<ToolTip>().SetSimpleTooltip(UI.StripLinkFormatting(surveyable.GetProperName()) + PackAnythingString.UI.SIDE_SCREEN.TOOL_TIP_OBJ);
                    component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = sprite;
                    component.onClick = delegate {
                        if (PackAnythingStaticVars.MoveStatus.surveyable != surveyable) {
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
            var newScreen = template.AddComponent<ModifierSideScreen>();
            newScreen.stateButtonPrefab = STATE_BUTTON_PREFAB.Get(oldScreen);
            newScreen.applyButton = APPLY_BUTTON.Get(oldScreen);
            newScreen.applyButton.GetComponent<ToolTip>().SetSimpleTooltip(PackAnythingString.UI.SIDE_SCREEN.APPLY_BUTTON_TOOL_TIP);
            newScreen.clearButton = CLEAR_BUTTON.Get(oldScreen);
            TryChangeText(newScreen.applyButton.gameObject.transform,"Label", PackAnythingString.UI.SIDE_SCREEN.APPLY_BUTTON_TEXT);
            newScreen.buttonContainer = BUTTON_CONTAINER.Get(oldScreen);
            newScreen.clearButton = CLEAR_BUTTON.Get(oldScreen);
            newScreen.clearButton.GetComponent<ToolTip>().SetSimpleTooltip(PackAnythingString.UI.SIDE_SCREEN.CANCEL_BUTTON_TOOL_TIP);
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

        private void ActiveMoveTool(Surveyable surveyable) {
            MoveTargetTool.Instance.Acitvate(surveyable);
        }

    }

}
