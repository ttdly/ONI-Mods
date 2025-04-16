using System;
using System.Collections.Generic;
using GeyserExpandMachine.Buildings;
using GeyserExpandMachine.GeyserModify;
using KSerialization;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GeyserExpandMachine.Screen {
    public class ExpandSideScreen : SideScreenContent {
        private static readonly Color Pink = new (0.4980392f, 0.2392157f, 0.3686275f);
        private static readonly Color Blue = new (0.2431373f, 0.2627451f, 0.3411765f);
            
        private BaseGeyserExpand expand;
        public Toggle[] toggles;

        public FNumberInputField flowControlInput;
        public FNumberInputField logicMaxInputField;
        public FNumberInputField logicMinInputField;
        public FSlider flowControlSlider;
        public FSlider logicMaxSlider;
        public FSlider logicMinSlider;
        private const float MaxFlowValue = 10000f;
        

        [Serialize]
        public GeyserLogicController.RunMode runMode = GeyserLogicController.RunMode.Default;


        public GeyserLogicController.RunMode RunMode {

            set {
                runMode = value;
                expand.RunMode = value;
            }
        }
        
        public override int GetSideScreenSortOrder() => -1;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            // ModAssets.ListChildren(transform);
            InitText();

            flowControlInput = transform.Find("FlowControlContent/SilderControl/InputContent/Input").FindOrAddComponent<FNumberInputField>();
            flowControlInput.SetTextFromData("rehook");
            flowControlSlider = transform.Find("FlowControlContent/SilderControl/Slider").gameObject.AddComponent<FSlider>();
            flowControlSlider.AttachInputField(flowControlInput);
            flowControlSlider.AttachOutputField(flowControlInput);
            flowControlSlider.SetMin(0);
            flowControlSlider.SetMax(10000);
            flowControlSlider.OnChange += OnFlowMassValueChanged;
            flowControlSlider.SetWholeNumbers(true);
            
            logicMaxInputField = transform.Find("LogicActivateContent/Max/Input").FindOrAddComponent<FNumberInputField>();
            logicMinInputField = transform.Find("LogicActivateContent/Min/Input").FindOrAddComponent<FNumberInputField>();
            logicMaxInputField.SetTextFromData("rehook");
            logicMinInputField.SetTextFromData("rehook");
            
            logicMaxSlider = transform.Find("LogicActivateContent/Max/Slider").gameObject.AddComponent<FSlider>();
            logicMinSlider = transform.Find("LogicActivateContent/Min/Slider").gameObject.AddComponent<FSlider>();
            logicMaxSlider.UnitString = "%";
            logicMaxSlider.AttachInputField(logicMaxInputField);
            logicMaxSlider.AttachOutputField(logicMaxInputField);
            logicMaxSlider.SetMin(0);
            logicMaxSlider.SetMax(100);
            logicMaxSlider.OnChange += OnLogicMaxValueChanged;
            logicMaxSlider.SetWholeNumbers(true);
            
            logicMinSlider.UnitString = "%";
            logicMinSlider.AttachInputField(logicMinInputField);
            logicMinSlider.AttachOutputField(logicMinInputField);
            logicMinSlider.SetMin(0);
            logicMinSlider.SetMax(100);
            logicMinSlider.OnChange += OnLogicMinValueChanged;
            logicMinSlider.SetWholeNumbers(true);
            
            toggles = transform.Find("ModeControl").GetComponentsInChildren<Toggle>();
            foreach (var toggle in toggles) {
                DeactivateToggle(toggle);
                toggle.onValueChanged.AddListener((value => OnToggleValueChange(toggle, value)));
            }
            
        }

        public void OnFlowMassValueChanged(float value) {
            expand.FlowMass = value;
            flowControlSlider.SetWholeNumbers(value > 1);
        }

        public void OnLogicMaxValueChanged(float value) {
            expand.logicMax = value;
        }

        public void OnLogicMinValueChanged(float value) {
            expand.logicMin = value;
        }

        private void InitText() {
            transform
                .Find("ModeControl/Default/Background/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.TOGGLEGROUP.DEFAULT.Label);
            transform
                .Find("ModeControl/SkipErupt/Background/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.TOGGLEGROUP.SKIPERUPTION.Label);
            transform
                .Find("ModeControl/SkipIdle/Background/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.TOGGLEGROUP.SKIPIDLE.Label);
            transform
                .Find("ModeControl/SkipDormant/Background/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.TOGGLEGROUP.SKIPDORMANCY.Label);
            transform
                .Find("ModeControl/AlwaysDormant/Background/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.TOGGLEGROUP.DORMANCY.Label);
            
            
            transform.Find("FlowControlTitle/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(STRINGS.UI.UISIDESCREENS.VALVESIDESCREEN.TITLE);
            transform.Find("FlowControlContent/SilderControl/InputContent/unit")
                .gameObject.GetComponent<LocText>()
                .SetText("g");
            transform.Find("FlowControlContent/SilderControl/Hint/Min")
                .gameObject.GetComponent<LocText>()
                .SetText("0g");
            transform.Find("FlowControlContent/SilderControl/Hint/Max")
                .gameObject.GetComponent<LocText>()
                .SetText("10000g");

            
            transform.Find("LogicActivate/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_TITLE);
            transform.Find("LogicActivateContent/Max/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_DEACTIVATE);
            transform.Find("LogicActivateContent/Min/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.SIDESCREEN_ACTIVATE);
        }

        private void RefreshUI() {
            if (expand == null) return;
            flowControlSlider.SetCurrent(expand.FlowMass);
            logicMaxSlider.SetCurrent(expand.logicMax);
            logicMinSlider.SetCurrent(expand.logicMin);
            foreach (var toggle in toggles) {
                if (toggle.name != Enum.GetName(typeof(GeyserLogicController.RunMode), expand.RunMode)) continue;
                toggle.isOn = true;
                ActivateToggle(toggle);
            }
        }
        
        public override void SetTarget(GameObject target) {
            base.SetTarget(target);
            expand = target.GetComponent<BaseGeyserExpand>();
            if (!expand.safe) {
                LogUtil.Error("不安全状态不允许刷新界面，请及时删除不安全的建筑");
                return;
            }
            RefreshUI();
        }

        public void FlowInputValueChanged(string value) {
            if (float.TryParse(value, out var result)) {
                if (result > MaxFlowValue || result < 0) result = MaxFlowValue;
            }
            else {
                result = MaxFlowValue;
            }
            flowControlSlider.SetCurrent(result);
            expand.FlowMass = result;
        }
        
        
        private static void ActivateToggle(Toggle toggle) {
            toggle.transform.Find("Background").gameObject.GetComponent<Image>().color = Pink;
        }

        private static void DeactivateToggle(Toggle toggle) {
            toggle.transform.Find("Background").gameObject.GetComponent<Image>().color = Blue;
        }

        public override string GetTitle() {
            return ModString.SIDESCREEN.TITLE;
        }

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<BaseGeyserExpand>() != null && target.GetComponent<GeyserExpandProxy>() != null;
        }

        private void OnToggleValueChange(Toggle toggle, bool value) {
            if (value) {
                ActivateToggle(toggle);
            }
            else {
                DeactivateToggle(toggle);
                return;
            }

            switch (toggle.name) {
                case "SkipIdle":
                    RunMode = GeyserLogicController.RunMode.SkipIdle;
                    break;
                case "SkipErupt":
                    RunMode = GeyserLogicController.RunMode.SkipErupt;
                    break;
                case "Default":
                    RunMode = GeyserLogicController.RunMode.Default;
                    break;
                case "AlwaysDormant":
                    RunMode = GeyserLogicController.RunMode.AlwaysDormant;
                    break;
                case "SkipDormant":
                    RunMode = GeyserLogicController.RunMode.SkipDormant;
                    break;
                default:
                    Debug.LogWarning($"奇怪的 Toggle 项 {toggle.name}");
                    break;
            }

        }
        
        public static void AddSideScreenContent() {
            var instance = DetailsScreen.Instance;
            if (instance == null) {
                Debug.LogWarning("DetailsScreen 目前还没实例化 1");
                return;
            }
            var screensDe = PDetours.DetourFieldLazy<DetailsScreen,List<DetailsScreen.SideScreenRef>>("sideScreens");
            var screens = screensDe.Get(instance);
            if (screens == null) {
                Debug.LogWarning("DetailsScreen 目前还没实例化 2; 找不到 screens");
                return;
            }
            const string name = "ExpandSideScreen";
            var newScreen = ModAssets.ExpandSideSecondScreenPrefab.AddComponent<ExpandSideScreen>() as SideScreenContent;
            var newScreenRef = new DetailsScreen.SideScreenRef {
                name = name,
                offset = Vector2.zero,
                screenPrefab = newScreen,
                tab = DetailsScreen.SidescreenTabTypes.Config
            };
            screens.Add(newScreenRef);
        }
    }
}