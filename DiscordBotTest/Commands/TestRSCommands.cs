using System.Linq;
using System.Threading.Tasks;
using DiscordBotTest.Model;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotTest.Commands
{
    public class TestRSCommands : BaseCommandModule
    {
        [Command("play"), Aliases("p")]
        public async Task Play(CommandContext ctx)
        {
            var arrowUp = DiscordEmoji.FromName(ctx.Client, ":arrow_up:");
            var arrowDown = DiscordEmoji.FromName(ctx.Client, ":arrow_down:");
            var checkMark = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
            var researches = new Researches();

            var researchEmbed = GetResearchEmbed(ctx, researches);
            var joinMessage = await ctx.Channel.SendMessageAsync(embed: researchEmbed);
            await joinMessage.CreateReactionAsync(checkMark);
            await joinMessage.CreateReactionAsync(arrowUp);
            await joinMessage.CreateReactionAsync(arrowDown);

            var interactivity = ctx.Client.GetInteractivity();
            while (true)
            {
                var reactionResult = await interactivity.WaitForReactionAsync(
                    x => x.Message == joinMessage &&
                         x.User == ctx.User &&
                         (x.Emoji == arrowUp || x.Emoji == arrowDown));
                if (reactionResult.TimedOut)
                {
                    break;
                }
                if (reactionResult.Result.Emoji == arrowDown)
                {
                    researches.SelectDown();
                    researchEmbed = UpdateSelection(ctx, researchEmbed, researches);
                    await joinMessage.ModifyAsync(x => { x.Embed = researchEmbed; });
                    await joinMessage.DeleteReactionAsync(arrowDown, ctx.User);
                }
                else if (reactionResult.Result.Emoji == arrowUp)
                {
                    researches.SelectUp();
                    researchEmbed = UpdateSelection(ctx, researchEmbed, researches);
                    await joinMessage.ModifyAsync(x => { x.Embed = researchEmbed; });
                    await joinMessage.DeleteReactionAsync(arrowUp, ctx.User);
                }
            }
        }

        private DiscordEmbedBuilder GetResearchEmbed(CommandContext ctx, Researches researches)
        {
            var exclamationMark = DiscordEmoji.FromName(ctx.Client, ":grey_exclamation:");
            var arrowUp = DiscordEmoji.FromName(ctx.Client, ":arrow_up:");
            var arrowDown = DiscordEmoji.FromName(ctx.Client, ":arrow_down:");
            var checkMark = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");

            var researchEmbed = new DiscordEmbedBuilder
            {
                Title = "Research",
                Color = DiscordColor.DarkBlue,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://runescape.wiki/images/1/13/Invention.png" },
                Author = new DiscordEmbedBuilder.EmbedAuthor { Name = ctx.User.Username, IconUrl = ctx.User.AvatarUrl },
                Description = $"{exclamationMark}Research unlocks new mechanics, skills and items.\n React with {checkMark} to begin researching your selection or refresh the list.\n React with {arrowUp} {arrowDown} to change selection.",
                Footer = new DiscordEmbedBuilder.EmbedFooter { Text = "• Created by Tolaka • Get exclusive dye themes using .patreon • Idle Runescape out now! Use .tutorial to get started on your adventure." },
                
        };

            researchEmbed = UpdateSelection(ctx, researchEmbed, researches);
            
            return researchEmbed;
        }

        private DiscordEmbedBuilder UpdateSelection(CommandContext ctx, DiscordEmbedBuilder embed, Researches researches)
        {
            var checkMark = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
            var emptyBox = DiscordEmoji.FromName(ctx.Client, ":green_square:");

            for (int i = 0; i < researches.Items.Count; i++)
            {
                researches.Items[i].SelectionStatus = researches.SelectedIndex == i
                    ? checkMark
                    : emptyBox;
            }

            embed.ClearFields();
            var updatedFieldTitle = $"{string.Join("\n", researches.Items.Select(x => x.Name))}";
            embed.AddField($"{updatedFieldTitle}", $"{researches.CurrentDescription}");
            

            return embed;
        }
    }
}