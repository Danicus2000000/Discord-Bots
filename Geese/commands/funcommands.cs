using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Geese.commands
{
    [Obsolete("Funcommands is moved into slash commands",true)]
    class FunCommands : BaseCommandModule
    {
        [Command("logout")]
        [Description("Shuts down the bot")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Logout(CommandContext ctx)
        {
            await ctx.Client.DisconnectAsync();//dsiconnect client
            Environment.Exit(0);//kill program
        }
    }
}
