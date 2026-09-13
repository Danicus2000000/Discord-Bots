using DSharpPlus;
using Lavalink4NET;
using System;
using System.Collections.Concurrent;

namespace DiscordBots
{
    internal static class LavalinkAudioServices
    {
        private static readonly ConcurrentDictionary<DiscordClient, IAudioService> Services = new();

        public static void Register(DiscordClient client, IAudioService audioService)
        {
            Services[client] = audioService;
        }

        public static IAudioService Get(DiscordClient client)
        {
            if (Services.TryGetValue(client, out var audioService))
                return audioService;

            throw new InvalidOperationException("No Lavalink4NET audio service is registered for this Discord client.");
        }
    }
}
