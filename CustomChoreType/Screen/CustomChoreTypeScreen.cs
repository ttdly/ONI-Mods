using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace CustomChoreType.Screen {
    public class CustomChoreTypeScreen {    
        public static GameObject CustomChoreTypePrefab;
        private static GameObject _customChoreTypeGo;
        private static ChoreType _targetChoreType;
        private static GameObject _parent;
        private static GameObject _deleteButton;
        
        private static readonly Dictionary<string, ChoreGroupEntry> ChoreGroupBind =
            new Dictionary<string, ChoreGroupEntry>();
        

        public static void Show(ChoreType choreType) {
            if (_customChoreTypeGo == null) InitScreen();

            foreach (var valuePair in ChoreGroupBind) {
                valuePair.Value.toggle.image.color = Color.white;
            }
            _targetChoreType = choreType;
            _deleteButton.SetActive(Mod.Changes.ContainsKey(_targetChoreType.Id));
            _customChoreTypeGo.SetActive(true);
        }

        public static void Hide() {
            _customChoreTypeGo.SetActive(false);
        }

        private static void InitScreen() {
            GetParent();
            _customChoreTypeGo = Util.KInstantiateUI(CustomChoreTypePrefab, _parent);
            _customChoreTypeGo.transform.Find("Head/Title").gameObject.GetComponent<LocText>()
                .SetText(STRINGS.UI.UISIDESCREENS.BASICRECEPTACLE.DEPOSIT);
            _customChoreTypeGo.transform.Find("Head/Close").gameObject.GetComponent<Button>().onClick
                .AddListener(Hide);
            var choreGroupContainer = _customChoreTypeGo.transform
                .Find("Content/Groups").gameObject;
            var choreGroupPrefab = _customChoreTypeGo.transform
                .Find("Content/Groups/ChoreGroup").gameObject;

            _customChoreTypeGo.transform.Find("Content/Confirm")
                .gameObject.GetComponent<Button>().onClick.AddListener(ApplySetting);
            _customChoreTypeGo.transform.Find("Content/Confirm/Text")
                .gameObject.GetComponent<LocText>().SetText(STRINGS.UI.FRONTEND.PAUSE_SCREEN.ADD_DLC_MENU.CONFIRM);
            _customChoreTypeGo.transform.Find("Content/Clear/Text ")
                    .gameObject.GetComponent<LocText>().SetText(STRINGS.UI.SANDBOXTOOLS.CLEARFLOOR.DELETED);
            _customChoreTypeGo.transform.Find("Content/Clear")
                .gameObject.GetComponent<Button>().onClick.AddListener(DeleteSetting);
            _customChoreTypeGo.gameObject.AddComponent<CustomChoreTypeScreenEvent>();
            
            _deleteButton = _customChoreTypeGo.transform.Find("Content/Clear").gameObject;
            _deleteButton.SetActive(false);
            choreGroupPrefab.gameObject.SetActive(false);

            foreach (var choreGroup in Db.Get().ChoreGroups.resources) {
                var choreGroupUIGo =
                    Util.KInstantiateUI(choreGroupPrefab, choreGroupContainer);
                var choreGroupEntry = choreGroupUIGo.AddOrGet<ChoreGroupEntry>();
                choreGroupEntry.selected = false;
                choreGroupEntry.ChoreGroup = choreGroup;

                choreGroupUIGo.transform.Find("Image").gameObject.GetComponent<Image>().sprite =
                    Assets.GetSprite(choreGroup.sprite);
                var toggle = choreGroupUIGo.GetComponent<Toggle>();
                choreGroupUIGo.AddComponent<ToolTip>().toolTip = choreGroup.Name;
                toggle.image.color = Color.white;
                toggle.isOn = false;
                toggle.onValueChanged.AddListener((value) => {
                    toggle.isOn = value;
                    if (value) {
                        choreGroupEntry.selected = true;
                        toggle.image.color = Color.green;
                    }
                    else {
                        choreGroupEntry.selected = false;
                        toggle.image.color = Color.white;
                    }
                });
                choreGroupEntry.toggle = toggle;
                choreGroupUIGo.SetActive(true);
                ChoreGroupBind.Add(choreGroup.Id, choreGroupEntry);
            }
        }

        public static void ApplyChoreGroup(string choreTypeId, ChoreGroup[] groups) {
            Traverse.Create(Db.Get().ChoreTypes.TryGet(choreTypeId)).Property("groups")
                .SetValue(groups);
        }

        private static void DeleteSetting() {
            Mod.Changes.Remove(_targetChoreType.Id);
            ApplyChoreGroup(_targetChoreType.Id, Mod.Backup[_targetChoreType.Id]
                .Select(id => Db.Get().ChoreGroups.TryGet(id)).ToArray());
            Mod.Backup.Remove(_targetChoreType.Id);
            Hide();
        }
        
        private static void ApplySetting() {
            var choreGroups = new List<ChoreGroup>();
            foreach (var choreGroupPair in ChoreGroupBind) {
                if (!choreGroupPair.Value.selected) continue;
                choreGroups.Add(choreGroupPair.Value.ChoreGroup);
            }
            Mod.Changes[_targetChoreType.Id] =  choreGroups.Select(group => group.Id).ToArray();
            var data = JsonConvert.SerializeObject(Mod.Changes);
            
            try {
                File.WriteAllText(Mod.ConfigPath, data);
            }
            catch (Exception e) {
                Debug.LogError($"Custom chore types config file could not be saved: {e}");
                throw;
            }
            Mod.Backup[_targetChoreType.Id] = _targetChoreType.groups.Select(g => g.Id).ToArray();
            ApplyChoreGroup(_targetChoreType.Id, choreGroups.ToArray());
            Hide();
        }
        
        private static void GetParent() {
            var frontEndManager = FrontEndManager.Instance;
            if (frontEndManager != null) {
                _parent = frontEndManager.gameObject;
                return;
            }
            
            var gameScreenManager = GameScreenManager.Instance;
            if (gameScreenManager != null) {
                _parent = gameScreenManager.ssOverlayCanvas;
                return;
            }
            Debug.LogWarning("Can't initialize dialog game screen");
        }
    }
}