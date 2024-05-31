using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace FragmentThermostat {
  [RestartRequired, JsonObject(MemberSerialization.OptIn)]
  public class ModOptions {
    [Option("STRINGS.UI_FTMOD.Options.MAX_TEMP")]
    [JsonProperty]
    public float MinTemperature { get; set; } = -20;

    [Option("STRINGS.UI_FTMOD.Options.MIN_TEMP")]
    [JsonProperty]
    public float MaxTemperature { get; set; } = 30;

    [Option("STRINGS.UI_FTMOD.Options.MODE_OPEN", "STRINGS.UI_FTMOD.Options.MODE_TIP",
      "STRINGS.UI_FTMOD.Options.MODE2")]
    [JsonProperty]
    public bool Mode { get; set; } = false;

    [Option("STRINGS.UI_FTMOD.Options.MIN_TEMP", "STRINGS.UI_FTMOD.Options.MODE_TIP2",
      "STRINGS.UI_FTMOD.Options.MODE2")]
    [JsonProperty]
    public float Mode2Temp { get; set; } = 10;
  }
}