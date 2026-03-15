using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.VoiceNext;
using System;
using System.Diagnostics;
using System.Threading;
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
            // check whether VNext is enabled
            var vnext = ctx.Client.GetVoiceNext();//gets voice state
            if (vnext == null)//if vnext not enabled
            {
                await ctx.CreateResponseAsync("Vnext is not enabled or configured!");
                return;
            }

            var vnc = vnext.GetConnection(ctx.Guild);//gets connection state
            if (vnc == null)//if we are not connected
            {
                var chn = ctx.Member?.VoiceState?.Channel;//gets message member voice channel
                if (chn == null)
                {
                    await ctx.CreateResponseAsync("You need to be in a voice channel in order for bot to auto connect!");//throw exception
                    return;
                }
                Random test = new();
                int offset = test.Next(0, 1500);
                Thread.Sleep(offset);
                vnc = await vnext.ConnectAsync(chn);//connect
            }
            // wait for current playback to finish
            while (vnc.IsPlaying)
                await vnc.WaitForPlaybackFinishAsync();

            // play
            Exception exc;

            try
            {
                await vnc.SendSpeakingAsync(true);//send speaking prompt
                await ctx.CreateResponseAsync("I have flocked");
                var psi = new ProcessStartInfo//starts ffmeg process
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{"HONK.mp3"}"" -ac 2 -f s16le -ar 48000 pipe:1 -vol 256",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };
                Random test = new();
                int offset = test.Next(0, 5000);
                Thread.Sleep(offset);
                var ffmpeg = Process.Start(psi);
                var ffout = ffmpeg.StandardOutput.BaseStream;

                var txStream = vnc.GetTransmitSink();
                await ffout.CopyToAsync(txStream);//clones ffmpeg stream to discord call
                await txStream.FlushAsync();
                await vnc.WaitForPlaybackFinishAsync();

                await vnc.SendSpeakingAsync(true);//send speaking prompt

            }
            catch (Exception ex) { exc = ex; }
            finally
            {
                await vnc.SendSpeakingAsync(false);
                vnc.Disconnect();
                await ctx.EditResponseAsync(new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent("Flock Completed"));
            }
        }
        [SlashCommand("deflock", "Leaves the voice channel")]
        public static async Task Leave(InteractionContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();//get voice client
            var vnc = vnext.GetConnection(ctx.Guild);//gets connection state
            if (vnc == null)//if no state
            {
                await ctx.CreateResponseAsync("Not connected to this guild!");//throws error
                return;
            }
            await ctx.CreateResponseAsync("I have deflocked!");
            vnc.Disconnect();//disconnect
        }
    }
}
