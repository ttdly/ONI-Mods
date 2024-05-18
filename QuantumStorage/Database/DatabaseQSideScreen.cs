using Database;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChangeBlueprints {
    internal class DatabaseQSideScreen : SideScreenContent {
        private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> APPLY_BUTTON = PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(applyButton));
        private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> CLEAR_BUTTON = PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(clearButton));
        private static readonly IDetouredField<ArtableSelectionSideScreen, GameObject> STATE_BUTTON_PREFAB = PDetours.DetourField<ArtableSelectionSideScreen, GameObject>(nameof(stateButtonPrefab));
        private static readonly IDetouredField<ArtableSelectionSideScreen, RectTransform> BUTTON_CONTAINER = PDetours.DetourField<ArtableSelectionSideScreen, RectTransform>(nameof(buttonContainer));
        private static readonly IDetouredField<ArtableSelectionSideScreen, RectTransform> SCROLL = PDetours.DetourField<ArtableSelectionSideScreen, RectTransform>(nameof(scrollTransoform));
        private static string Title {
            get {
                if (Localization.GetCurrentLanguageCode() == "zh_klei") {
                    return "切换蓝图";
                }
                return "Change Blueprint";
            }
        }
        private KButton applyButton;
        private KButton clearButton;
        public GameObject stateButtonPrefab;
        private RectTransform scrollTransoform;
        private readonly Dictionary<string, MultiToggle> buttons = new Dictionary<string, MultiToggle>();
        [SerializeField]
        private RectTransform buttonContainer;
        public List<string> primits = new List<string>();
        private BuildingFacade buildingFacade;
        private BuildingDef buildingDef;

        protected override void OnSpawn() {
            base.OnSpawn();
        }

        public override bool IsValidForTarget(GameObject target) {
            if (target.GetComponent<BuildingFacade>() == null) return false;
            if (target.GetComponent<Building>().Def.AvailableFacades.Count == 0) return false;
            primits.Clear();
            foreach (string id in target.GetComponent<Building>().Def.AvailableFacades) {
                if (id != null && PermitItems.IsPermitUnlocked(Db.Get().Permits.Get(id))){
                    primits.Add(id);
                }
            }
            return primits.Count>0;
        }

        public override void SetTarget(GameObject target) {
           buildingFacade = target.GetComponent<BuildingFacade>();
           buildingDef = target.GetComponent<Building>().Def;
           GenerateStateButtons();
        }

        public override string GetTitle() {
            return Title;
        }

        public void GenerateStateButtons() {
            foreach (KeyValuePair<string, MultiToggle> button in buttons) {
                Util.KDestroyGameObject(button.Value.gameObject);
            }
            buttons.Clear();
            if (buildingDef.AvailableFacades.Count == 0) {
                gameObject.SetActive(false);
            }

            if (true) {
                GameObject rawGameObject = Util.KInstantiate(Assets.GetPrefab(buildingDef.PrefabID));
                BuildingDef rawDef = rawGameObject.GetComponent<Building>().Def;
                GameObject obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, force_active: true);
                Sprite sprite = Def.GetUISprite(rawGameObject).first;
                MultiToggle component = obj.GetComponent<MultiToggle>();
                component.GetComponent<ToolTip>().SetSimpleTooltip(rawDef.Name);
                component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = sprite;
                if (buildingFacade.CurrentFacade == "" || buildingFacade.CurrentFacade == null) {
                    component.ChangeState(1);
                }
                component.onClick = delegate {
                    buttons.TryGetValue(buildingFacade.CurrentFacade, out MultiToggle perButton);
                    if (perButton != null) {
                        perButton.ChangeState(0);
                        component.ChangeState(1);
                    }
                    ChangeBuilding(rawDef.AnimFiles, rawDef.Name, rawDef.Desc);
                };
                buttons.Add("DEFAULT", component);
            }

            foreach (string facade in primits) {
                BuildingFacadeResource buildingFacadeResource = Db.GetBuildingFacades().Get(facade);
                GameObject obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, force_active: true);
                Sprite sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(buildingFacadeResource.AnimFile));
                MultiToggle component = obj.GetComponent<MultiToggle>();
                component.GetComponent<ToolTip>().SetSimpleTooltip(buildingFacadeResource.Name);
                component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = sprite;
                component.ChangeState((facade == buildingFacade.CurrentFacade)? 1 : 0);
                component.onClick = delegate {
                    MultiToggle perButton;
                    if (buildingFacade.CurrentFacade == "" || buildingFacade.CurrentFacade == null) {
                        buttons.TryGetValue("DEFAULT", out perButton);
                    } else {
                        buttons.TryGetValue(buildingFacade.CurrentFacade, out perButton);
                        if(perButton.CurrentState == 0) {
                            buttons.TryGetValue("DEFAULT", out perButton);
                        }
                    }
                    
                    if (perButton != null) {
                        perButton.ChangeState(0);
                        component.ChangeState(1);
                    }
                    buildingFacade.ApplyBuildingFacade(buildingFacadeResource);
                };
                buttons.Add(facade, component);
            }
            return;
        }

        private static DatabaseQSideScreen CreateScreen(ArtableSelectionSideScreen sideScreen) {
            var template = Instantiate(sideScreen.gameObject);
            template.name = nameof(DatabaseQSideScreen);
            bool active = template.activeSelf;
            template.SetActive(false);
            var oldScreen = template.GetComponent<ArtableSelectionSideScreen>();
            var ours = template.AddComponent<DatabaseQSideScreen>();
            ours.stateButtonPrefab = STATE_BUTTON_PREFAB.Get(oldScreen);
            ours.applyButton = APPLY_BUTTON.Get(oldScreen);
            ours.applyButton.gameObject.SetActive(false);
            ours.buttonContainer = BUTTON_CONTAINER.Get(oldScreen);
            ours.clearButton = CLEAR_BUTTON.Get(oldScreen);
            ours.scrollTransoform = SCROLL.Get(oldScreen);
            ours.gameObject.transform.Find("Content").Find("Scroll").TryGetComponent(out LayoutElement scrollLayout);
            ours.buttonContainer.gameObject.transform.TryGetComponent(out GridLayoutGroup containerLayout);
            ours.stateButtonPrefab.gameObject.transform.TryGetComponent(out LayoutElement stateButtonPrefabLayout);
            if (scrollLayout != null) {
                scrollLayout.minHeight = 54;
                scrollLayout.preferredHeight = 5;
            }
            if (containerLayout != null) {
                containerLayout.cellSize = new Vector2(50, 50);
                containerLayout.constraintCount = 5;
                containerLayout.spacing = new Vector2(4, 4);
            }
            if (stateButtonPrefabLayout != null) {
                stateButtonPrefabLayout.minHeight = 50;
                stateButtonPrefabLayout.minWidth = 50;
            }
            ours.clearButton.gameObject.SetActive(false);
            DestroyImmediate(oldScreen);
            template.SetActive(active);
            return ours;
        }

        public static void ListAllChildrenPath(Transform parent, string path = "/", int level = 0, int maxDepth = 10) {
            if (level >= maxDepth) return;
            foreach (Transform child in parent) {
                var newpath = string.Concat(path + child.name + "/");
                Console.WriteLine(newpath);
                ListAllChildrenPath(child, newpath, level + 1);
            }
        }

        private void ChangeBuilding(KAnimFile[] animFiles, string displayName, string desc) {
            Building[] components = buildingFacade.gameObject.GetComponents<Building>();
            foreach (Building building in components) {
                building.SetDescription(desc);
                building.GetComponent<KBatchedAnimController>().SwapAnims(animFiles);
            }
            buildingFacade.gameObject.GetComponent<KSelectable>().SetName(displayName);
            if (!(buildingFacade.gameObject.GetComponent<AnimTileable>() != null) || components.Length == 0)
                return;
            GameScenePartitioner.Instance.TriggerEvent(components[0].GetExtents(), GameScenePartitioner.Instance.objectLayers[1], null);
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
                    existing.Add(newScreen);
                    break;
                }
            if (!found)
                PUtil.LogWarning("Unable to find side screen!");
        }
    }
}
