using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using UnityEngine;
using PeterHan.PLib.Detours;
using System.Collections.Generic;

namespace WirelessProject.ProwerManager {
    public class LinkToProxyScreen : SideScreenContent {
        public static LinkToProxyScreen Instance = null;
        public static bool Dirty = true;
        private GameObject MainContainer;
        private readonly Dictionary<int, GameObject> IdAndCheckBoxDic = new Dictionary<int, GameObject>();
        private BaseLinkToProxy targetPowerEquipment;


        public override bool IsValidForTarget(GameObject target) {
            PUtil.LogDebug("hahaha");
            return target.GetComponent<BaseLinkToProxy>() != null;
        }

        public override string GetTitle() {
            return "dddd";
        }

        public void AddCheckBox(int id, string text) {
            if (IdAndCheckBoxDic.ContainsKey(id)) return;
            GameObject newCheckBox =  new PCheckBox {
                Text = text,
                TextStyle = PUITuning.Fonts.TextDarkStyle,
                Margin = new RectOffset(0, 0, 0, 5),
                OnChecked = delegate {
                    OnChangeState(id);
                }
            }.AddTo(MainContainer);
            IdAndCheckBoxDic.Add(id, newCheckBox);
        }

        public void RemoveCheckBox(int id) {
            if (IdAndCheckBoxDic.TryGetValue(id, out GameObject checkBox)) {
                checkBox.DeleteObject();
                IdAndCheckBoxDic.Remove(id);
            } else {
                PUtil.LogWarning($"Id {id} has not checkbox");
            }
        }

        private void OnChangeState(int id) {
            PUtil.LogDebug($" {id} Change state ");
            if (targetPowerEquipment == null) {
                PUtil.LogWarning("There is no targetEquipment");
                return;
            }
            if (id == targetPowerEquipment.ProxyInfoId) return;
            if (IdAndCheckBoxDic.TryGetValue(targetPowerEquipment.ProxyInfoId, out GameObject previewCheckBox)) {
                PCheckBox.SetCheckState(previewCheckBox, 0);
            }
            if (IdAndCheckBoxDic.TryGetValue(id, out GameObject currentCheckBox)) {
                PCheckBox.SetCheckState(currentCheckBox, 1);
                targetPowerEquipment.ChangeProxy(id);
            }
        }

        public void SetTarget(BaseLinkToProxy baseLink) {
#if DEBUG
            PUtil.LogDebug($"This equipment proxy id is {baseLink.ProxyInfoId}");
#endif
            targetPowerEquipment = baseLink;
            RefreshStaues(baseLink);
        }

        private void RefreshStaues(BaseLinkToProxy baseLink) {
            foreach (KeyValuePair<int, GameObject> idAndCheckbox in IdAndCheckBoxDic) {
                if(idAndCheckbox.Key == baseLink.ProxyInfoId) {
                    PCheckBox.SetCheckState(idAndCheckbox.Value, 1);
                    continue;
                }
                PCheckBox.SetCheckState(idAndCheckbox.Value, 0);
            }
        }

        protected override void OnCmpDisable() {
            base.OnCmpDisable();
            DetailsScreen.Instance.ClearSecondarySideScreen();
        }

        protected override void OnPrefabInit() {
            var margin = new RectOffset(8, 8, 8, 8);
            PPanel panel = new PPanel() {
                FlexSize = Vector2.right,
                DynamicSize = true,
                Alignment = TextAnchor.LowerLeft,
                Margin = margin,
            }
            .AddOnRealize((obj) => { MainContainer = obj; });
            new PScrollPane() {
                Child = panel,
                FlexSize = Vector2.right,
                ScrollVertical = true,
                AlwaysShowVertical = true,
                TrackSize = 8,
            }
            .AddTo(gameObject);
            ContentContainer = gameObject;
            base.OnPrefabInit();
            AddCheckBox(-1, "不接入终端");
            foreach (KeyValuePair<int, PowerProxy.ProxyList> idAndProxyList in StaticVar.PowerInfoList) {
                AddCheckBox(idAndProxyList.Value.ProxyInfoId, idAndProxyList.Value.ProxyName);
            }
        }

        public static void AddSideScreenContent() {
            DetailsScreen instance = DetailsScreen.Instance;
            if (instance == null) {
                PUtil.LogDebug("DetailsScreen is not yet initialized, try a postfix on DetailsScreen.OnPrefabInit");
                return;
            }
            IDetouredField<DetailsScreen, GameObject> SIDE_2_SCREENS =
                PDetours.DetourFieldLazy<DetailsScreen, GameObject>("sideScreen2ContentBody");
            IDetouredField<SideScreenContent, GameObject> SS_CONTENT_CONTAINER =
                PDetours.DetourFieldLazy<SideScreenContent, GameObject>("ContentContainer");
            IDetouredField<DetailsScreen, Dictionary<KScreen, KScreen>> SSS_LIST =
                PDetours.DetourFieldLazy<DetailsScreen, Dictionary<KScreen, KScreen>>("instantiatedSecondarySideScreens");
            GameObject gameObject = SIDE_2_SCREENS.Get(instance);
            Dictionary<KScreen, KScreen> SSS_DIC = SSS_LIST.Get(instance);
            string name = "PowerProxy";
            if (gameObject != null) {
                GameObject gameObject2 = PUIElements.CreateUI(gameObject, name);
                BoxLayoutGroup box = gameObject2.AddComponent<BoxLayoutGroup>();
                box.Params = new BoxLayoutParams {
                    Direction = PanelDirection.Vertical,
                    Alignment = TextAnchor.UpperCenter,
                    Margin = new RectOffset(1, 1, 0, 1),
                };
                box.SetLayoutHorizontal();
                Instance = gameObject2.AddComponent<LinkToProxyScreen>();
                SSS_DIC?.Add(Instance, Instance);
            }
        }
    }
}
