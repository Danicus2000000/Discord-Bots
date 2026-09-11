using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;
/*
 * To Complete:
 * Make geese all work together
 * */
namespace Geese.commands
{
    internal class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("logout", "Shuts down the bot")]
        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public static async Task Logout(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("I have logged out!");
            await ctx.Client.DisconnectAsync();//dsiconnect client
            Environment.Exit(0);//kill program
        }

        [SlashCommand("flock", "flocks and honks")]
        public static async Task Play(InteractionContext ctx)
        {
            // check whether Lavalink is enabled
            var lavalink = ctx.Client.GetLavalink();
            if (lavalink == null)//if lavalink not enabled
            {
                await ctx.CreateResponseAsync("Lavalink is not enabled or configured!");
                return;
            }

            var vnc = lavalink.GetGuildConnection(ctx.Guild);//gets connection state
            if (vnc == null)//if we are not connected
            {
                var chn = ctx.Member?.VoiceState?.Channel;//gets message member voice channel
                if (chn == null)
                {
                    await ctx.CreateResponseAsync("You need to be in a voice channel in order for bot to auto connect!");//throw exception
                    return;
                }

                var node = lavalink.ConnectedNodes.Values.First();
                vnc = await node.ConnectAsync(chn);
            }

            // play
            try
            {
                await ctx.CreateResponseAsync("I have flocked");
                string honkPath = $"https://www.dropbox.com/scl/fi/4z1bit7hqtahkd5eal9wu/HONK.mp3?rlkey=8mgc54j83gmdiw2vli1t5l455&st=fqpin5cr&dl=1";
                var result = await vnc.GetTracksAsync(new Uri(honkPath));//send speaking prompt
                var track = result.Tracks.FirstOrDefault();
                if (result.LoadResultType == LavalinkLoadResultType.NoMatches || track == null)
                    throw new InvalidOperationException("Lavalink could not load the attachment.");

                if (result.LoadResultType == LavalinkLoadResultType.LoadFailed)
                    throw new InvalidOperationException(result.Exception.Message);

                await vnc.PlayAsync(track);
            }
            finally
            {
                await ctx.EditResponseAsync(new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent("Flock Completed"));
            }
        }

        [SlashCommand("deflock", "Leaves the voice channel")]
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
            await ctx.CreateResponseAsync("I have deflocked.");
        }
    }
}
