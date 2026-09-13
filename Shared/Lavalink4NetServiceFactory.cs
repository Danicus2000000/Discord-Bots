using DSharpPlus;
using Lavalink4NET;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBots
{
    internal sealed class Lavalink4NetService
    {
        private readonly ServiceProvider _services;

        public Lavalink4NetService(ServiceProvider services, IAudioService audioService)
        {
            _services = services;
            AudioService = audioService;
        }

        public IAudioService AudioService { get; }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await AudioService.StartAsync(cancellationToken).ConfigureAwait(false);
            await AudioService.WaitForReadyAsync(cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await AudioService.DisposeAsync().ConfigureAwait(false);
            await _services.DisposeAsync().ConfigureAwait(false);
        }
    }

    internal static class Lavalink4NetServiceFactory
    {
        public static Lavalink4NetService Create(
            DiscordClient discordClient,
            Uri baseAddress,
            Uri webSocketUri,
            string passphrase,
            string label)
        {
            var services = new ServiceCollection();
            services.AddSingleton(discordClient);
            services.AddLavalink<DiscordClientWrapper>();
            services.ConfigureLavalink(options =>
            {
                options.BaseAddress = baseAddress;
                options.WebSocketUri = webSocketUri;
                options.Passphrase = passphrase;
                options.Label = label;
            });

            var serviceProvider = services.BuildServiceProvider();
            var audioService = serviceProvider.GetRequiredService<IAudioService>();
            return new Lavalink4NetService(serviceProvider, audioService);
        }

        public static Uri GetWebSocketUri(Uri endpoint)
        {
            var builder = new UriBuilder(endpoint);
            var path = builder.Path.TrimEnd('/');
            if (!path.EndsWith("/v4/websocket", StringComparison.OrdinalIgnoreCase))
                builder.Path = string.IsNullOrEmpty(path) ? "/v4/websocket" : $"{path}/v4/websocket";

            return builder.Uri;
        }
    }
}
