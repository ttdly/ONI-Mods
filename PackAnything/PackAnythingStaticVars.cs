using System.Collections.Generic;
using System.Reflection;
using Database;
using PeterHan.PLib.UI;
using UnityEngine;

namespace PackAnything {
  public class MoveStatus {
    public bool HaveAnObjectMoving;
    public ObjectCanMove watingMoveObject;
    public WorldModifier worldModifier;

    public MoveStatus() {
      watingMoveObject = null;
      worldModifier = null;
    }
  }

  public class PackAnythingStaticVars {
    public static HashSet<ObjectCanMove> SurveableCmps;
    public static Sprite ToolIcon;
    public static MoveStatus MoveStatus;
    public static bool IsDlc;

    public static void Init() {
      SurveableCmps = new HashSet<ObjectCanMove>();
      ToolIcon = PUIUtils.LoadSprite("PackAnything.images.tooIcon.png");
      MoveStatus = new MoveStatus();
      IsDlc = DlcManager.GetActiveDLCIds().Count > 0 && DlcManager.GetActiveDLCIds().Contains("EXPANSION1_ID");
    }

    public static void SetMoving(bool isMoving) {
      MoveStatus.HaveAnObjectMoving = isMoving;
    }

    public static void SetTargetObjectCanMove(ObjectCanMove objectCanMove) {
      MoveStatus.watingMoveObject = objectCanMove;
    }

    public static void SetTargetModifier(WorldModifier worldModifier) {
      MoveStatus.worldModifier = worldModifier;
    }
  }
}