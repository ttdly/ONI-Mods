using System.Collections.Generic;
using KSerialization;
using PeterHan.PLib.Core;

namespace QuantumStorage.Database {
  [SerializationConfig(MemberSerialization.OptIn)]
  public class DatabaseQ : KMonoBehaviour {
    [Serialize] public float tempHeat;

    [MyCmpGet] public Operational operational;

    private readonly float maxTemp = 50000f;

    [Serialize] public Dictionary<Tag, double> itemDic = new Dictionary<Tag, double>();

    #region LifeCycle

    protected override void OnSpawn() {
      base.OnSpawn();
      StaticVar.database = this;
    }

    protected override void OnCleanUp() {
      base.OnCleanUp();
      StaticVar.database = null;
    }

    #endregion

    #region UpdateItems

    public void UpdateItems(Tag tag, double amount, float transferHeat) {
      PUtil.LogDebug($"{tag}:{amount}:{transferHeat}");
      if (itemDic.ContainsKey(tag)) {
        var buffer = itemDic[tag] + amount;
        itemDic[tag] = buffer;
      } else {
        itemDic.Add(tag, amount);
      }

      operational.SetActive(operational.IsOperational);
      ApplyHeat(transferHeat);
    }

    public void ApplyHeat(float heat) {
      tempHeat += heat;
      if (tempHeat > maxTemp) {
        var count = tempHeat - maxTemp;
        SimMessages.ModifyEnergy(Grid.PosToCell(gameObject), count, 10000f, SimMessages.EnergySourceID.Overheatable);
      }
    }

    #endregion
  }
}