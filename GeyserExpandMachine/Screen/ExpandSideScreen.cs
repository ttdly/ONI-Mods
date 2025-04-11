using System;
using System.Collections.Generic;
using GeyserExpandMachine.Buildings;
using GeyserExpandMachine.GeyserModify;
using KSerialization;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using UnityEngine;
using UnityEngine.UI;

namespace GeyserExpandMachine.Screen {
    public class ExpandSideScreen : SideScreenContent {
        
        private GeyserLogicExpand expand;
        public Toggle[] toggles;

        [Serialize]
        public GeyserLogicController.RunMode runMode = GeyserLogicController.RunMode.Default;

        public GeyserLogicController.RunMode RunMode {

            set {
                runMode = value;
                expand.RunMode = value;
                // PUtil.LogDebug($"被设了{expand.RunMode}");
            }
        }

        public override int GetSideScreenSortOrder() => -1;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            // ModAssets.ListChildren(transform);
            transform
                .Find("MainContent/ToggleGroup/Default/Background/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.TOGGLEGROUP.DEFAULT.Label);
            transform
                .Find("MainContent/ToggleGroup/SkipEruption/Background/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.TOGGLEGROUP.SKIPERUPTION.Label);
            transform
                .Find("MainContent/ToggleGroup/SkipIdle/Background/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.TOGGLEGROUP.SKIPIDLE.Label);
            transform
                .Find("MainContent/ToggleGroup/Dormancy/Background/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.TOGGLEGROUP.DORMANCY.Label);

            toggles = transform.GetComponentsInChildren<Toggle>();

            foreach (var toggle in toggles) {
                toggle.onValueChanged.AddListener((value => OnToggleValueChange(toggle, value)));
            }
        }

        public override void SetTarget(GameObject target) {
            base.SetTarget(target);
            expand = target.GetComponent<GeyserLogicExpand>();
            foreach (var toggle in toggles) {
                if (toggle.name != Enum.GetName(typeof(GeyserLogicController.RunMode), expand.RunMode)) continue;
                toggle.isOn = true;
                ActivateToggle(toggle);
            }
        }

        private static void ActivateToggle(Toggle toggle) {
            toggle.transform.Find("Background").gameObject.GetComponent<Image>().color =
                new Color(0.4980392f, 0.2392157f, 0.3686275f);
            toggle.transform.Find("Background/Label").gameObject.GetComponent<LocText>().color = Color.white;
        }

        private static void DeactivateToggle(Toggle toggle) {
            toggle.transform.Find("Background").gameObject.GetComponent<Image>().color = Color.white;
            toggle.transform.Find("Background/Label").gameObject.GetComponent<LocText>().color = Color.black;
        }

        public override string GetTitle() {
            return ModString.SIDESCREEN.TITLE;
        }

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<GeyserLogicExpand>() != null && target.GetComponent<GeyserExpandProxy>() != null;
        }

        void OnToggleValueChange(Toggle toggle, bool value) {
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
                case "SkipEruption":
                    RunMode = GeyserLogicController.RunMode.SkipErupt;
                    break;
                case "Default":
                    RunMode = GeyserLogicController.RunMode.Default;
                    break;
                case "Dormancy":
                    RunMode = GeyserLogicController.RunMode.Dormant;
                    break;
                default:
                    PUtil.LogDebug($"Toogle {toggle.name}");
                    break;
            }

        }
        
        public static void AddSideScreenContent() {
            var instance = DetailsScreen.Instance;
            
            if (instance == null) {
                PUtil.LogDebug("DetailsScreen is not yet initialized, try a postfix on DetailsScreen.OnPrefabInit");
                return;
            }
            
            var screensDe = PDetours.DetourFieldLazy<DetailsScreen,List<DetailsScreen.SideScreenRef>>("sideScreens");
            
            var screens = screensDe.Get(instance);
        
            if (screens == null) {
                PUtil.LogDebug("DetailsScreen is not yet initialized, try a postfix on DetailsScreen.OnPrefabInit");
                return;
            }
        
            PUtil.LogDebug("Successfully added DetailsScreen to DetailsScreen");
        
            
            var name = "ExpandSideScreen";
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