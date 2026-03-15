using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Geese.commands;
using Newtonsoft.Json;
using System;
using System.IO;
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
        public static VoiceNextExtension Voice1 { get; set; }//stores voice client
        public static VoiceNextExtension Voice2 { get; set; }//stores voice client
        public static VoiceNextExtension Voice3 { get; set; }//stores voice client
        public static VoiceNextExtension Voice4 { get; set; }//stores voice client
        public static VoiceNextExtension Voice5 { get; set; }//stores voice client
        public static VoiceNextExtension Voice6 { get; set; }//stores voice client
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
            };//client configuration settings
            var config2 = new DiscordConfiguration()
            {
                Token = config.Token2,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            var config3 = new DiscordConfiguration()
            {
                Token = config.Token3,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            var config4 = new DiscordConfiguration()
            {
                Token = config.Token4,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            var config5 = new DiscordConfiguration()
            {
                Token = config.Token5,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            var config6 = new DiscordConfiguration()
            {
                Token = config.Token6,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            Client1 = new DiscordClient(config1);//initialises client with discord config set above
            Client2 = new DiscordClient(config2);//initialises client with discord config set above
            Client3 = new DiscordClient(config3);//initialises client with discord config set above
            Client4 = new DiscordClient(config4);//initialises client with discord config set above
            Client5 = new DiscordClient(config5);//initialises client with discord config set above
            Client6 = new DiscordClient(config6);//initialises client with discord config set above
            var voiceconfig = new VoiceNextConfiguration//sets up voice
            {
                EnableIncoming = false,//disables incoming voice
                AudioFormat = AudioFormat.Default//default audio formatting
            };
            Voice1 = Client1.UseVoiceNext(voiceconfig);//enables voice
            Client1.Ready += OnClientReady;//adds client ready event
            Client1.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client1.ClientErrored += Client_ClientError;//adds client error event
            Voice2 = Client2.UseVoiceNext(voiceconfig);//enables voice
            Client2.Ready += OnClientReady;//adds client ready event
            Client2.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client2.ClientErrored += Client_ClientError;//adds client error event
            Voice3 = Client3.UseVoiceNext(voiceconfig);//enables voice
            Client3.Ready += OnClientReady;//adds client ready event
            Client3.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client3.ClientErrored += Client_ClientError;//adds client error event
            Voice4 = Client4.UseVoiceNext(voiceconfig);//enables voice
            Client4.Ready += OnClientReady;//adds client ready event
            Client4.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client4.ClientErrored += Client_ClientError;//adds client error event
            Voice5 = Client5.UseVoiceNext(voiceconfig);//enables voice
            Client5.Ready += OnClientReady;//adds client ready event
            Client5.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client5.ClientErrored += Client_ClientError;//adds client error event
            Voice6 = Client6.UseVoiceNext(voiceconfig);//enables voice
            Client6.Ready += OnClientReady;//adds client ready event
            Client6.GuildAvailable += Client_GuildAvailable;//adds guild avilable event
            Client6.ClientErrored += Client_ClientError;//adds client error event

            await Client1.ConnectAsync();//connects to discord asyncronously
            await Client2.ConnectAsync();//connects to discord asyncronously
            await Client3.ConnectAsync();//connects to discord asyncronously
            await Client4.ConnectAsync();//connects to discord asyncronously
            await Client5.ConnectAsync();//connects to discord asyncronously
            await Client6.ConnectAsync();//connects to discord asyncronously
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
