using System;
using System.Collections.Generic;
using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using TMPro;
using TweaksPack.Tweakable;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TweaksPack {
  public class GeyserChangeDialog {
    public static List<EnumOption> Options = new List<EnumOption>(0);
    public static readonly RectOffset LABEL_MARGIN = new RectOffset(0, 20, 5, 5);
    private readonly List<FloatOptionsEntry> modiferFloatOptionsEntrys = new List<FloatOptionsEntry>(0);
    private EnumOption chosen;
    public GameObject geyser;
    private Tag originTag;
    public GameObject windowObject = new GameObject();

    public GeyserChangeDialog(GameObject gyeserObject) {
      geyser = gyeserObject;
      var dialog = new PDialog("GeyserChangeWindow") {
        Title = TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.WINDOW_TITLE
      };
      var windowTitle = new PLabel();
      dialog.DialogClosed = _ => DestroyWindow();
      dialog.Body.AddChild(windowTitle);
      var scrollBody = new PPanel("ScrollBody") {
        Spacing = 20,
        Direction = PanelDirection.Vertical,
        Alignment = TextAnchor.UpperCenter,
        FlexSize = Vector2.right
      };
      var scrollPane = new PScrollPane("MainPanel") {
        ScrollHorizontal = false,
        ScrollVertical = true,
        Child = scrollBody,
        FlexSize = Vector2.right,
        TrackSize = 8,
        AlwaysShowHorizontal = false,
        AlwaysShowVertical = false
      };
      var confirmPanel = new PPanel("ConfirmBody") {
        Margin = new RectOffset(0, 0, 10, 0),
        FlexSize = Vector2.left,
        DynamicSize = false
      };
      var confirmButton = new PButton("ConfirmButton") {
        Text = TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CONFIRM_BUTTON.TITLE,
        ToolTip = TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CONFIRM_BUTTON.TOOL_TIP,
        TextStyle = PUITuning.Fonts.UILightStyle.DeriveStyle(style: FontStyles.Bold),
        OnClick = _ => ChangeGeyser(),
        FlexSize = new Vector2(0.8f, 1f),
        Margin = new RectOffset(10, 10, 13, 13)
      };
      confirmPanel.AddChild(confirmButton);
      CreateSelecteOption(scrollBody);
#if DEBUG
            CreateModiferOptions(scrollBody);
#endif
      dialog.Body.AddChild(scrollPane);
      dialog.Body.AddChild(confirmPanel);
      windowObject = dialog.Build();
    }

    private bool IsChangedGeyser => originTag != (Tag)chosen.Value;

    private void SwitchTag(GameObject _, EnumOption selected) {
      if (selected != null) chosen = selected;
    }

    public void CreateSelecteOption(PPanel parent) {
      EnumOption defaultOption = null;
      var maxLen = 0;
      foreach (var option in Options) {
        if (geyser.GetComponent<KPrefabID>().HasTag((Tag)option.Value)) {
          defaultOption = option;
          originTag = (Tag)option.Value;
          chosen = option;
        }

        var len = option.Title?.Trim()?.Length ?? 0;
        if (len > maxLen)
          maxLen = len;
      }

      var panel = new PPanel("SelectOption") {
        Alignment = TextAnchor.MiddleCenter,
        Direction = PanelDirection.Horizontal
      };
      var label = new PLabel("SelectOptionLabel") {
        Text = TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CHANGE_LABEL,
        TextAlignment = TextAnchor.MiddleCenter,
        Margin = LABEL_MARGIN
      };
      var combo = new PComboBox<EnumOption> {
        Content = Options,
        OnOptionSelected = SwitchTag,
        BackColor = PUITuning.Colors.ButtonPinkStyle,
        InitialItem = defaultOption,
        EntryColor = PUITuning.Colors.ButtonBlueStyle,
        TextStyle = PUITuning.Fonts.TextLightStyle
      }.SetMinWidthInCharacters(maxLen);
      panel.AddChild(label);
      panel.AddChild(combo);
      parent.AddChild(panel);
    }

    public void CreateModiferOptions(PPanel parent) {
      var modifers = new Dictionary<string, LimitAttribute> {
        { "mass", new LimitAttribute(0f, 1f) },
        { "temperature", new LimitAttribute(0f, 30f) },
        { "pressure", new LimitAttribute(0f, 100F) }
      };
      foreach (var pair in modifers) {
        var row = 0;
        var panel = new PGridPanel("Grid" + pair.Key) {
          Margin = new RectOffset(10, 0, 0, 0),
          FlexSize = Vector2.left
        }.AddColumn(new GridColumnSpec()).AddColumn(new GridColumnSpec(0, 1));
        panel.AddRow(new GridRowSpec(0, 1));
        var entry = new FloatOptionsEntry(pair.Key,
          new OptionAttribute(
            Strings.Get("TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CHANGE_MODIFI." + pair.Key.ToUpper() + "_NAME"),
            Strings.Get("TweaksPackStrings.UI.CHANGE_GEYSER_WINDOW.CHANGE_MODIFI." + pair.Key.ToUpper() + "_TIP")),
          pair.Value);
        entry.CreateUIEntry(panel, ref row);
        parent.AddChild(panel);
        modiferFloatOptionsEntrys.Add(entry);
      }
    }

    public void ChangeGeyser() {
      if (IsChangedGeyser) {
        var go = GameUtil.KInstantiate(Assets.GetPrefab((Tag)chosen.Value), Grid.SceneLayer.Building);
        var posCbc = Grid.CellToPosCBC(Grid.PosToCell(geyser), Grid.SceneLayer.Building);
        var num = -0.15f;
        posCbc.z += num;
        go.transform.SetPosition(posCbc);
        go.SetActive(true);
        Object.Destroy(geyser);
        geyser = null;
        geyser = go;
        CameraController.Instance?.CameraGoTo(geyser.transform.GetPosition());
      }

      DestroyWindow();
    }

    public void DestroyWindow() {
      SpeedControlScreen.Instance?.Unpause();
      if (!(CameraController.Instance != null)) return;
      CameraController.Instance.DisableUserCameraControl = false;
      windowObject.DeleteObject();
      geyser.GetComponent<GeyserTweakable>()?.Refresh();
    }
  }

  public class EnumOption : ITooltipListableOption {
    public EnumOption(string title, string toolTip, object value) {
      Title = title ?? throw new ArgumentNullException(nameof(title));
      ToolTip = toolTip;
      Value = value;
    }

    public string Title { get; }
    public string ToolTip { get; }
    public object Value { get; }

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