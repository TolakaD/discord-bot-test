using System.Threading.Tasks;
using DiscordBotTest.DAL;
using DiscordBotTest.DAL.Models.Items;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTest.Commands
{
    public class TeamCommands : BaseCommandModule
    {
        private readonly RPGContext _context;

        public TeamCommands(RPGContext context)
        {
            _context = context;
        }

        [Command("join")]
        public async Task Join(CommandContext ctx)
        {
            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Would you like to join?",
                Color = DiscordColor.DarkBlue,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Client.CurrentUser.AvatarUrl }
            };

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed);
            var thumbsUp = DiscordEmoji.FromName(ctx.Client, ":+1:");
            var thumbsDown = DiscordEmoji.FromName(ctx.Client, ":-1:");

            await joinMessage.CreateReactionAsync(thumbsUp);
            await joinMessage.CreateReactionAsync(thumbsDown);

            var interactivity = ctx.Client.GetInteractivity();
            var reactionResult = await interactivity.WaitForReactionAsync(
                x => x.Message == joinMessage &&
                     x.User == ctx.User &&
                     (x.Emoji == thumbsUp || x.Emoji == thumbsDown));

            var role = ctx.Guild.GetRole(1011357047855530076);
            if (reactionResult.Result.Emoji == thumbsUp)
            {
                var member = ctx.Member;
                await member.GrantRoleAsync(role).ConfigureAwait(false);
            }
            else
            {
                var member = ctx.Member;
                await member.RevokeRoleAsync(role).ConfigureAwait(false);
            }

            await joinMessage.DeleteAsync();
        }
    }
}