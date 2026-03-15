using System;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using System.IO;
using System.Threading;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
namespace Music_Man.commands
{
    internal class SlashCommands : ApplicationCommandModule
    {
        //test command
        //[SlashCommand("ping", "Responds to Ping with Pong")]
        //public async Task Ping(InteractionContext ctx)
        //{
        //    await ctx.CreateResponseAsync("Pong!");//responds with pong
        //}
        [SlashCommand("logout","logs out the bot")]
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
            else
            {
                var vnc = vnext.GetConnection(ctx.Guild);//gets connection state
                if (vnc == null)//if we are not connected
                {
                    var chn = ctx.Member?.VoiceState?.Channel;//gets message member voice channel
                    if (chn == null)
                    {
                        await ctx.CreateResponseAsync("You need to be in a voice channel in order for bot to auto connect!");//throw exception
                        return;
                    }
                    vnc = await vnext.ConnectAsync(chn);//connect
                    await ctx.CreateResponseAsync("I have now connected to " + ctx.Member?.VoiceState?.Channel.Name + "!");
                }
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
            else
            {
                var vnc = vnext.GetConnection(ctx.Guild);//gets connection state
                if (vnc == null)//if no state
                {
                    await ctx.CreateResponseAsync("Not connected in this guild!");//error message
                }
                vnc.Dispose();
                await ctx.CreateResponseAsync("I have left " + ctx.Member?.VoiceState?.Channel.Name + "!");//disconnect message
            }
        }
        #region Depreciated Commands (Removed due to security risk)
        //[SlashCommand("fileplay", "Plays an audio file via it's path on the server machine.")]
        //[SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        //public async Task Play(InteractionContext ctx, [Option("filename", "Full path to the file to play.")] string filename)
        //{
        //    // check whether VNext is enabled
        //    var vnext = ctx.Client.GetVoiceNext();//gets voice state
        //    if (vnext == null)//if vnext not enabled
        //    {
        //        await ctx.CreateResponseAsync("VNext is not enabled or configured.");//error message
        //    }
        //    else
        //    {

        //        var vnc = vnext.GetConnection(ctx.Guild);//gets connection state
        //        if (vnc == null)//if we are not connected
        //        {
        //            var chn = ctx.Member?.VoiceState?.Channel;//gets message member voice channel
        //            if (chn == null)
        //            {
        //                await ctx.CreateResponseAsync("You need to be in a voice channel! for bot auto connect");//error message
        //            }
        //            vnc = await vnext.ConnectAsync(chn);//connect
        //        }
        //        await ctx.Channel.SendMessageAsync("I am in " + ctx.Member?.VoiceState?.Channel.Name + "!");//connection message
        //        // check if file exists
        //        if (!File.Exists(filename))
        //        {
        //            // file does not exist
        //            await ctx.CreateResponseAsync($"File `{filename}` does not exist.");
        //        }
        //        else
        //        {
        //            // wait for current playback to finish
        //            while (vnc.IsPlaying)
        //                await vnc.WaitForPlaybackFinishAsync();

        //            // play
        //            Exception exc = null;
        //            await ctx.EditResponseAsync(new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent($"Playing `{filename}`"));

        //            try
        //            {
        //                await vnc.SendSpeakingAsync(true);//send speaking prompt

        //                var psi = new ProcessStartInfo//starts ffmeg process
        //                {
        //                    FileName = "ffmpeg.exe",
        //                    Arguments = $@"-i ""{filename}"" -ac 2 -f s16le -ar 48000 pipe:1 -vol 64",
        //                    RedirectStandardOutput = true,
        //                    UseShellExecute = false,
        //                };
        //                var ffmpeg = Process.Start(psi);
        //                var ffout = ffmpeg.StandardOutput.BaseStream;

        //                var txStream = vnc.GetTransmitSink();
        //                await ffout.CopyToAsync(txStream);//clones ffmpeg stream to discord call
        //                await txStream.FlushAsync();
        //                await vnc.WaitForPlaybackFinishAsync();
        //            }
        //            catch (Exception ex) { exc = ex; }
        //            finally
        //            {
        //                await vnc.SendSpeakingAsync(false);
        //                await ctx.EditResponseAsync(new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent($"Finished playing `{filename}`"));
        //            }

        //            if (exc != null)
        //            {
        //                await ctx.EditResponseAsync(new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent($"An exception occured during playback: `{exc.GetType()}: {exc.Message}`"));
        //            }
        //        }
        //    }
        //}

        //[SlashCommand("attachplay", "Plays an audio file attached to the message.")]
        //[Obsolete("Method cannot work with slash commands")]
        //public async Task attachPlay(InteractionContext ctx)
        //{
        //    // check whether VNext is enabled
        //    var vnext = ctx.Client.GetVoiceNext();//gets voice state
        //    if (vnext == null)//if vnext not enabled
        //    {
        //        await ctx.CreateResponseAsync("VNext is not enabled or configured.");//error message
        //    }
        //    else
        //    {
        //        var vnc = vnext.GetConnection(ctx.Guild);//gets connection state
        //        if (vnc == null)//if we are not connected
        //        {
        //            var chn = ctx.Member?.VoiceState?.Channel;//gets message member voice channel
        //            if (chn == null)
        //            {
        //                await ctx.CreateResponseAsync("You need to be in a voice channel! for bot auto connect");//error message
        //            }
        //            else
        //            {
        //                await ctx.Channel.SendMessageAsync("I have joined " + ctx.Member?.VoiceState?.Channel + "!");//connection message
        //                vnc = await vnext.ConnectAsync(chn);//connect
        //                                                    // wait for current playback to finish
        //                while (vnc.IsPlaying)
        //                    await vnc.WaitForPlaybackFinishAsync();

