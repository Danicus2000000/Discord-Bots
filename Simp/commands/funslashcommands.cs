using DSharpPlus;
using System;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
namespace OwO.commands
{
    public class FunSlashCommands : ApplicationCommandModule
    {
        ////test command to ensure bot works correctly
        //[SlashCommand("ping", "Responds to Ping with Pong")]
        //public async Task Ping(InteractionContext ctx)
        //{
        //    await ctx.CreateResponseAsync("Pong");//answers ping with pong
        //}
        [SlashCommand("simp4me", "Sends a random simp message")]
        public static async Task SimpForMe(InteractionContext ctx)
        {
            string[] options = ["You look okay today! OwO", "you want some fuck?", "OwO could you like be mine OwO", "great job today!", "you look adorable!", "can i like pay all your bills", "i would tier 3 sub for u"];
            Random random = new();
            int result = random.Next(0, options.Length);
            await ctx.CreateResponseAsync(options[result]);
        }
        [SlashRequireUserPermissions(Permissions.Administrator)]
        [SlashCommand("logout", "Shuts down the bot")]
        public static async Task Logout(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("Goodbye cruel world");//goodbye message
            await ctx.Client.DisconnectAsync();//disconnect client
            Environment.Exit(0);//kill program
        }
    }
}
