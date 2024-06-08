using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace PackAnything {
  [RestartRequired]
  [JsonObject(MemberSerialization.OptIn)]
  public class Options {
    [Option("STRINGS.Options.GENERATE_UNOBTANIUM", "STRINGS.Options.GENERATE_UNOBTANIUM_DESC")]
    [JsonProperty]
    public bool GenerateUnobtanium { get; set; } = true;

    [Option("STRINGS.Options.TOGGLE_GEYSER_NUM", "STRINGS.Options.TOGGLE_GEYSER_NUM_DESC")]
    [JsonProperty]
    public bool ToggleGeyserAttribute { get; set; } = true;
  }
}