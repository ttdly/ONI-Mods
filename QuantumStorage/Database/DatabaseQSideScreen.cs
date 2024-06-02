using System;
using System.Collections.Generic;
using PeterHan.PLib.Core;
using PeterHan.PLib.Detours;
using QuantumStorage;
using QuantumStorage.Database;
using UnityEngine;
using UnityEngine.UI;

namespace ChangeBlueprints {
  internal class DatabaseQSideScreen : SideScreenContent {
    private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> APPLY_BUTTON =
      PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(applyButton));

    private static readonly IDetouredField<ArtableSelectionSideScreen, KButton> CLEAR_BUTTON =
      PDetours.DetourField<ArtableSelectionSideScreen, KButton>(nameof(clearButton));

    private static readonly IDetouredField<ArtableSelectionSideScreen, GameObject> STATE_BUTTON_PREFAB =
      PDetours.DetourField<ArtableSelectionSideScreen, GameObject>(nameof(stateButtonPrefab));

    private static readonly IDetouredField<ArtableSelectionSideScreen, RectTransform> BUTTON_CONTAINER =
      PDetours.DetourField<ArtableSelectionSideScreen, RectTransform>(nameof(buttonContainer));

    private static readonly IDetouredField<ArtableSelectionSideScreen, RectTransform> SCROLL =
      PDetours.DetourField<ArtableSelectionSideScreen, RectTransform>(nameof(scrollTransoform));

    public GameObject stateButtonPrefab;

    [SerializeField] private RectTransform buttonContainer;

    private readonly List<MultiToggle> buttons = new List<MultiToggle>();
    private KButton applyButton;
    private KButton clearButton;
    private DatabaseQ database;
    private RectTransform scrollTransoform;

    protected override void OnSpawn() {
      base.OnSpawn();
    }

    public override bool IsValidForTarget(GameObject target) {
      return target != null && target.TryGetComponent(out DatabaseQ _);
    }

    public override void SetTarget(GameObject target) {
      if (target == null) PUtil.LogDebug("Target null");
      database = target.GetComponent<DatabaseQ>();
      GenerateStateButtons();
    }

    public override string GetTitle() {
      return ModString.UI.DatabaseQSideScreen.TITLE;
    }

    public void GenerateStateButtons() {
      foreach (var button in buttons) Util.KDestroyGameObject(button.gameObject);
      buttons.Clear();
      foreach (var item in database.itemDic) {
        var obj = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);
        var sprite = Def.GetUISprite(item.Key);
        var component = obj.GetComponent<MultiToggle>();
        component.GetComponent<ToolTip>()
          .SetSimpleTooltip(
            $"{Assets.GetPrefab(item.Key).GetProperName()}\n{GameUtil.GetFormattedMass((float)item.Value)}");
        var image = component.GetComponent<HierarchyReferences>().GetReference<Image>("Icon");
        image.sprite = sprite.first;
        image.color = sprite.second;
        component.onClick = delegate { };
        buttons.Add(component);
      }
    }

    private static DatabaseQSideScreen CreateScreen(ArtableSelectionSideScreen sideScreen) {
      var template = Instantiate(sideScreen.gameObject);
      template.name = nameof(DatabaseQSideScreen);
      var active = template.activeSelf;
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
        containerLayout.cellSize = new Vector2(46, 46);
        containerLayout.constraintCount = 5;
        containerLayout.spacing = new Vector2(4, 4);
      }

      if (stateButtonPrefabLayout != null) {
        stateButtonPrefabLayout.minHeight = 46;
        stateButtonPrefabLayout.minWidth = 46;
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

    public static void AddSideScreen(IList<DetailsScreen.SideScreenRef> existing, GameObject parent) {
      var found = false;
      foreach (var ssRef in existing)
        if (ssRef.screenPrefab is ArtableSelectionSideScreen sideScreen) {
          var newScreen = new DetailsScreen.SideScreenRef();
          var ours = CreateScreen(sideScreen);
          found = true;
          newScreen.name = nameof(DatabaseQSideScreen);
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