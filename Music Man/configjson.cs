using Newtonsoft.Json;

namespace Music_Man
{
    public struct ConfigJson
    {
        [JsonProperty(nameof(Token))]//gets token from json file sets it to store in token
        public string Token { get; private set; }

        [JsonProperty(nameof(Lavalink))]
        public LavalinkConfig Lavalink { get; private set; }
    }

    public sealed class LavalinkConfig
    {
        [JsonProperty(nameof(RestEndpoint))]
        public string RestEndpoint { get; private set; }

        [JsonProperty(nameof(SocketEndpoint))]
        public string SocketEndpoint { get; private set; }

        [JsonProperty(nameof(Password))]
        public string Password { get; private set; }
    }
}
