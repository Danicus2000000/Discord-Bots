using DiscordBots;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Geese.commands;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geese
{
    public class Program
    {
        public static DiscordClient Client1 { get; private set; }//stores client
        public static DiscordClient Client2 { get; private set; }//stores client
        public static DiscordClient Client3 { get; private set; }//stores client
        public static DiscordClient Client4 { get; private set; }//stores client
        public static DiscordClient Client5 { get; private set; }//stores client
        public static DiscordClient Client6 { get; private set; }//stores client
        public static DiscordClient[] Clients => new[]
        {
            Client1,
            Client2,
            Client3,
            Client4,
            Client5,
            Client6,
        };
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            ConfigJson config = GetJSON().Result;
            var config1 = new DiscordConfiguration()
            {
                Token = config.Token1,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildVoiceStates,
            };//client configuration settings
            var config2 = new DiscordConfiguration()
            {
                Token = config.Token2,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildVoiceStates,
            };//client configuration settings
            var config3 = new DiscordConfiguration()
            {
                Token = config.Token3,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildVoiceStates,
            };//client configuration settings
            var config4 = new DiscordConfiguration()
            {
                Token = config.Token4,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildVoiceStates,
            };//client configuration settings
            var config5 = new DiscordConfiguration()
            {
                Token = config.Token5,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildVoiceStates,
            };//client configuration settings
            var config6 = new DiscordConfiguration()
            {
                Token = config.Token6,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildVoiceStates,
            };//client configuration settings
            Client1 = new DiscordClient(config1);//initialises client with discord config set above
            Client2 = new DiscordClient(config2);//initialises client with discord config set above
            Client3 = new DiscordClient(config3);//initialises client with discord config set above
            Client4 = new DiscordClient(config4);//initialises client with discord config set above
            Client5 = new DiscordClient(config5);//initialises client with discord config set above
            Client6 = new DiscordClient(config6);//initialises client with discord config set above
            Client1.Ready += OnClientReady;//adds client ready event
            Client1.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client1.ClientErrored += Client_ClientError;//adds client error event
            Client2.Ready += OnClientReady;//adds client ready event
            Client2.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client2.ClientErrored += Client_ClientError;//adds client error event
            Client3.Ready += OnClientReady;//adds client ready event
            Client3.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client3.ClientErrored += Client_ClientError;//adds client error event
            Client4.Ready += OnClientReady;//adds client ready event
            Client4.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client4.ClientErrored += Client_ClientError;//adds client error event
            Client5.Ready += OnClientReady;//adds client ready event
            Client5.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client5.ClientErrored += Client_ClientError;//adds client error event
            Client6.Ready += OnClientReady;//adds client ready event
            Client6.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client6.ClientErrored += Client_ClientError;//adds client error event

            var restEndpoint = new Uri(config.Lavalink.RestEndpoint);
            var socketEndpoint = new Uri(config.Lavalink.SocketEndpoint);
            var webSocketUri = Lavalink4NetServiceFactory.GetWebSocketUri(socketEndpoint);
            var lavalinkServices = new[]
            {
                Lavalink4NetServiceFactory.Create(Client1, restEndpoint, webSocketUri, config.Lavalink.Password, "Geese bot 1"),
                Lavalink4NetServiceFactory.Create(Client2, restEndpoint, webSocketUri, config.Lavalink.Password, "Geese bot 2"),
                Lavalink4NetServiceFactory.Create(Client3, restEndpoint, webSocketUri, config.Lavalink.Password, "Geese bot 3"),
                Lavalink4NetServiceFactory.Create(Client4, restEndpoint, webSocketUri, config.Lavalink.Password, "Geese bot 4"),
                Lavalink4NetServiceFactory.Create(Client5, restEndpoint, webSocketUri, config.Lavalink.Password, "Geese bot 5"),
                Lavalink4NetServiceFactory.Create(Client6, restEndpoint, webSocketUri, config.Lavalink.Password, "Geese bot 6"),
            };
            LavalinkAudioServices.Register(Client1, lavalinkServices[0].AudioService);
            LavalinkAudioServices.Register(Client2, lavalinkServices[1].AudioService);
            LavalinkAudioServices.Register(Client3, lavalinkServices[2].AudioService);
            LavalinkAudioServices.Register(Client4, lavalinkServices[3].AudioService);
            LavalinkAudioServices.Register(Client5, lavalinkServices[4].AudioService);
            LavalinkAudioServices.Register(Client6, lavalinkServices[5].AudioService);

            var slash = Client1.UseSlashCommands();
            var slash2 = Client2.UseSlashCommands();
            var slash3 = Client3.UseSlashCommands();
            var slash4 = Client4.UseSlashCommands();
            var slash5 = Client5.UseSlashCommands();
            var slash6 = Client6.UseSlashCommands();
            slash.RegisterCommands<SlashCommands>();
            slash2.RegisterCommands<SlashCommands>();
            slash3.RegisterCommands<SlashCommands>();
            slash4.RegisterCommands<SlashCommands>();
            slash5.RegisterCommands<SlashCommands>();
            slash6.RegisterCommands<SlashCommands>();

            await Client1.ConnectAsync();//connects to discord asyncronously
            await Client2.ConnectAsync();//connects to discord asyncronously
            await Client3.ConnectAsync();//connects to discord asyncronously
            await Client4.ConnectAsync();//connects to discord asyncronously
            await Client5.ConnectAsync();//connects to discord asyncronously
            await Client6.ConnectAsync();//connects to discord asyncronously

            await Task.WhenAll(lavalinkServices.Select(service => service.StartAsync()));

            await Task.Delay(-1);//ensures bot cannot accidentally quit
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
