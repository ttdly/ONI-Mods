using System;
using System.Collections.Generic;
using GeyserExpandMachine.Buildings;
using KSerialization;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using UnityEngine;
using UnityEngine.UI;

namespace GeyserExpandMachine.Screen {
    public class ExpandSideScreen : SideScreenContent {
        
        private GeyserExpand expand;
        public Toggle[] toggles;

        [Serialize]
        public GeyserExpand.RunMode runMode = GeyserExpand.RunMode.Default;

        public GeyserExpand.RunMode RunMode {
            get {
                if (expand != null) return expand.runMode;
                return runMode;
            }
            set {
                runMode = value;
                expand.runMode = runMode;
                PUtil.LogDebug($"被设了{expand.runMode}");
            }
        }

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            // ModAssets.ListChildren(transform);
            transform
                .Find("MainContent/ToggleGroup/Default/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.UI.GEYSEREXPANDSIDESCREEN.MAINCONTENT.TOGGLEGROUP.DEFAULT.Label);
            transform
                .Find("MainContent/ToggleGroup/SkipEruption/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.UI.GEYSEREXPANDSIDESCREEN.MAINCONTENT.TOGGLEGROUP.SKIPERUPTION.Label);
            transform
                .Find("MainContent/ToggleGroup/SkipIdle/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.UI.GEYSEREXPANDSIDESCREEN.MAINCONTENT.TOGGLEGROUP.SKIPIDLE.Label);
            transform
                .Find("MainContent/ToggleGroup/Dormancy/Label")
                .gameObject.GetComponent<LocText>()
                .SetText(ModString.UI.GEYSEREXPANDSIDESCREEN.MAINCONTENT.TOGGLEGROUP.DORMANCY.Label);

            toggles = transform.GetComponentsInChildren<Toggle>();

            foreach (var toggle in toggles) {
                toggle.onValueChanged.AddListener((value => OnToggleValueChange(toggle)));
            }
        }

        public override void SetTarget(GameObject target) {
            base.SetTarget(target);
            expand = target.GetComponent<GeyserExpand>();
            if (runMode == expand.runMode) return;
            foreach (var toggle in toggles) {
                if (toggle.name != Enum.GetName(typeof(GeyserExpand.RunMode), expand.runMode)) continue;
                toggle.isOn = true;
                break;
            }
        }

        public override string GetTitle() {
            return "流量/模式控制";
        }

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<GeyserExpand>() != null && target.GetComponent<GeyserExpandProxy>() != null;
        }

        void OnToggleValueChange(Toggle toggle) {
            if (!toggle.isOn) return;
            PUtil.LogDebug($"Toogle {toggle.name}");
            if (toggle.name == "SkipIdle")  {RunMode = GeyserExpand.RunMode.SkipIdle; return;}
            if (toggle.name == "SkipEruption") {RunMode = GeyserExpand.RunMode.SkipErupt; return;}
            if (toggle.name == "Default")   {RunMode = GeyserExpand.RunMode.Default; return;}
            if (toggle.name == "Dormancy")   {RunMode = GeyserExpand.RunMode.Dormant; }

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