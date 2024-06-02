using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace PackAnything {
  [RestartRequired]
  [JsonObject(MemberSerialization.OptIn)]
  public class Options {
    [Option("STRINGS.GENERATE_UNOBTANIUM", "STRINGS.GENERATE_UNOBTANIUM_DESC")]
    [JsonProperty]
    public bool GenerateUnobtanium { get; set; } = true;
  }
}