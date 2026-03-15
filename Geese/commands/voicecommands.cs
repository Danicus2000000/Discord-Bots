using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using System.Threading;

namespace Geese.commands
{
    [Obsolete("Voice commmands have been merged into slash commands",true)]
    class voicecommands : BaseCommandModule
    {
        [Command("flock")]
        [Description("flocks and honks")]
        public async Task Play(CommandContext ctx)
        {
            // check whether VNext is enabled
            var vnext = ctx.Client.GetVoiceNext();//gets voice state
            if (vnext == null)//if vnext not enabled
            {
                throw new InvalidOperationException("Vnext is not enabled or configured!");
            }

            var vnc = vnext.GetConnection(ctx.Guild);//gets connection state
            if (vnc == null)//if we are not connected
            {
                var chn = ctx.Member?.VoiceState?.Channel;//gets message member voice channel
                if (chn == null)
                {
                    throw new InvalidOperationException("You need to be in a voice channel in order for bot to auto connect!");//throw exception
                }
                Random test = new Random();
                int offset = test.Next(0, 1500);
                Thread.Sleep(offset);
                vnc = await vnext.ConnectAsync(chn);//connect
            }
            // wait for current playback to finish
            while (vnc.IsPlaying)
                await vnc.WaitForPlaybackFinishAsync();

            // play
            Exception exc = null;

            try
            {
                await vnc.SendSpeakingAsync(true);//send speaking prompt

                var psi = new ProcessStartInfo//starts ffmeg process
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{"HONK.mp3"}"" -ac 2 -f s16le -ar 48000 pipe:1 -vol 256",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };
                Random test = new Random();
                int offset = test.Next(0, 5000);
                Thread.Sleep(offset);
                var ffmpeg = Process.Start(psi);
                var ffout = ffmpeg.StandardOutput.BaseStream;

                var txStream = vnc.GetTransmitSink();
                await ffout.CopyToAsync(txStream);//clones ffmpeg stream to discord call
                await txStream.FlushAsync();
                await vnc.WaitForPlaybackFinishAsync();

                await vnc.SendSpeakingAsync(true);//send speaking prompt

                psi = new ProcessStartInfo//starts ffmeg process
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{"HONK.mp3"}"" -ac 2 -f s16le -ar 48000 pipe:1 -vol 256",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };
                ffmpeg = Process.Start(psi);
                ffout = ffmpeg.StandardOutput.BaseStream;

                txStream = vnc.GetTransmitSink();
                await ffout.CopyToAsync(txStream);//clones ffmpeg stream to discord call
                await txStream.FlushAsync();
                await vnc.WaitForPlaybackFinishAsync();

                await vnc.SendSpeakingAsync(true);//send speaking prompt

                psi = new ProcessStartInfo//starts ffmeg process
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{"HONK.mp3"}"" -ac 2 -f s16le -ar 48000 pipe:1 -vol 256",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };
                ffmpeg = Process.Start(psi);
                ffout = ffmpeg.StandardOutput.BaseStream;

                txStream = vnc.GetTransmitSink();
                await ffout.CopyToAsync(txStream);//clones ffmpeg stream to discord call
                await txStream.FlushAsync();
                await vnc.WaitForPlaybackFinishAsync();

            }
            catch (Exception ex) { exc = ex; }
            finally
            {
                await vnc.SendSpeakingAsync(false);
                vnc.Disconnect();
            }
        }
        [Command("deflock")]
        [Description("Leaves the voice channel")]
        public async Task leave(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();//get voice client
            var vnc = vnext.GetConnection(ctx.Guild);//gets connection state
            if (vnc == null)//if no state
            {
                throw new InvalidOperationException("Not connected to this guild!");//throws error
            }
            vnc.Disconnect();//disconnect
        }
    }
}
