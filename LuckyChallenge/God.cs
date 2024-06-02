using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace LuckyChallenge {
  public enum GiftType {
    Normal,
    Room,
    Creature,
    Food
  }

  public class God {
    public static void OpenTheGift(GiftType type, int cell = 0, int num = 0, Worker worker = null) {
      switch (type) {
        case GiftType.Normal:
          RandomInAllElement(cell, num);
          break;
        case GiftType.Room:
          RandomRoom(cell, worker);
          break;
      }
    }

    public static void RandomRoom(int cell, Worker worker) {
      var dic = "hq";
      var template = TemplateCache.GetTemplate(dic);
      if (template == null || template.cells == null) return;
      var workerCell = Grid.PosToCell(worker);
      var templateSize = template.info.size;
      var x = Mathf.FloorToInt((float)(-(double)templateSize.X / 2.0));
      var y = Mathf.FloorToInt((float)(-(double)templateSize.Y / 2.0));
      var cellTopLeft = Grid.OffsetCell(cell, x, y);
      cellTopLeft = Grid.CellLeft(cellTopLeft);
      var posCbc = Grid.CellToPosCBC(cellTopLeft, Grid.SceneLayer.Move);
      worker.transform.SetPosition(posCbc);
      var centerCell = Grid.OffsetCell(cell, 0, -y);
      TemplateLoader.Stamp(template, Grid.CellToXY(centerCell), () => TransWorker(worker, cell));
    }

    public static void TransWorker(Worker worker, int cell) {
      worker.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
    }

    public static void RandomInAllElement(int cell, int num) {
      var random = new Random();
      cell = Grid.OffsetCell(cell, 0, 1);
      var count = num;
      while (count > 0) {
        var keyValuePair = ElementLoader.elementTagTable.ElementAt(random.Next(0, ElementLoader.elementTagTable.Count));
        var temperature = 290f;
        if ((temperature < keyValuePair.Value.lowTemp && keyValuePair.Value.lowTempTransition.IsSolid) ||
            !IsValideElement(keyValuePair.Key)) continue;
        var go = keyValuePair.Value.substance.SpawnResource(Grid.CellToPosCBC(cell, Grid.SceneLayer.Ore),
          random.Next(100, 300), temperature, byte.MaxValue, 0);
        if (GameComps.Fallers.Has(go)) GameComps.Fallers.Remove(go);
        var initial_velocity = new Vector2(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(3f, 6f));
        GameComps.Fallers.Add(go, initial_velocity);
        count--;
      }
    }

    public static int[] GenerateRandomArray(int min, int max, int length) {
      var random = new Random();
      var array = new int[length];
      for (var i = 0; i < length; i++) array[i] = random.Next(min, max);
      return array;
    }

    public static bool IsValideElement(Tag elem) {
      Tag[] tags = { new Tag("Vacuum"), new Tag("COMPOSITION"), new Tag("Void") };
      foreach (var tag in tags)
        if (elem == tag)
          return false;
      return true;
    }
  }
}