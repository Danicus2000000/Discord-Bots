using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using Geese.commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Geese
{
    [Obsolete("Bot has been merged into Main",true)]
    public class Bot
    {
        public DiscordClient Client1 { get; private set; }//stores client
        public DiscordClient Client2 { get; private set; }//stores client
        public DiscordClient Client3 { get; private set; }//stores client
        public DiscordClient Client4 { get; private set; }//stores client
        public DiscordClient Client5{ get; private set; }//stores client
        public DiscordClient Client6 { get; private set; }//stores client
        public CommandsNextExtension Commands1 { get; private set; }//stores commands client
        public CommandsNextExtension Commands2 { get; private set; }//stores commands client
        public CommandsNextExtension Commands3 { get; private set; }//stores commands client
        public CommandsNextExtension Commands4{ get; private set; }//stores commands client
        public CommandsNextExtension Commands5 { get; private set; }//stores commands client
        public CommandsNextExtension Commands6 { get; private set; }//stores commands client
        public static VoiceNextExtension Voice1 { get; set; }//stores voice client
        public static VoiceNextExtension Voice2 { get; set; }//stores voice client
        public static VoiceNextExtension Voice3 { get; set; }//stores voice client
        public static VoiceNextExtension Voice4 { get; set; }//stores voice client
        public static VoiceNextExtension Voice5 { get; set; }//stores voice client
        public static VoiceNextExtension Voice6 { get; set; }//stores voice client
        public async Task RunAsync() //function to async run client
        {
            #region Read JSON
            var json = string.Empty;//will store json
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();//loads json as string

            var configjson = JsonConvert.DeserializeObject<ConfigJson>(json);//configures json to class
            #endregion
            var config1 = new DiscordConfiguration
            {
                Token = configjson.Token1,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            var config2 = new DiscordConfiguration
            {
                Token = configjson.Token2,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            var config3 = new DiscordConfiguration
            {
                Token = configjson.Token3,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            var config4 = new DiscordConfiguration
            {
                Token = configjson.Token4,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            var config5 = new DiscordConfiguration
            {
                Token = configjson.Token5,//sets token
                TokenType = TokenType.Bot,//sets token type
                AutoReconnect = true,//ensures bot attempts reconnect if connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            var config6 = new DiscordConfiguration
            {
                Token = configjson.Token6,//sets token
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
            var commandsconfig = new CommandsNextConfiguration//sets up command module
            {
                EnableMentionPrefix = true,//allows @mention as prefix
                EnableDms = false,//disables dm commands
                IgnoreExtraArguments = true//allows commands to run when extra arguments are placed
            };//gets configuration for commands
            Commands1 = Client1.UseCommandsNext(commandsconfig);//sets command confiuration 
            Commands1.RegisterCommands<FunCommands>();
            Commands1.RegisterCommands<voicecommands>();
            Commands2 = Client2.UseCommandsNext(commandsconfig);//sets command confiuration 
            Commands2.RegisterCommands<FunCommands>();
            Commands2.RegisterCommands<voicecommands>();
            Commands3 = Client3.UseCommandsNext(commandsconfig);//sets command confiuration 
            Commands3.RegisterCommands<FunCommands>();
            Commands3.RegisterCommands<voicecommands>();
            Commands4 = Client4.UseCommandsNext(commandsconfig);//sets command confiuration 
            Commands4.RegisterCommands<FunCommands>();
            Commands4.RegisterCommands<voicecommands>();
            Commands5 = Client5.UseCommandsNext(commandsconfig);//sets command confiuration 
            Commands5.RegisterCommands<FunCommands>();
            Commands5.RegisterCommands<voicecommands>();
            Commands6 = Client6.UseCommandsNext(commandsconfig);//sets command confiuration 
            Commands6.RegisterCommands<FunCommands>();
            Commands6.RegisterCommands<voicecommands>();
            await Client1.ConnectAsync();//connects to discord asyncronously
            await Client2.ConnectAsync();//connects to discord asyncronously
            await Client3.ConnectAsync();//connects to discord asyncronously
            await Client4.ConnectAsync();//connects to discord asyncronously
            await Client5.ConnectAsync();//connects to discord asyncronously
            await Client6.ConnectAsync();//connects to discord asyncronously
            await Task.Delay(-1);//ensures bot cannot accidentally quit
        }

        private Task OnClientReady(DiscordClient client,ReadyEventArgs e)
        {
            Console.WriteLine("Client is ready to process events.");
            return Task.CompletedTask;
        }
        private Task Client_GuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            Console.WriteLine("Guild Available: " + e.Guild.Name);
            return Task.CompletedTask;
        }

        private Task Client_ClientError(DiscordClient client, ClientErrorEventArgs e)
        {
            Console.WriteLine("Exception Ocuured: " + e.Exception.GetType() + ": " + e.Exception.Message);
            return Task.CompletedTask;
        }
    }
}

