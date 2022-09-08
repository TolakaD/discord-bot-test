using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBotTest.Attributes;
using DiscordBotTest.Handlers.Dialogue;
using DiscordBotTest.Handlers.Dialogue.Steps;
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
        [RequireCategories(ChannelCheckmode.Any, "Text Channels")]
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

        [Command("dialogue")]
        [Description("Starts a new dialogue")]
        public async Task Dialogue(CommandContext ctx)
        {
            var inputStep = new TextDialogueStep("Enter something interesting!", null);
            var funnyStep = new IntDialogueStep("Haha, that's funny", null, maxValue: 100);

            string input = string.Empty;
            int value = 0;

            inputStep.OnValidResult += (result) =>
            {
                input = result;
                if (result.Equals("Something interesting", StringComparison.OrdinalIgnoreCase))
                {
                    inputStep.SetNextStep(funnyStep);
                }
            };

            funnyStep.OnValidResult += (result) => value = result;

            var inputDialogueHandler = new DialogueHandler(ctx.Client, ctx.Channel, ctx.User, inputStep);

            bool succeeded = await inputDialogueHandler.ProcessDialog();

            if(!succeeded)
                return;

            await ctx.Channel.SendMessageAsync(input);
            await ctx.Channel.SendMessageAsync(value.ToString());
        }

        [Command("emojiidialogue")]
        [Description("Starts a new emojii dialogue")]
        public async Task EmojiiDialogue(CommandContext ctx)
        {
            var yesStep = new TextDialogueStep("You chose Yes", null);
            var noStep = new TextDialogueStep("You chose No", null);

            var emojiiStep = new ReactionDialogueStep("Yes or No?", new Dictionary<DiscordEmoji, ReactionStepData>
            {
                { DiscordEmoji.FromName(ctx.Client, ":thumbsup:"), new ReactionStepData{Content = "This means YES", NexStep = yesStep} },
                { DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"), new ReactionStepData{Content = "This means NO", NexStep = noStep} }
            });
            var inputDialogueHandler = new DialogueHandler(ctx.Client, ctx.Channel, ctx.User, emojiiStep);

            bool succeeded = await inputDialogueHandler.ProcessDialog();

            if (!succeeded)
                return;
        }
    }
}