        //                // play
        //                Exception exc = null;
        //                await ctx.CreateResponseAsync($"Playing `{ctx.Message.Attachments[0].FileName}`");

        //                try
        //                {
        //                    await vnc.SendSpeakingAsync(true);//send speaking prompt
        //                    HttpClient client = new HttpClient();
        //                    var response = await client.GetAsync(ctx.Message.Attachments[0].Url);
        //                    var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "/temp.mp3";
        //                    using (var fs = new FileStream(String.Format(path), FileMode.Create))
        //                    {
        //                        await response.Content.CopyToAsync(fs);
        //                    }
        //                    var psi = new ProcessStartInfo//starts ffmeg process
        //                    {
        //                        FileName = "ffmpeg.exe",
        //                        Arguments = $@"-i ""{"temp.mp3"}"" -ac 2 -f s16le -ar 48000 pipe:1 -vol 64",
        //                        RedirectStandardOutput = true,
        //                        UseShellExecute = false
        //                    };
        //                    var ffmpeg = Process.Start(psi);
        //                    var ffout = ffmpeg.StandardOutput.BaseStream;

        //                    var txStream = vnc.GetTransmitSink();
        //                    await ffout.CopyToAsync(txStream);//clones ffmpeg stream to discord call
        //                    await txStream.FlushAsync();
        //                    await vnc.WaitForPlaybackFinishAsync();
        //                }
        //                catch (Exception ex) { exc = ex; }
        //                finally
        //                {
        //                    await vnc.SendSpeakingAsync(false);
        //                    await ctx.TriggerTypingAsync();
        //                    await ctx.Message.RespondAsync($"Finished playing `{ctx.Message.Attachments[0].FileName}`");
        //                    vnc.Dispose();
        //                    File.Delete("temp.mp3");
        //                }

        //                if (exc != null)
        //                {
        //                    await ctx.TriggerTypingAsync();
        //                    await ctx.RespondAsync($"An exception occured during playback: `{exc.GetType()}: {exc.Message}`");
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion
        [SlashCommand("youtubeplay", "plays youtube audio")]
        public static async Task YoutubePlay(InteractionContext ctx, [Option("URL", "The link to the youtube video")] string URL)
        {
            var vnext = ctx.Client.GetVoiceNext();//gets voice state
            if (vnext == null)
            {
                // not enabled
                await ctx.CreateResponseAsync("VNext is not enabled or configured.");
            }
            else
            {
                var vnc = vnext.GetConnection(ctx.Guild);
                if (vnc == null)
                {
                    var chn = ctx.Member?.VoiceState?.Channel;//gets message member voice channel
                    if (chn == null)
                    {
                        await ctx.CreateResponseAsync("You need to be in a voice channel! for bot auto connect");//error message
                    }
                    else
                    {
                        await ctx.Channel.SendMessageAsync("I am in " + ctx.Member?.VoiceState?.Channel.Name + "!");//connection message
                        vnc = await vnext.ConnectAsync(chn);//connect
                                                            // wait for current playback to finish
                        while (vnc.IsPlaying)
                            await vnc.WaitForPlaybackFinishAsync();

                        // play
                        Exception exc = null;
                        await ctx.CreateResponseAsync($"Playing `{URL[32..]}`");
                        try
                        {
                            var youtube = new YoutubeClient();//gets youtube client
                            var streams = await youtube.Videos.Streams.GetManifestAsync(URL);//gets video quality and audio branches
                            var streamInfo = streams.GetAudioOnlyStreams().GetWithHighestBitrate;//gets audio only branch
                            if (streamInfo == null)//if no branches are avilable
                            {
                                await ctx.EditResponseAsync( new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent("This videos has no Branches"));
                            }
                            var fileName = string.Concat(URL.AsSpan(32), ".mp3");//sets filename
                            await youtube.Videos.Streams.DownloadAsync(streamInfo.Invoke(), fileName);//downloads audio
                            await vnc.SendSpeakingAsync(true);//sends speak prompt
                            var psi = new ProcessStartInfo//starts ffmpeg
                            {
                                FileName = "ffmpeg.exe",
                                Arguments = $@"-i ""{string.Concat(URL.AsSpan(32), ".mp3")}"" -ac 2 -f s16le -ar 48000 pipe:1",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                            };
                            var ffmpeg = Process.Start(psi);
                            var ffout = ffmpeg.StandardOutput.BaseStream;
                            var txStream = vnc.GetTransmitSink();
                            await ffout.CopyToAsync(txStream);//clones ffmpeg stream to discord call
                            await txStream.FlushAsync();
                            await vnc.WaitForPlaybackFinishAsync();
                        }
                        catch (Exception ex) { exc = ex; }
                        finally
                        {
                            await vnc.SendSpeakingAsync(false);
                            await ctx.EditResponseAsync(new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent($"Finished playing `{URL[32..]}`"));
                            vnc.Dispose();
                            Thread.Sleep(5000);
                            File.Delete(string.Concat(URL.AsSpan(32), ".mp3"));
                        }

                        if (exc != null)
                        {
                            await ctx.EditResponseAsync(new DSharpPlus.Entities.DiscordWebhookBuilder().WithContent($"An exception occured during playback: `{exc.GetType()}: {exc.Message}`"));
                        }
                    }
                }
            }
        }
    }
}
