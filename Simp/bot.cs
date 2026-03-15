using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using OwO.commands;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OwO
{
    [Obsolete("Code has been merged into main",true)]
    public class Bot
    {
        public DiscordClient Client { get; private set; }//storesa client
        public CommandsNextExtension Commands { get; private set; }//stores commands client
        public InteractivityExtension Interactivity { get; private set; }//stores interactivity client
        public async Task RunAsync() //function to async run client
        {
            #region Read JSON
            var json = string.Empty;//will store json
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();//loads json as string

            var configjson = JsonConvert.DeserializeObject<ConfigJson>(json);//configures json to class
            #endregion
            var config = new DiscordConfiguration
            {
                Token = configjson.Token,//sets token
                TokenType = TokenType.Bot,//shows token type
                AutoReconnect = true,//bot tries to reconnect when connection is lost
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,//debug log
            };//client configuration settings
            Client = new DiscordClient(config);//initialises client with discord config set above
            Client.Ready += OnClientReady;//adds client ready event
            Client.GuildAvailable += Client_GuildAvailable;//add guilds avilable event
            Client.ClientErrored += Client_ClientError;//adds client error event

            var commandsconfig = new CommandsNextConfiguration
            {
                StringPrefixes = [configjson.CommandPrefix],//gets string prefixes
                EnableMentionPrefix = true,//enables mention useage rather than prefix
                EnableDms = false,//disables dm usage
                IgnoreExtraArguments = true//cuts off unrequired arguments
            };//gets configuration for commands
            Commands = Client.UseCommandsNext(commandsconfig);//sets command confiuration 
            Commands.RegisterCommands<FunCommands>();
            await Client.ConnectAsync();//connects to discord asyncronously
            await Task.Delay(-1);//ensures bot cannot accidentally quit
        }

        private Task OnClientReady(DiscordClient client,ReadyEventArgs e)
        {
            Console.WriteLine("Client is ready to process events");
            return Task.CompletedTask;
        }
        private Task Client_GuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            Console.WriteLine("Guild available: " + e.Guild.Name);
            return Task.CompletedTask;
        }

        private Task Client_ClientError(DiscordClient client, ClientErrorEventArgs e)
        {
            Console.WriteLine("Exception occured: " + e.Exception.GetType() + ": " + e.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
