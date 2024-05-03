using PeterHan.PLib.Core;
using PeterHan.PLib.UI;
using UnityEngine;
using PeterHan.PLib.Detours;
using System.Collections.Generic;

namespace WirelessProject.ProwerManager {
    public class LinkToProxyScreen : SideScreenContent {
        public static LinkToProxyScreen Instance = null;
        private GameObject MainContainer;
        private readonly Dictionary<int, GameObject> IdAndCheckBoxDic = new Dictionary<int, GameObject>();
        private BaseLinkToProxy targetPowerEquipment;
        public KeyValuePair<int, GameObject> LastCheckBox = new KeyValuePair<int, GameObject>(-2, null);

        protected override void OnPrefabInit() {
            Init();
        }

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<BaseLinkToProxy>() != null;
        }

        public override string GetTitle() {
            return "None";
        }

        #region 操作
        private void OnChangeState(int id) {
            if (targetPowerEquipment == null) {
                PUtil.LogWarning("There is no targetEquipment");
                return;
            }
            if (id == targetPowerEquipment.ProxyListId) return;
            
            // 先将当前选中的复选框状态置 0
            if (targetPowerEquipment.ProxyListId == LastCheckBox.Key && LastCheckBox.Value != null) {
                PCheckBox.SetCheckState(LastCheckBox.Value, PCheckBox.STATE_UNCHECKED);
            } else {
                if (CheckAndGetCheckBox(targetPowerEquipment.ProxyListId, out GameObject lastCheckBox)) {
                    PCheckBox.SetCheckState(lastCheckBox, PCheckBox.STATE_UNCHECKED);
                }
            }
            targetPowerEquipment.ChangeProxy(id); // 切换端口
            // 再将当前选中的复选框状态置 1
            if (CheckAndGetCheckBox(id, out GameObject currentCheckBox)) {
                PCheckBox.SetCheckState(currentCheckBox, PCheckBox.STATE_CHECKED);
                LastCheckBox = new KeyValuePair<int, GameObject>(id, currentCheckBox);
            }
        }

        public void SetTarget(BaseLinkToProxy baseLink) {
#if DEBUG
            PUtil.LogDebug($"This equipment proxy id is {baseLink.ProxyListId}");
#endif
            targetPowerEquipment = baseLink;
            Refresh(baseLink.ProxyListId);
        }

        private void Refresh(int id) {
            if (LastCheckBox.Key == targetPowerEquipment.ProxyListId) return;
            if (LastCheckBox.Value != null) {
                PCheckBox.SetCheckState(LastCheckBox.Value, PCheckBox.STATE_UNCHECKED);
            }
            if (CheckAndGetCheckBox(id, out GameObject currentCheckBox)) {
                PCheckBox.SetCheckState(currentCheckBox, PCheckBox.STATE_CHECKED);
                LastCheckBox = new KeyValuePair<int, GameObject>(id, currentCheckBox);
            }
        }
        #endregion

        private bool CheckAndGetCheckBox(int id, out GameObject result) {
            if (!IdAndCheckBoxDic.ContainsKey(id)) {
                result = null;
#if DEBUG
                PUtil.LogDebug($"Dic not contains id {id}");
#endif
                return false;
            }
            return IdAndCheckBoxDic.TryGetValue(id, out result);
        }
        
        #region UI 相关
        private void Init() {
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
            AddCheckBox(-1, Strings.PowerManager.NoTerminal);
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
            IDetouredField<DetailsScreen, Dictionary<KScreen, KScreen>> SSS_LIST =
                PDetours.DetourFieldLazy<DetailsScreen, Dictionary<KScreen, KScreen>>("instantiatedSecondarySideScreens");
            GameObject sideScreen2ContentBody = SIDE_2_SCREENS.Get(instance);
            Dictionary<KScreen, KScreen> instantiatedSecondarySideScreens = SSS_LIST.Get(instance);
            string name = "PowerProxy";
            if (sideScreen2ContentBody != null) {
                GameObject ProxyContainer = PUIElements.CreateUI(sideScreen2ContentBody, name);
                BoxLayoutGroup box = ProxyContainer.AddComponent<BoxLayoutGroup>();
                box.Params = new BoxLayoutParams {
                    Direction = PanelDirection.Vertical,
                    Alignment = TextAnchor.UpperCenter,
                    Margin = new RectOffset(1, 1, 0, 1),
                };
                box.SetLayoutHorizontal();
                Instance = ProxyContainer.AddComponent<LinkToProxyScreen>();
                instantiatedSecondarySideScreens?.Add(Instance, Instance.GetComponent<KScreen>());
            }
        }

        #region 添加或删除复选框
        public void AddCheckBox(int id, string text) {
            if (IdAndCheckBoxDic.ContainsKey(id)) return;
            GameObject newCheckBox = new PCheckBox {
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
        #endregion
        #endregion
    }
}
