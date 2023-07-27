using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LuckyChallenge {
    public class God {
        public static void RandomInAllElement(int cell, int num) {
            System.Random random = new System.Random();
            cell = Grid.OffsetCell(cell, 0, 1);
            int count = num;
            while (count > 0) {
                KeyValuePair<Tag, Element> keyValuePair = ElementLoader.elementTagTable.ElementAt(random.Next(0, ElementLoader.elementTagTable.Count));
                float temperature = 290f;
                if (temperature < keyValuePair.Value.lowTemp && keyValuePair.Value.lowTempTransition.IsSolid || !IsValideElement(keyValuePair.Key)) continue;
                GameObject go = keyValuePair.Value.substance.SpawnResource(Grid.CellToPosCBC(cell, Grid.SceneLayer.Ore), random.Next(100, 300), temperature, byte.MaxValue, 0);
                if (GameComps.Fallers.Has((object)go)) GameComps.Fallers.Remove(go);
                Vector2 initial_velocity = new Vector2(Random.Range(-2f, 2f) , Random.Range(3f, 6f));
                GameComps.Fallers.Add(go, initial_velocity);
                count--;
            }
        }

        public static int[] GenerateRandomArray(int min, int max, int length) {
            System.Random random = new System.Random();
            int[] array = new int[length];
            for (int i = 0; i < length; i++) {
                array[i] = random.Next(min, max);
            }
            return array;
        }

        public static bool IsValideElement(Tag elem) {
            Tag[] tags = { new Tag("Vacuum") , new Tag("COMPOSITION") , new Tag("Void") };
            foreach (Tag tag in tags) {
                if (elem == tag) return false;
            }
            return true;
        }
    }
}
