﻿using System;
using PackAnything.MoveTool;
using STRINGS;

namespace PackAnything.Movable {
  public class BaseMovable : KMonoBehaviour {
    private static readonly EventSystem.IntraObjectHandler<BaseMovable> OnRefreshUserMenuDelegate =
      new EventSystem.IntraObjectHandler<BaseMovable>((component, data) =>
        component.OnRefreshUserMenu(data));

    public bool canCrossMove = true;

    protected override void OnSpawn() {
      base.OnSpawn();
      Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
      Subscribe((int)GameHashes.StatusChange, OnRefreshUserMenuDelegate);
    }

    private void OnRefreshUserMenu(object _) {
      if (gameObject.HasTag("OilWell") &&
          gameObject.GetComponent<BuildingAttachPoint>()?.points[0].attachedBuilding != null) return;

      Game.Instance.userMenu.AddButton(
        gameObject,
        new KIconButtonMenu.ButtonInfo("action_control", UI.USERMENUACTIONS.PICKUPABLEMOVE.NAME, OnClickMove,
          tooltipText: UI.USERMENUACTIONS.PICKUPABLEMOVE.TOOLTIP));
    }

    protected virtual void OnClickMove() {
      EntityMoveTool.Instance.Activate(this);
    }

    public bool CanMoveTo(int targetCell) {
      if (!canCrossMove && Grid.WorldIdx[targetCell] != gameObject.GetMyWorldId()) return false;
      try {
        if (!Grid.IsValidCell(targetCell)) return false;
        return !Grid.Element[targetCell].IsSolid;
      } catch (Exception) {
        return false;
      }
    }

    public virtual void Move(int targetCell) {
    }
  }
}