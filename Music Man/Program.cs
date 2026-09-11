using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Music_Man.commands;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Music_Man
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            ConfigJson config = GetJSON().Result;
            DiscordClient discord = new(new DiscordConfiguration()
            {
                Token = config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildVoiceStates,
                MinimumLogLevel = LogLevel.Information,
            });
            var lavalink = discord.UseLavalink();//enables Lavalink voice
            discord.Ready += OnClientReady;//adds client ready event
            discord.GuildAvailable += Client_GuildAvailable;//add guilds avilable event
            discord.ClientErrored += Client_ClientError;//adds client error event

            var slash = discord.UseSlashCommands();
            slash.RegisterCommands<SlashCommands>();

            await discord.ConnectAsync();
            var restEndpoint = new Uri(config.Lavalink.RestEndpoint);
            var socketEndpoint = new Uri(config.Lavalink.SocketEndpoint);
            var lavalinkRestEndpoint = new DSharpPlus.Net.ConnectionEndpoint(restEndpoint.Host, restEndpoint.Port, restEndpoint.Scheme == "https");
            var lavalinkSocketEndpoint = new DSharpPlus.Net.ConnectionEndpoint(socketEndpoint.Host, socketEndpoint.Port, socketEndpoint.Scheme == "wss");
            await lavalink.ConnectAsync(new LavalinkConfiguration
            {
                RestEndpoint = lavalinkRestEndpoint,
                SocketEndpoint = lavalinkSocketEndpoint,
                Password = config.Lavalink.Password,
                SocketAutoReconnect = true,
            });
            await Task.Delay(-1);
        }

        static async Task<ConfigJson> GetJSON()
        {
            string json = string.Empty;//will store json
            string configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
            using (FileStream fs = File.OpenRead(configPath))
            {
                using StreamReader sr = new(fs, new UTF8Encoding(false));
                json = await sr.ReadToEndAsync();//loads json as string
            }

            ConfigJson configjson = JsonConvert.DeserializeObject<ConfigJson>(json);//configures json to class
            return configjson;
        }
        private static Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            Console.WriteLine("Client is ready to process events.");
            return Task.CompletedTask;
        }
        private static Task Client_GuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            Console.WriteLine("Guild Avaiable: " + e.Guild.Name);
            return Task.CompletedTask;
        }

        private static Task Client_ClientError(DiscordClient client, ClientErrorEventArgs e)
        {
            Console.WriteLine("Exception occured: " + e.Exception.GetType() + ": " + e.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
