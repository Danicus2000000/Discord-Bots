using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.VoiceNext;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
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
                await vnext.ConnectAsync(chn);//connect
                await ctx.CreateResponseAsync("I have now connected to " + ctx.Member?.VoiceState?.Channel.Name + "!");
            }
        }

        [SlashCommand("leave", "Leaves the voice channel")]
        public static async Task Leave(InteractionContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();//get voice client
            if (vnext == null)//if vnext not enabled
            {
                await ctx.CreateResponseAsync("Vnext is not enabled or configured!");
                return;
            }

            var vnc = vnext.GetConnection(ctx.Guild);//gets connection state
            if (vnc == null)//if no state
            {
                await ctx.CreateResponseAsync("Not connected in this guild!");//error message
            }
            vnc.Dispose();
            await ctx.CreateResponseAsync("I have left " + ctx.Member?.VoiceState?.Channel.Name + "!");//disconnect message
        }

        [SlashCommand("play", "plays an uploaded MP3")]
        public static async Task Play(InteractionContext ctx, [Option("mp3", "The MP3 file to play")] DiscordAttachment mp3)
        {
            if (mp3 == null || !Path.GetExtension(mp3.FileName).Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                await ctx.CreateResponseAsync("Please upload an MP3 file.");
                return;
            }

            var vnext = ctx.Client.GetVoiceNext();//gets voice state
            if (vnext == null)
            {
                await ctx.CreateResponseAsync("VNext is not enabled or configured.");
                return;
            }

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                var chn = ctx.Member?.VoiceState?.Channel;//gets message member voice channel
                if (chn == null)
                {
                    await ctx.CreateResponseAsync("You need to be in a voice channel! for bot auto connect");//error message
                    return;
                }

                await ctx.Channel.SendMessageAsync("I am in " + chn.Name + "!");//connection message
                vnc = await vnext.ConnectAsync(chn);//connect
            }

            while (vnc.IsPlaying)
                await vnc.WaitForPlaybackFinishAsync();

            Exception exception = null;
            await ctx.CreateResponseAsync($"Playing `{mp3.FileName}`");
            try
            {
                using var httpClient = new HttpClient();
                using var mp3Stream = new MemoryStream();
                using (var responseStream = await httpClient.GetStreamAsync(mp3.Url))
                {
                    await responseStream.CopyToAsync(mp3Stream);
                }

                mp3Stream.Position = 0;
                await vnc.SendSpeakingAsync(true);//sends speak prompt

                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };
                psi.ArgumentList.Add("-i");
                psi.ArgumentList.Add("pipe:0");
                psi.ArgumentList.Add("-ac");
                psi.ArgumentList.Add("2");
                psi.ArgumentList.Add("-f");
                psi.ArgumentList.Add("s16le");
                psi.ArgumentList.Add("-ar");
                psi.ArgumentList.Add("48000");
                psi.ArgumentList.Add("pipe:1");

                using var ffmpeg = Process.Start(psi) ?? throw new InvalidOperationException("Unable to start ffmpeg.exe.");
                var transmitStream = vnc.GetTransmitSink();
                var inputTask = mp3Stream.CopyToAsync(ffmpeg.StandardInput.BaseStream);
                var outputTask = ffmpeg.StandardOutput.BaseStream.CopyToAsync(transmitStream);
                await inputTask;
                await ffmpeg.StandardInput.BaseStream.FlushAsync();
                await ffmpeg.StandardInput.BaseStream.DisposeAsync();
                await outputTask;
                await ffmpeg.WaitForExitAsync();

                if (ffmpeg.ExitCode != 0)
                    throw new InvalidOperationException($"ffmpeg exited with code {ffmpeg.ExitCode}.");

                await transmitStream.FlushAsync();
                await vnc.WaitForPlaybackFinishAsync();
            }
            catch (Exception ex) { exception = ex; }
            finally
            {
                await vnc.SendSpeakingAsync(false);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(exception == null
                    ? $"Finished playing `{mp3.FileName}`"
                    : $"An exception occurred during playback: `{exception.GetType()}: {exception.Message}`"));
            }
        }
    }
}
