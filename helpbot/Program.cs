using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Audio;
namespace helpbot
{
    class Program
    {
        private DiscordSocketClient Client;
        private CommandService Commands;
        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel =LogSeverity.Debug
            });
            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });
            Client.MessageReceived += Client_MessageRecieved;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            Client.Ready += Client_ready;
            Client.Log += Client_log;
            string token = "";
            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task Client_log(LogMessage arg)
        {
            Console.WriteLine($"{DateTime.Now} at {arg.Source}: {arg.Message}");
            await Task.FromResult(Task.CompletedTask);
        }

        private async Task Client_ready()
        {
            await Client.SetGameAsync("Your Mum", null, ActivityType.Playing);
            await Client.SetStatusAsync(UserStatus.Online);
        }

        private async Task Client_MessageRecieved(SocketMessage Messageprompt)
        {
            var message = Messageprompt as SocketUserMessage;
            var Context = new SocketCommandContext(Client, message);
            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;
            int argspos = 0;
            if (!(message.HasCharPrefix('!', ref argspos) || message.HasMentionPrefix(Client.CurrentUser, ref argspos))) return;

            var Result = await Commands.ExecuteAsync(Context, argspos,null);

            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at commands something went wrong when executing a command text: {Context.Message.Content} reason: {Result.ErrorReason}");
            }

        }
    }
}
