using System;
using System.Collections.Generic;
using PeterHan.PLib.Core;

namespace SyntheticEvolution {
  public class Evolution : KMonoBehaviour {
    
    
    private static readonly Dictionary<string, string> ConvertDict = new Dictionary<string, string>() {
      { "AMARI", "GIZMO" },
      { "MEEP", "CHIP" },
      { "ARI", "EDWIREDO" },
      { "NAILS", "SONYAR" },
      { "PEI", "STEELA" }
    };

    private static readonly EventSystem.IntraObjectHandler<Evolution> OnRefreshUserMenuDelegate =
      new EventSystem.IntraObjectHandler<Evolution>((component, data) => component.OnRefreshUserMenu(data));

    private string button_name = "Synthetic Evolution";
    private string button_tooltip = "Transform deceased Duplicant into Bionic Duplicant.";

    protected override void OnPrefabInit() {
      base.OnPrefabInit();
      if (Localization.GetCurrentLanguageCode() != "zh_klei") return;
      button_name = "机械飞升";
      button_tooltip = "将死亡的复制人变为仿生复制人";
    }

    protected override void OnSpawn() {
      base.OnSpawn();
      Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
      Subscribe((int)GameHashes.StatusChange, OnRefreshUserMenuDelegate);
    }

    private void OnRefreshUserMenu(object _) {
#if DEBUG
      Game.Instance.userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", "杀死我",
        DoHarm
        , tooltipText: "???"));
#endif
      if(!SaveLoader.Instance.IsDLCActiveForCurrentSave("DLC3_ID")) return;
      if (!gameObject.HasTag(GameTags.Dead)) return;
      Game.Instance.userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", button_name,
        DoEvolution
        , tooltipText:button_tooltip));
    }

    private void DoHarm() {
      var health = gameObject.GetComponent<Health>();
      health.Damage(50f);
    }

    private void DoEvolution() {
      SpawnMinion(BionicMinionConfig.ID);
      gameObject.DeleteObject();
    }


    private void SpawnMinion(string prefabID) {
      var flag = true;
      var prefab = Assets.GetPrefab((Tag)prefabID);
      var model = (Tag)prefabID;
      var bionicMinion = Util.KInstantiate(prefab);
      bionicMinion.name = prefab.name;
      Immigration.Instance.ApplyDefaultPersonalPriorities(bionicMinion);
      var posCbc = Grid.CellToPosCBC(Grid.PosToCell(gameObject.transform.position), Grid.SceneLayer.Move);
      bionicMinion.transform.SetLocalPosition(posCbc);
      bionicMinion.SetActive(true);
      ConvertDict.TryGetValue(gameObject.GetComponent<MinionIdentity>().nameStringKey, out var convertValue);
      PUtil.LogDebug($"Convert {convertValue} to {gameObject.GetComponent<MinionIdentity>().nameStringKey}");
      if (convertValue != null) {
        PUtil.LogDebug($"ConvertValue: {convertValue}");
        var personality = Db.Get().Personalities.GetPersonalityFromNameStringKey(convertValue);
        if (personality != null) {
          new MinionStartingStats(personality).Apply(bionicMinion);
          flag = false;
        }
      }
      if (flag) {
        new MinionStartingStats(model, false).Apply(bionicMinion);
      }
      bionicMinion.GetMyWorld().SetDupeVisited();
    }
  }
}