using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotTest.Handlers.Dialogue.Steps
{
    public class IntDialogueStep : DialogueStep
    {
        private readonly string _content;
        private IDialogueStep _nextStep;
        private readonly int? _minValue;
        private readonly int? _maxValue;

        public IntDialogueStep(
            string content,
            IDialogueStep nextStep,
            int? minValue = null,
            int? maxValue = null) : base(content)
        {
            _content = content;
            _nextStep = nextStep;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public Action<int> OnValidResult { get; set; } = delegate { };

        public void SetNextStep(IDialogueStep nextStep)
        {
            _nextStep = nextStep;
        }

        public override IDialogueStep NextStep => _nextStep;
        public override async Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Please respond below",
                Description = $"{user.Mention}, {_content}"
            };

            embedBuilder.AddField("To stop the Dialogue", "Use the .cancel command");

            if (_minValue.HasValue)
            {
                embedBuilder.AddField($"Min Value:", $"{_minValue.Value}");
            }
            if (_maxValue.HasValue)
            {
                embedBuilder.AddField($"Max Value:", $"{_maxValue.Value}");
            }

            var interactivity = client.GetInteractivity();
            while (true)
            {
                var embed = await channel.SendMessageAsync(embed: embedBuilder);
                OnMessageAdded(embed);

                var messageResult = await interactivity.WaitForMessageAsync(x => 
                    x.ChannelId == channel.Id && x.Author.Id == user.Id);
                OnMessageAdded(messageResult.Result);

                if (messageResult.Result.Content.Equals(".cancel", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (!int.TryParse(messageResult.Result.Content, out var inputResult))
                {
                    await TryAgain(channel, "Your input is not an integer");
                    continue;
                }
                if (_minValue.HasValue)
                {
                    if (inputResult < _minValue.Value)
                    {
                        await TryAgain(channel,
                            $"Your input value is smaller than: {_minValue.Value}");
                        continue;
                    }
                }
                if (_maxValue.HasValue)
                {
                    if (messageResult.Result.Content.Length > _maxValue.Value)
                    {
                        await TryAgain(channel,
                            $"Your input value is greater than {_maxValue.Value}");
                        continue;
                    }
                }

                OnValidResult(inputResult);
                return false;
            }
        }
    }
}
