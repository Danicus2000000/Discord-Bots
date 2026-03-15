using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Simp.commands;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Simp
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
                Intents = DiscordIntents.AllUnprivileged,
                MinimumLogLevel = LogLevel.Debug,
            });
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            discord.Ready += OnClientReady;//adds client ready event
            discord.GuildAvailable += Client_GuildAvailable;//add guilds avilable event
            discord.ClientErrored += Client_ClientError;//adds client error event
            await discord.ConnectAsync();
            var slash = discord.UseSlashCommands();
            slash.RegisterCommands<FunSlashCommands>();
            await Task.Delay(-1);
        }

        static async Task<ConfigJson> GetJSON()
        {
            string json = string.Empty;//will store json
            using (FileStream fs = File.OpenRead("config.json"))
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
