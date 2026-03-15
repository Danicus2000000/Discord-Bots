using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace OwO.commands
{
    [Obsolete("Commands have been transfered to the slash module",true)]
    class FunCommands : BaseCommandModule
    {
        //test command to ensure bot works correctly
        //[Command("ping")]
        //[Description("Responds to Ping with Pong")]
        //public async Task Ping(CommandContext ctx)
        //{
        //    await ctx.TriggerTypingAsync();//triggers typing
        //    await ctx.RespondAsync("Pong");//answers ping with pong
        //}
        [Command("simp4me")]
        [Description("Sends a random simp message")]
        public async Task SimpForMe(CommandContext ctx) 
        {
            string[] options =["You look okay today! OwO","you want some fuck?","OwO could you like be mine OwO","great job today!","you look adorable!","can i like pay all your bills","i would tier 3 sub for u"];
            Random random = new();
            int result=random.Next(0, options.Length);
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(options[result]);
        }
        [RequirePermissions(Permissions.Administrator)]
        [Command("logout")]
        [Description("Shuts down the bot")]
        public async Task Logout(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();//triggers typing
            await ctx.RespondAsync("Goodbye cruel world");//goodbye message
            await ctx.Client.DisconnectAsync();//disconnect client
            Environment.Exit(0);//kill program
        }
    }
}
