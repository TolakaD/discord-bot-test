using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBotTest.Handlers.Dialogue.Steps
{
    public abstract class DialogueStep : IDialogueStep
    {
        protected readonly string _content;

        protected DialogueStep(string content)
        {
            _content = content;
        }

        public Action<DiscordMessage> OnMessageAdded { get; set; } = delegate { };
        public abstract IDialogueStep NextStep { get; }
        public abstract Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user);

        protected async Task TryAgain(DiscordChannel channel, string problem)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Please Try Again",
                Color = DiscordColor.Red
            };

            embedBuilder.AddField("There was a problem with your previous input", problem);
            var embed = await channel.SendMessageAsync(embed: embedBuilder);

            OnMessageAdded(embed);
        }
    }
}
