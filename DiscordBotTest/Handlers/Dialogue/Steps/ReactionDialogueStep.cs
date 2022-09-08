using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotTest.Handlers.Dialogue.Steps
{
    public class ReactionDialogueStep : DialogueStep
    {
        private readonly Dictionary<DiscordEmoji, ReactionStepData> _options;
        private DiscordEmoji _selectedEmojii;

        public ReactionDialogueStep(string content, Dictionary<DiscordEmoji, ReactionStepData> options) : base(content)
        {
            _options = options;
        }

        public override IDialogueStep NextStep => _options[_selectedEmojii].NexStep;

        public override async Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var cancelEmojii = DiscordEmoji.FromName(client, ":x:");
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Please React to this Embed",
                Description = $"{user.Mention}, {_content}",
            };
            embedBuilder.AddField("To stop the Dialogue", "React with :x: emojii");

            var interactivity = client.GetInteractivity();

            while (true)
            {
                var embed = await channel.SendMessageAsync(embed: embedBuilder);
                OnMessageAdded(embed);

                foreach (var emojii in _options.Keys)
                {
                    await embed.CreateReactionAsync(emojii);
                }
                await embed.CreateReactionAsync(cancelEmojii);

                var reactionResult = await interactivity.WaitForReactionAsync(
                    x => _options.ContainsKey(x.Emoji) || x.Emoji == cancelEmojii, embed, user);

                if (reactionResult.Result.Emoji == cancelEmojii)
                {
                    return true;
                }

                _selectedEmojii = reactionResult.Result.Emoji;
                OnValidResult(_selectedEmojii);

                return false;
            }
        }

        public Action<DiscordEmoji> OnValidResult { get; set; } = delegate { };
}

    public class ReactionStepData
    {
        public string Content { get; set; }
        public IDialogueStep NexStep { get; set; }
    }
}
