using Newtonsoft.Json;

namespace Snakebot
{
    public struct ConfigJson
    {
        [JsonProperty(nameof(Token))]//gets token from json file sets it to store in token
        public string Token { get; private set; }
    }
}
