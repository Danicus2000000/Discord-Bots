using DiscordBots;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Music_Man.commands
{
    internal class SlashCommands : ApplicationCommandModule
    {
        private const string NotConnectedMessage = "Not connected in this guild!";

        [SlashCommand("logout", "logs out the bot")]
        [SlashRequireUserPermissions(Permissions.Administrator)]
        public static async Task Logout(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("goodbye cruel world!");
            await ctx.Client.DisconnectAsync();
            Environment.Exit(0);
        }

        [SlashCommand("join", "Joins you in the current voice channel")]
        public static async Task Join(InteractionContext ctx)
        {
            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await ctx.CreateResponseAsync("You need to be in a voice channel in order for the bot to connect!");
                return;
            }

            try
            {
                await GetAudioService(ctx).Players.JoinAsync(
                    ctx.Guild.Id,
                    channel.Id,
                    PlayerFactory.Queued,
                    new QueuedLavalinkPlayerOptions(),
                    CancellationToken.None);
                await ctx.CreateResponseAsync("I have now connected to " + channel.Name + "!");
            }
            catch (Exception ex)
            {
                await ctx.CreateResponseAsync($"Unable to connect to voice: `{ex.Message}`");
            }
        }

        [SlashCommand("leave", "Leaves the voice channel")]
        public static async Task Leave(InteractionContext ctx)
        {
            if (!GetAudioService(ctx).Players.TryGetPlayer(ctx.Guild.Id, out ILavalinkPlayer player))
            {
                await ctx.CreateResponseAsync(NotConnectedMessage);
                return;
            }

            await player.DisconnectAsync();
            await ctx.CreateResponseAsync("I have left the voice channel!");
        }

        [SlashCommand("play", "plays an uploaded MP3")]
        public static async Task Play(InteractionContext ctx, [Option("mp3", "The MP3 or WAV file to play")] DiscordAttachment file)
        {
            if (file == null || (!Path.GetExtension(file.FileName).Equals(".mp3", StringComparison.OrdinalIgnoreCase) && !Path.GetExtension(file.FileName).Equals(".wav", StringComparison.OrdinalIgnoreCase)))
            {
                await ctx.CreateResponseAsync("Please upload an MP3 or WAV file.");
                return;
            }

            var channel = ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await ctx.CreateResponseAsync("You need to be in a voice channel for the bot to connect.");
                return;
            }

            try
            {
                var player = await GetAudioService(ctx).Players.JoinAsync(
                    ctx.Guild.Id,
                    channel.Id,
                    PlayerFactory.Queued,
                    new QueuedLavalinkPlayerOptions(),
                    CancellationToken.None);

                int pos = await player.PlayAsync(new Uri(file.Url), enqueue: true, cancellationToken: CancellationToken.None);
                if (pos == 0)
                {
                    await ctx.CreateResponseAsync($"Now playing: {file.FileName}");
                }
                else
                {
                    await ctx.CreateResponseAsync($"Added to queue at position {pos}: {file.FileName}");
                }
            }
            catch (Exception ex)
            {
                await ctx.CreateResponseAsync($"An exception occurred during playback: `{ex.GetType()}: {ex.Message}`");
            }
        }

        [SlashCommand("skip", "skips the current track")]
        public static async Task Skip(InteractionContext ctx)
        {

            try
            {
                var player = await GetAudioService(ctx).Players.GetPlayerAsync<QueuedLavalinkPlayer>(
                    ctx.Guild.Id);

                if (player == null)
                {
                    await ctx.CreateResponseAsync(NotConnectedMessage);
                    return;
                }

                await player.SkipAsync();
                await ctx.CreateResponseAsync("Track skipped!");
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
                    $"An exception occurred during playback: `{ex.GetType()}: {ex.Message}`"));
            }
        }

        [SlashCommand("pause", "pauses the current track")]
        public static async Task Pause(InteractionContext ctx)
        {

            try
            {
                var player = await GetAudioService(ctx).Players.GetPlayerAsync<QueuedLavalinkPlayer>(
                    ctx.Guild.Id);

                if (player == null)
                {
                    await ctx.CreateResponseAsync(NotConnectedMessage);
                    return;
                }

                if (player.IsPaused)
                {
                    await ctx.CreateResponseAsync("Already paused!");
                    return;
                }

                await player.PauseAsync();
                await ctx.CreateResponseAsync("Paused!");
            }
            catch (Exception ex)
            {
                await ctx.CreateResponseAsync($"An exception occurred during playback: `{ex.GetType()}: {ex.Message}`");
            }
        }

        [SlashCommand("resume", "resumes the current track")]
        public static async Task Resume(InteractionContext ctx)
        {

            try
            {
                var player = await GetAudioService(ctx).Players.GetPlayerAsync<QueuedLavalinkPlayer>(
                    ctx.Guild.Id);

                if (player == null)
                {
                    await ctx.CreateResponseAsync(NotConnectedMessage);
                    return;
                }

                if (!player.IsPaused)
                {
                    await ctx.CreateResponseAsync("Already playing!");
                    return;
                }

                await player.ResumeAsync();
                await ctx.CreateResponseAsync("Resumed!");
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
                    $"An exception occurred during playback: `{ex.GetType()}: {ex.Message}`"));
            }
        }

        [SlashCommand("listqueue", "lists the current queue")]
        public static async Task ListQueue(InteractionContext ctx)
        {

            try
            {
                var player = await GetAudioService(ctx).Players.GetPlayerAsync<QueuedLavalinkPlayer>(
                    ctx.Guild.Id);

                if (player == null)
                {
                    await ctx.CreateResponseAsync(NotConnectedMessage);
                    return;
                }

                // get list of queued tracks
                var current = player.CurrentItem?.Identifier;
                var queuedTracks = player.Queue.Select((track, index) => $"{index + 1}) {track.Identifier}").ToList();

                if (current == null)
                {
                    await ctx.CreateResponseAsync("No track is currently playing.");
                    return;
                }
                else if (queuedTracks.Count == 0)
                {
                    await ctx.CreateResponseAsync($"Currently playing: {current}\nNo tracks in the queue.");
                    return;
                }

                await ctx.CreateResponseAsync($"Currently playing: {current}\nQueued tracks:\n{string.Join("\n", queuedTracks)}");
            }
            catch (Exception ex)
            {
                await ctx.CreateResponseAsync($"An exception occurred during playback: `{ex.GetType()}: {ex.Message}`");
            }
        }

        private static Lavalink4NET.IAudioService GetAudioService(InteractionContext ctx)
        {
            return LavalinkAudioServices.Get(ctx.Client);
        }
    }
}
