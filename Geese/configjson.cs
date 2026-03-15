using Newtonsoft.Json;

namespace Geese
{
    public struct ConfigJson
    {
        [JsonProperty(nameof(Token1))]//gets token from json file sets it to store in token
        public string Token1 { get; private set; }
        [JsonProperty(nameof(Token2))]//gets token from json file sets it to store in token
        public string Token2 { get; private set; }
        [JsonProperty(nameof(Token3))]//gets token from json file sets it to store in token
        public string Token3 { get; private set; }
        [JsonProperty(nameof(Token4))]//gets token from json file sets it to store in token
        public string Token4 { get; private set; }
        [JsonProperty(nameof(Token5))]//gets token from json file sets it to store in token
        public string Token5 { get; private set; }
        [JsonProperty(nameof(Token6))]//gets token from json file sets it to store in token
        public string Token6 { get; private set; }
    }
}
