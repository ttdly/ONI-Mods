using PeterHan.PLib.Core;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;
using PeterHan.PLib.Detours;

namespace PackAnything {
    public class WorldModifierSideScreen : SideScreenContent {
        private static readonly IDetouredField<GeoTunerSideScreen, GameObject> ROW_PREFAB = PDetours.DetourField<GeoTunerSideScreen, GameObject>(nameof(rowPrefab));
        private static readonly IDetouredField<GeoTunerSideScreen, RectTransform> ROW_CONTAINER= PDetours.DetourField<GeoTunerSideScreen, RectTransform>(nameof(rowContainer));
        private static readonly IDetouredField<GeoTunerSideScreen, Dictionary<object, GameObject>> ROW = PDetours.DetourField<GeoTunerSideScreen, Dictionary<object, GameObject>>(nameof(rows));

        private int uiRefreshSubHandle = -1;
        private WorldModifier targetBuilding;
        public GameObject rowPrefab;
        public RectTransform rowContainer;
        public Dictionary<object, GameObject> rows = new Dictionary<object, GameObject>();

        public override void SetTarget(GameObject target) {
            targetBuilding = target.GetComponent<WorldModifier>();
            RefreshOptions();
            uiRefreshSubHandle = target.Subscribe(GameHashes.UIRefresh.GetHashCode(), RefreshOptions);
        }

        public override string GetTitle() {
            return PackAnythingString.UI.SIDE_SCREEN.NAME;
        }

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<WorldModifier>() != null;
        }

        public static void AddSideScreen(IList<DetailsScreen.SideScreenRef> existing , GameObject parent) {
            bool found = false;
            foreach (var ssRef in existing)
                if (ssRef.screenPrefab is GeoTunerSideScreen sideScreen) {
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

        private void RefreshOptions(object data = null) {
            int idx = 0;
            int num = idx + 1;
            List<Surveyable> items = PackAnythingStaticVars.SurveableCmps;
            SetRow(idx, (string)UI.UISIDESCREENS.GEOTUNERSIDESCREEN.NOTHING, Assets.GetSprite((HashedString)"action_building_disabled"), null, true);
            foreach (Surveyable item in items) {
                SetRow(num++, UI.StripLinkFormatting(item.GetProperName()), Def.GetUISprite(item.gameObject).first, item, surveyed: true);
            }
            for (int i = num; i < rowContainer.childCount; i++) {
                rowContainer.GetChild(i).gameObject.SetActive(value: false);
            }
        }

        private void ClearRows() {
            for (int num = rowContainer.childCount - 1; num >= 0; num--) {
                Util.KDestroyGameObject(rowContainer.GetChild(num));
            }
            rows.Clear();
        }

        private void SetRow(int idx, string name, Sprite icon, Surveyable surveyable, bool surveyed) {
            bool flag = surveyable == null;
            GameObject gameObject = ((idx >= rowContainer.childCount) ? Util.KInstantiateUI(rowPrefab, rowContainer.gameObject, force_active: true) : rowContainer.GetChild(idx).gameObject);
            HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
            LocText reference = component.GetReference<LocText>("label");
            reference.text = name;
            Image reference2 = component.GetReference<Image>("icon");
            reference2.sprite = icon;
            reference2.color = (surveyed ? Color.white : new Color(0f, 0f, 0f, 0.5f));
            if (flag) {
                reference2.color = Color.black;
            }
            ToolTip[] componentsInChildren = gameObject.GetComponentsInChildren<ToolTip>();
            ToolTip toolTip = componentsInChildren.First();
            bool usingStudiedTooltip = surveyable != null && (flag || surveyed);
            toolTip.SetSimpleTooltip(usingStudiedTooltip ? UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP.ToString() : UI.UISIDESCREENS.GEOTUNERSIDESCREEN.UNSTUDIED_TOOLTIP.ToString());
            toolTip.enabled = surveyable != null;
            LocText reference3 = component.GetReference<LocText>("amount");
            reference3.transform.parent.gameObject.SetActive(false);
            MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
            component2.ChangeState((PackAnythingStaticVars.targetSurveyable == surveyable) ? 1 : 0);
            component2.onClick = delegate {
                if (PackAnythingStaticVars.targetSurveyable != surveyable) {
                    PackAnythingStaticVars.targetSurveyable = surveyable;
                    ActiveMoveTool(surveyable);
                }
                RefreshOptions();
            };
            component2.onDoubleClick = delegate {
                if (surveyable != null) {
                    CameraController.Instance.CameraGoTo(surveyable.transform.GetPosition());
                    return true;
                }
                return false;
            };
        }

        private static WorldModifierSideScreen CreateScreen(GeoTunerSideScreen sideScreen) {
            var template = Instantiate(sideScreen.gameObject);
            template.name = nameof(WorldModifierSideScreen);
            bool active = template.activeSelf;
            template.SetActive(false);
            var oldScreen = template.GetComponent<GeoTunerSideScreen>();
            var ours = template.AddComponent<WorldModifierSideScreen>();
            ours.rowPrefab = ROW_PREFAB.Get(oldScreen);
            ours.rowContainer = ROW_CONTAINER.Get(oldScreen);
            ours.rows = ROW.Get(oldScreen);
            ours.rows.Clear();
            DestroyImmediate(oldScreen);
            template.SetActive(active);
            return ours;
        }

        private void ActiveMoveTool(Surveyable surveyable) {
            MoveTargetTool.Instance.Acitvate(surveyable);
        }

    }
}
