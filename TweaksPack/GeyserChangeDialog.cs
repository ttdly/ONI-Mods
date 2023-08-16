using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using TweaksPack.Tweakable;
using UnityEngine;

namespace TweaksPack
{
    public class GeyserChangeDialog {
        public static List<EnumOption> Options = new List<EnumOption>(0);
        public static readonly RectOffset LABEL_MARGIN = new RectOffset(0, 20, 5, 5);
        public GameObject windowObject = new GameObject();
        public GameObject geyser;
        private EnumOption chosen;
        private Tag originTag;
        private readonly List<FloatOptionsEntry> modiferFloatOptionsEntrys = new List<FloatOptionsEntry>(0);
        private bool IsChangedGeyser {
            get {
                return originTag != (Tag)chosen.Value;
            }
        }

        public GeyserChangeDialog(GameObject gyeserObject) {
            geyser = gyeserObject;
            PDialog dialog = new PDialog("GeyserChangeWindow") {
                Title = TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.WINDOW_TITLE,
            };
            PLabel windowTitle = new PLabel();
            dialog.DialogClosed = _ => DestroyWindow();
            dialog.Body.AddChild(windowTitle);
            PPanel scrollBody = new PPanel("ScrollBody") {
                Spacing = 20,
                Direction = PanelDirection.Vertical,
                Alignment = TextAnchor.UpperCenter,
                FlexSize = Vector2.right
            };
            PScrollPane scrollPane = new PScrollPane("MainPanel") {
                ScrollHorizontal = false,
                ScrollVertical = true,
                Child = scrollBody,
                FlexSize = Vector2.right,
                TrackSize = 8,
                AlwaysShowHorizontal = false,
                AlwaysShowVertical = false
            };
            PPanel confirmPanel = new PPanel("ConfirmBody") {
                Margin = new RectOffset(0, 0, 10, 0),
                FlexSize = Vector2.left,
                DynamicSize = false
            };
            PButton confirmButton = new PButton("ConfirmButton") {
                Text = TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CONFIRM_BUTTON.TITLE,
                ToolTip = TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CONFIRM_BUTTON.TOOL_TIP,
                TextStyle = PUITuning.Fonts.UILightStyle.DeriveStyle(style: TMPro.FontStyles.Bold),
                OnClick = _ => ChangeGeyser(),
                FlexSize = new Vector2(0.8f, 1f),
                Margin = new RectOffset(10, 10, 13, 13),
            };
            confirmPanel.AddChild(confirmButton);
            CreateSelecteOption(scrollBody);
            CreateModiferOptions(scrollBody);
            dialog.Body.AddChild(scrollPane);
            dialog.Body.AddChild(confirmPanel);
            windowObject = dialog.Build();
        }

        private void SwitchTag(GameObject _, EnumOption selected) {
            if (selected != null) {
                chosen = selected;
            }
        }

        public void CreateSelecteOption(PPanel parent) {
            EnumOption defaultOption = null;
            int maxLen = 0;
            foreach (var option in Options) {
                if (geyser.GetComponent<KPrefabID>().HasTag((Tag)option.Value)) {
                    defaultOption = option;
                    originTag = (Tag)option.Value;
                    chosen = option;
                }
                int len = option.Title?.Trim()?.Length ?? 0;
                if (len > maxLen)
                    maxLen = len;
            }
            PPanel panel = new PPanel("SelectOption") {
                Alignment = TextAnchor.MiddleCenter,
                Direction = PanelDirection.Horizontal,
            };
            PLabel label = new PLabel("SelectOptionLabel") {
                Text = TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CHANGE_LABEL,
                TextAlignment = TextAnchor.MiddleCenter,
                Margin = LABEL_MARGIN,
            };
            PComboBox<EnumOption> combo = new PComboBox<EnumOption>() {
                Content = Options,
                OnOptionSelected = SwitchTag,
                BackColor = PUITuning.Colors.ButtonPinkStyle,
                InitialItem = defaultOption,
                EntryColor = PUITuning.Colors.ButtonBlueStyle,
                TextStyle = PUITuning.Fonts.TextLightStyle,
            }.SetMinWidthInCharacters(maxLen);
            panel.AddChild(label);
            panel.AddChild(combo);
            parent.AddChild(panel);
        }

        public void CreateModiferOptions(PPanel parent) {
            Dictionary<string, LimitAttribute> modifers = new Dictionary<string, LimitAttribute>() {
                {"mass", new LimitAttribute(0f, 1f) },
                {"temperature", new LimitAttribute(0f, 30f) },
                {"pressure", new LimitAttribute(0f, 100F) }
            };
            foreach (KeyValuePair<string, LimitAttribute> pair in modifers) {
                int row = 0;
                PGridPanel panel = new PGridPanel("Grid" + pair.Key) {
                    Margin = new RectOffset(10, 0, 0, 0),
                    FlexSize = Vector2.left,
                }.AddColumn(new GridColumnSpec()).AddColumn(new GridColumnSpec(0, 1));
                panel.AddRow(new GridRowSpec(0, 1));
                FloatOptionsEntry entry = new FloatOptionsEntry(pair.Key, new OptionAttribute(Strings.Get("TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CHANGE_MODIFI." + pair.Key.ToUpper() + "_NAME"), Strings.Get("TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CHANGE_MODIFI." + pair.Key.ToUpper() + "_TIP")), pair.Value);
                entry.CreateUIEntry(panel, ref row);
                parent.AddChild(panel);
                modiferFloatOptionsEntrys.Add(entry);
            }
        }

        public void ChangeGeyser() {
            if (IsChangedGeyser) {
                GameObject go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)chosen.Value), Grid.SceneLayer.Building);
                Vector3 posCbc = Grid.CellToPosCBC(Grid.PosToCell(geyser), Grid.SceneLayer.Building);
                float num = -0.15f;
                posCbc.z += num;
                go.transform.SetPosition(posCbc);
                go.SetActive(true);
                UnityEngine.Object.Destroy(geyser);
                geyser = null;
                geyser = go;
                CameraController.Instance?.CameraGoTo(geyser.transform.GetPosition());
            } else {
                Geyser.GeyserModification geyserModification = new Geyser.GeyserModification() {
                    massPerCycleModifier = (float)modiferFloatOptionsEntrys[0].Value,
                    temperatureModifier = (float)modiferFloatOptionsEntrys[1].Value,
                    iterationDurationModifier = 0.0f,
                    iterationPercentageModifier = 0.0f,
                    yearDurationModifier = 0.0f,
                    yearPercentageModifier = (float)modiferFloatOptionsEntrys[2].Value,
                };
                GeyserTweakable geyserTweakable = geyser.GetComponent<GeyserTweakable>();
                geyserTweakable.modification = geyserModification;
                geyserTweakable.hasModification = true;
                geyser.GetComponent<Geyser>()?.AddModification(geyserModification);
            }
            DestroyWindow();
        }

        public void DestroyWindow() {
            SpeedControlScreen.Instance?.Unpause(true);
            if (!(CameraController.Instance != null)) return;
            CameraController.Instance.DisableUserCameraControl = false;
            windowObject.DeleteObject();
            geyser.GetComponent<GeyserTweakable>()?.Refresh();
        }
    }

    public class EnumOption : ITooltipListableOption {
        public string Title { get; }
        public string ToolTip { get; }
        public object Value { get; }

        public EnumOption(string title, string toolTip, object value) {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            ToolTip = toolTip;
            Value = value;
        }

        public string GetProperName() {
            return Title;
        }

        public string GetToolTipText() {
            return ToolTip;
        }

        public object GetValue() {
            return Value;
        }
    }
}
