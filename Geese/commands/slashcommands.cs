using DiscordBots;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Geese.commands
{
    internal class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("logout", "Shuts down the bot")]
        [SlashRequireUserPermissions(Permissions.Administrator)]
        public static async Task Logout(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("Logging out all bots...");

            var failures = new List<string>();
            foreach (var client in Program.Clients)
            {
                try
                {
                    await client.DisconnectAsync();
                }
                catch (Exception ex)
                {
                    failures.Add($"{client.CurrentUser?.Username ?? "Unknown bot"}: {ex.GetType().Name}: {ex.Message}");
                }
            }

            if (failures.Count > 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
                    $"Some bots could not be logged out:\n{string.Join("\n", failures)}"));
            }
            Environment.Exit(0);
        }

        [SlashCommand("flock", "flocks and honks")]
        public static async Task Play(InteractionContext ctx)
        {
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await ctx.CreateResponseAsync("You need to be in a voice channel in order for the bot to connect!");
                return;
            }

            try
            {
                await ctx.CreateResponseAsync("Flocking all bots...");

                var failures = new List<string>();
                foreach (var client in Program.Clients)
                {
                    try
                    {
                        var player = await LavalinkAudioServices.Get(client).Players.JoinAsync(
                            ctx.Guild.Id,
                            channel.Id,
                            PlayerFactory.Queued,
                            new QueuedLavalinkPlayerOptions(),
                            CancellationToken.None);
                        await player.PlayAsync(
                            new Uri("https://www.dropbox.com/scl/fi/4z1bit7hqtahkd5eal9wu/HONK.mp3?rlkey=8mgc54j83gmdiw2vli1t5l455&st=fqpin5cr&raw=1"),
                            enqueue: false,
                            cancellationToken: CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        failures.Add($"{client.CurrentUser?.Username ?? "Unknown bot"}: {ex.GetType().Name}: {ex.Message}");
                    }
                }

                var message = failures.Count == 0
                    ? "Flock Completed"
                    : $"Flock completed with {failures.Count} failure(s):\n{string.Join("\n", failures)}";
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(message));
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
                    $"An exception occurred during playback: `{ex.GetType()}: {ex.Message}`"));
            }
        }

        [SlashCommand("deflock", "Leaves the voice channel")]
        public static async Task Leave(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("Deflocking all bots...");

            var disconnected = 0;
            var failures = new List<string>();
            foreach (var client in Program.Clients)
            {
                try
                {
                    if (LavalinkAudioServices.Get(client).Players.TryGetPlayer(ctx.Guild.Id, out ILavalinkPlayer player))
                    {
                        await player.DisconnectAsync();
                        disconnected++;
                    }
                }
                catch (Exception ex)
                {
                    failures.Add($"{client.CurrentUser?.Username ?? "Unknown bot"}: {ex.GetType().Name}: {ex.Message}");
                }
            }

            var message = failures.Count == 0
                ? $"Deflocked {disconnected} bot(s)."
                : $"Deflocked {disconnected} bot(s) with {failures.Count} failure(s):\n{string.Join("\n", failures)}";
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(message));
        }
    }
}
