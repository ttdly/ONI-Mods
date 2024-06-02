using TUNING;
using UnityEngine;

public class SongMachineConfig : IBuildingConfig {
  public const string ID = "SongMachine";
  private const int RANGEX = 4;
  private const int RANGEY = 0;
  private readonly float[] mass = new float[1] { 200f };
  private readonly string[] materials = new string[1] { "Special" };

  public override BuildingDef CreateBuildingDef() {
    var buildingDef = BuildingTemplates.CreateBuildingDef("SongMachine", 1, 1, "song_machine_kanim", 30, 30f, mass,
      materials, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER4);

    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.Entombable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.PermittedRotations = PermittedRotations.Unrotatable;
    buildingDef.AlwaysOperational = true;

    return buildingDef;
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go) {
    AddVisualizer(go, true);
  }

  public override void DoPostConfigureComplete(GameObject go) {
    var overrider = go.AddOrGet<SongMachine>();
    overrider.wasOn = false;
    overrider.detectionRangeY = RANGEY;
    overrider.detectionRangeX = RANGEX;
    AddVisualizer(go, false);
  }

  private static void AddVisualizer(GameObject prefab, bool movable) {
    var rangeVisualizer = prefab.AddOrGet<RangeVisualizer>();
    rangeVisualizer.OriginOffset = new Vector2I(0, 0);
    rangeVisualizer.RangeMin.x = -(RANGEX / 2);
    rangeVisualizer.RangeMin.y = 0;
    rangeVisualizer.RangeMax.x = RANGEX / 2;
    rangeVisualizer.RangeMax.y = RANGEY;
    rangeVisualizer.BlockingTileVisible = true;
  }

  public override void DoPostConfigureUnderConstruction(GameObject go) {
    base.DoPostConfigureUnderConstruction(go);
    go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
  }
}