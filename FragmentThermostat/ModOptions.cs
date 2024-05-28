using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace FragmentThermostat {
  [RestartRequired, JsonObject(MemberSerialization.OptIn)]
  public class ModOptions {
    
      [Option("MinTemperature")]
      [JsonProperty]
      public float MinTemperature { get; set; } = -20;

    [Option("MaxTemperature")]
    [JsonProperty]
    public float MaxTemperature { get; set; } = 30;
  }
}