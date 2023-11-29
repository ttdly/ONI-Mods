using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Database;

namespace PackAnything {
    internal class BuildingFacadeSideScreen : SideScreenContent {

        private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> APPLY_BUTTON = PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(applyButton));
        private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> CLEAR_BUTTON = PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(clearButton));
        private static readonly IDetouredField<ArtableSelectionSideScreen, GameObject> STATE_BUTTON_PREFAB = PDetours.DetourField<ArtableSelectionSideScreen, GameObject>(nameof(stateButtonPrefab));
        private static readonly IDetouredField<ArtableSelectionSideScreen, RectTransform> BUTTON_CONTAINER = PDetours.DetourField<ArtableSelectionSideScreen, RectTransform>(nameof(buttonContainer));
        private KButton applyButton;
        private KButton clearButton;
        public GameObject stateButtonPrefab;
        [SerializeField]
        private RectTransform buttonContainer;
        private readonly Dictionary<string, MultiToggle> buttons = new Dictionary<string, MultiToggle>();
        private BuildingFacade targetBuildingFacade;

        protected override void OnSpawn() {
            base.OnSpawn();
           
        }

        public override void SetTarget(GameObject target) {
            targetBuildingFacade = target.GetComponent<BuildingFacade>();
            GenerateStateButtons();
        }


        public void GenerateStateButtons() {
            foreach (KeyValuePair<string, MultiToggle> button in buttons) {
                Util.KDestroyGameObject(button.Value.gameObject);
            }

            BuildingDef def = targetBuildingFacade.gameObject.GetComponent<Building>().Def;
            List<string> ava = def.AvailableFacades;
           
            foreach (string facade in  ava) {
                BuildingFacadeResource buildingFacadeResource = Db.GetBuildingFacades().Get(facade);
                GameObject obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, force_active: true);
                Sprite sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(buildingFacadeResource.AnimFile));
                MultiToggle component = obj.GetComponent<MultiToggle>();
                component.GetComponent<ToolTip>().SetSimpleTooltip(buildingFacadeResource.Name);
                component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = sprite;
                component.onClick = delegate {
                    targetBuildingFacade.ApplyBuildingFacade(buildingFacadeResource);
                };
                buttons.Add(buildingFacadeResource.Name,component);
            }

        }

        private void RefreshButtons() {
            foreach (KeyValuePair<string, MultiToggle> kvp in buttons) {
                kvp.Value.gameObject.SetActive(value: true);
                kvp.Value.ChangeState(0);
            }
        }

        private static BuildingFacadeSideScreen CreateScreen(ArtableSelectionSideScreen sideScreen) {
            var template = Instantiate(sideScreen.gameObject);
            template.name = nameof(BuildingFacadeSideScreen);
            bool active = template.activeSelf;
            template.SetActive(false);
            var oldScreen = template.GetComponent<ArtableSelectionSideScreen>();
            var ours = template.AddComponent<BuildingFacadeSideScreen>();
            ours.stateButtonPrefab = STATE_BUTTON_PREFAB.Get(oldScreen);
            ours.applyButton = APPLY_BUTTON.Get(oldScreen);
            ours.applyButton.gameObject.SetActive(false);
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

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<BuildingFacade>() != null;
        }
    }
}
