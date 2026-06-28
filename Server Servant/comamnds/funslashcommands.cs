using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
namespace Server_Servant.comamnds
{
    public class FunSlashCommands : ApplicationCommandModule
    {
        //Test Command
        //[SlashCommand("ping", "Responds to Ping with Pong")]
        //public async Task Ping(InteractionContext ctx)
        //{
        //    await ctx.CreateResponseAsync("Pong");//answers ping with pong
        //}
        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        [SlashCommand("logout", "Shuts down the bot")]
        public static async Task Logout(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("Goodbye cruel world");//goodbye message
            await ctx.Client.DisconnectAsync();//disconnect client
            Environment.Exit(0);//kill program
        }

        [SlashCommand("math", "Does basic math.")]
        public static async Task Math(InteractionContext ctx, [Option("MathToSolve", "The Math that needs to be completed")] string input)
        {
            StringToMath compute = new();
            double result = compute.Eval(input);
            await ctx.CreateResponseAsync(input + " = " + result);//sends answer
        }

        [SlashCommand("echomessage", "Your next line is...")]
        public static async Task EchoMessage(InteractionContext ctx, [Option("Message", "The message to be echoed by this command")] string message)
        {
            await ctx.CreateResponseAsync(message);//sends back message that was sent
        }

        [SlashCommand("echoreact", "Sends the next reaction this user uses on any message in this channel as a message")]
        public static async Task EchoReaction(InteractionContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();//gets the active interactivity module
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.DeferredChannelMessageWithSource);//holds the request until emoji is reacted
            var message = await interactivity.WaitForReactionAsync(x => x.Channel == ctx.Channel && x.User == ctx.Member);//checks message was in same channel and has same author
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(message.Result.Emoji));//sends back the emoji that was reacted
        }

        [SlashCommand("requestrole", "Requests a role from the admin, request will remain for a maximum of 4 minuites")]
        public static async Task RequestRole(InteractionContext ctx, [Option("rolename", "The role that you want to request")] DiscordRole rolename)
        {
            if (ctx.Channel.Name.Equals("role-requests", StringComparison.CurrentCultureIgnoreCase))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);//holds the request until emoji is reacted
                var interactivity = ctx.Client.GetInteractivity();//loads interactivity modual current state
                var JoinEmbed = new DiscordEmbedBuilder//builds embeded message
                {
                    Title = "Role Request: " + ctx.User.Username,
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Client.CurrentUser.AvatarUrl },
                    Color = DiscordColor.SapGreen,
                    Timestamp = DateTime.Now,
                    Description = "Requests role: " + rolename.Name,
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Role Request" }
                };
                var rolereqmes = await ctx.Channel.SendMessageAsync(embed: JoinEmbed);//sends message
                var thumbup = DiscordEmoji.FromName(ctx.Client, ":+1:");//stores thumbs up emoji
                var thumbdown = DiscordEmoji.FromName(ctx.Client, ":-1:");//stores thumbs down emoji
                await rolereqmes.CreateReactionAsync(thumbup);//reacts to own comment with thumbs up
                await rolereqmes.CreateReactionAsync(thumbdown);//reacts to own comment with thumbs down
                var result = await interactivity.WaitForReactionAsync(x => x.Message == rolereqmes && (x.Emoji == thumbup || x.Emoji == thumbdown) && ctx.Guild.GetMemberAsync(x.User.Id).Result.Permissions.HasPermission(Permissions.Administrator), TimeSpan.Parse("0:00:04:00"));//checks for correct react and user
                if (!result.TimedOut)
                {
                    if (result.Result.Emoji == thumbup)//if thumbs up is given
                    {
                        await ctx.Member.GrantRoleAsync(rolename);//grant role
                        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(ctx.User.Username + "'s request for the role of " + rolename.Name + " has been granted"));
                    }
                    else
                    {
                        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(ctx.User.Username + "'s request for the role of " + rolename.Name + " has been denied"));
                    }
                }
                else
                {
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("The request timed out!"));
                }
                await rolereqmes.DeleteAsync();//delete message
            }
            else
            {
                await ctx.CreateResponseAsync("This request can only be handled in the role request channel!");
            }
        }

        [SlashCommand("giverole", "Gives a person a role")]
        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public static async Task GiveRole(InteractionContext ctx, [Option("personID", "The person to be given a role")] DiscordUser person, [Option("rolename", "The role to be given")] DiscordRole rolename)
        {
            DiscordMember user = ctx.Guild.GetMemberAsync(person.Id).Result;//stores user
            await user.GrantRoleAsync(rolename, "Admin " + ctx.User.Username + " has given you this role!");//give role to desiered user
            await ctx.CreateResponseAsync("The role " + rolename.Name + " has been given to " + user.Username);//role granted message
        }

        [SlashCommand("revokerole", "Removes a role from a person")]
        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public static async Task RevokeRole(InteractionContext ctx, [Option("PersonID", "The Person's ID")] DiscordUser person, [Option("role", "The role to be revoked")] DiscordRole rolename)
        {
            DiscordMember user = ctx.Guild.GetMemberAsync(person.Id).Result;//stores user
            await user.RevokeRoleAsync(rolename, "Admin " + ctx.User.Username + " has revoked this role.");//revoke role from person
            await ctx.CreateResponseAsync("The role " + rolename.Name + " has been revoked from " + user.Username);//user revoked message
        }

        [SlashCommand("createrole", "Creates a new role with default settings")]
        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public static async Task NewRole(InteractionContext ctx, [Option("rolename", "The role to be created")] string rolename)
        {
            await ctx.Guild.CreateRoleAsync(rolename);//creates role with role name
            await ctx.CreateResponseAsync("Role " + rolename + " created!");//role created message
        }

        [SlashCommand("deleterole", "Delete a role")]
        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public static async Task DeleteRole(InteractionContext ctx, [Option("roleID", "The role to be deleted")] DiscordRole rolename)
        {
            await rolename.DeleteAsync();//delete role
            await ctx.CreateResponseAsync("The role " + rolename.Name + " has been deleted!");//respond from role
        }

        [SlashCommand("Poll", "Starts a poll on a topic")]
        public static async Task Poll(InteractionContext ctx, [Option("pollName", "The name of the poll")] string pollname, [Option("Description", "A description of what the poll is about")] string description, [Option("Timespan", "e.g. Timespan in format day:hour:minuite:second 0:00:00:00")] string duration, [Option("emojis", "A list of space seperated emojis")] string emojioptions)
        {

            var interactivity = ctx.Client.GetInteractivity();//gets interactivity module
            List<string> emojiStr = [.. emojioptions.Split(" ")];
            var embed = new DiscordEmbedBuilder//builds embed for poll
            {
                Title = pollname,
                Description = description + "\nTime limit set: " + duration,
                Color = DiscordColor.SapGreen,
                Timestamp = DateTime.Now,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.User.AvatarUrl },
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "Poll" },
                Author = new DiscordEmbedBuilder.EmbedAuthor { Name = ctx.User.Username }
            };
            await ctx.CreateResponseAsync("Poll generated");//holds the request until emoji is reacted
            var pollmessage = await ctx.Channel.SendMessageAsync(embed: embed);//sends embed
            try
            {
                foreach (string emoji in emojiStr)
                {
                    if (emoji.Contains(':'))
                    {
                        string trueEmoji = emoji[..(emoji.LastIndexOf(':') + 1)].Replace("<", "");
                        await pollmessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, trueEmoji));
                    }
                    else
                    {
                        await pollmessage.CreateReactionAsync(DiscordEmoji.FromUnicode(emoji));
                    }
                }
                try
                {
                    TimeSpan timeToPoll = TimeSpan.Parse(duration);
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Poll Started"));//holds the request until emoji is reacted
                    var result = await interactivity.CollectReactionsAsync(pollmessage, timeToPoll);//get poll results
                    var distinctresult = result.Distinct();//parse results for duplicates
                    var results = distinctresult.Select(x => $"{x.Emoji}: {x.Total}");//format results
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Poll Results from poll " + pollname + " (" + description + "):\n" + string.Join("\n", results)));
                    await pollmessage.DeleteAsync();
                }
                catch (Exception)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Invalid Timespan has been entered!"));
                    await pollmessage.DeleteAsync();
                }
            }
            catch (Exception)
            {
                await ctx.CreateResponseAsync("Invalid Emoji Selection, please ensure spaces are used between all emojis and that the emojis exist!");
                await pollmessage.DeleteAsync();
            }

        }
        [SlashCommand("Ban", "Bans a person")]
        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public static async Task Ban(InteractionContext ctx, [Option("personID", "The Person's ID")] DiscordUser person)
        {
            DiscordMember user = await ctx.Guild.GetMemberAsync(person.Id);//stores user
            await ctx.Guild.BanMemberAsync(user, 0, "Admin " + ctx.User.Username + " has banned you.");
            await ctx.CreateResponseAsync(user.Username + " has been banned!");
        }
        [SlashCommand("Unban", "Unbans a person")]
        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public static async Task Unban(InteractionContext ctx, [Option("personID", "The Person's ID")] DiscordUser person)
        {
            DiscordUser member = await ctx.Client.GetUserAsync(person.Id);//gets all users as a list
            await ctx.Guild.UnbanMemberAsync(member, "Admin " + ctx.User.Username + " has unbanned you!");//unbans member
            await ctx.CreateResponseAsync(member.Username + " has been unbanned!");
        }
    }
}
