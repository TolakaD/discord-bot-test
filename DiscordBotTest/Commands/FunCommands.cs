using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotTest.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns 'Pong'")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong");
        }

        [Command("add")]
        [Description("Adds two numbers")]
        //[RequireRoles(RoleCheckMode.Any, "Owner")] --> Limit a command to a specific role
        public async Task Add(CommandContext ctx,
            [Description("First number to add")] int firstNumber,
            [Description("Second number to add")] int secondNumber)
        {
            await ctx.Channel.SendMessageAsync($"Result: {firstNumber + secondNumber}");
        }

        [Command("response")]
        public async Task Response(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel);

            await ctx.Channel.SendMessageAsync($"Your message: {message.Result.Content}");
        }

        [Command("respondReaction")]
        public async Task RespondReaction(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity.WaitForReactionAsync(x => x.Channel == ctx.Channel);

            await ctx.Channel.SendMessageAsync($"Your message: {message.Result.Emoji}");
        }

        [Command("poll")]
        public async Task Poll(CommandContext ctx, TimeSpan duration, params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(o => o.ToString());

            var embed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: embed);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration);
            var results = result.Select(r => $"{r.Emoji}: {r.Total}" );
            await ctx.Channel.SendMessageAsync(string.Join("\n", results));
        }
    }
}
