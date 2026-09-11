using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Music_Man.commands
{
    internal class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("logout", "logs out the bot")]
        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public static async Task Logout(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("goodbye cruel world!");
            await ctx.Client.DisconnectAsync();
            Environment.Exit(0);
        }

        [SlashCommand("join", "Joins you in the current voice channel")]
        public static async Task Join(InteractionContext ctx)
        {
            var lavalink = ctx.Client.GetLavalink();
            if (lavalink == null || lavalink.ConnectedNodes.Count == 0)
            {
                await ctx.CreateResponseAsync("Lavalink is not enabled or connected.");
                return;
            }

            var vnc = lavalink.GetGuildConnection(ctx.Guild);
            if (vnc == null)//if we are not connected
            {
                var chn = ctx.Member?.VoiceState?.Channel;
                if (chn == null)
                {
                    await ctx.CreateResponseAsync("You need to be in a voice channel in order for bot to auto connect!");
                    return;
                }
                var node = lavalink.ConnectedNodes.Values.First();
                await node.ConnectAsync(chn);
                await ctx.CreateResponseAsync("I have now connected to " + ctx.Member?.VoiceState?.Channel.Name + "!");
            }
            else
            {
                await ctx.CreateResponseAsync("I am already connected to a voice channel.");
            }
        }

        [SlashCommand("leave", "Leaves the voice channel")]
        public static async Task Leave(InteractionContext ctx)
        {
            var lavalink = ctx.Client.GetLavalink();
            if (lavalink == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not enabled or configured!");
                return;
            }

            var vnc = lavalink.GetGuildConnection(ctx.Guild);
            if (vnc == null)//if no state
            {
                await ctx.CreateResponseAsync("Not connected in this guild!");
                return;
            }
            await vnc.DisconnectAsync();
            await ctx.CreateResponseAsync("I have left the voice channel!");
        }

        [SlashCommand("play", "plays an uploaded MP3")]
        public static async Task Play(InteractionContext ctx, [Option("mp3", "The MP3 file to play")] DiscordAttachment mp3)
        {
            if (mp3 == null || !Path.GetExtension(mp3.FileName).Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                await ctx.CreateResponseAsync("Please upload an MP3 file.");
                return;
            }

            var lavalink = ctx.Client.GetLavalink();
            if (lavalink == null || lavalink.ConnectedNodes.Count == 0)
            {
                await ctx.CreateResponseAsync("Lavalink is not enabled or connected.");
                return;
            }

            var vnc = lavalink.GetGuildConnection(ctx.Guild);
            if (vnc == null)
            {
                var chn = ctx.Member?.VoiceState?.Channel;
                if (chn == null)
                {
                    await ctx.CreateResponseAsync("You need to be in a voice channel for the bot to connect.");
                    return;
                }

                var node = lavalink.ConnectedNodes.Values.First();
                vnc = await node.ConnectAsync(chn);
            }

            await ctx.CreateResponseAsync($"Playing `{mp3.FileName}`");
            try
            {
                var result = await vnc.GetTracksAsync(new Uri(mp3.Url));
                var track = result.Tracks.FirstOrDefault();
                if (result.LoadResultType == LavalinkLoadResultType.NoMatches || track == null)
                    throw new InvalidOperationException("Lavalink could not load the attachment.");

                if (result.LoadResultType == LavalinkLoadResultType.LoadFailed)
                    throw new InvalidOperationException(result.Exception.Message);

                await vnc.PlayAsync(track);
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
                    $"An exception occurred during playback: `{ex.GetType()}: {ex.Message}`"));
            }
        }
    }
}
