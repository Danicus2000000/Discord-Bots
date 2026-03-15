using Newtonsoft.Json;

namespace Simp
{
    public struct ConfigJson
    {
        [JsonProperty(nameof(Token))]//gets token from json file sets it to store in token
        public string Token { get; private set; }
        [JsonProperty(nameof(CommandPrefix))]//gets prefix from file stores it to prefix
        public string CommandPrefix { get; private set; }
    }
}
