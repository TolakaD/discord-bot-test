using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBotTest.Handlers.Dialogue.Steps;
using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBotTest.Handlers.Dialogue
{
    public class DialogueHandler
    {
        private readonly DiscordClient _client;
        private readonly DiscordChannel _channel;
        private readonly DiscordUser _user;
        private IDialogueStep _currentStep;

        public DialogueHandler(
            DiscordClient client,
            DiscordChannel channel,
            DiscordUser user,
            IDialogueStep currentStep)
        {
            _client = client;
            _channel = channel;
            _user = user;
            _currentStep = currentStep;
        }

        private readonly List<DiscordMessage> _messages = new List<DiscordMessage>();

        public async Task<bool> ProcessDialog()
        {
            while (_currentStep != null)
            {
                _currentStep.OnMessageAdded += (message) => _messages.Add(message);

                bool canceled = await _currentStep.ProcessStep(_client, _channel, _user);
                if (canceled)
                {
                    await DeleteMessages();
                    var cancelEmbed = new DiscordEmbedBuilder
                    {
                        Title = "The dialogue has successfully been cancelled",
                        Description = _user.Mention,
                        Color = DiscordColor.Green
                    };

                    await _channel.SendMessageAsync(embed: cancelEmbed);
                    return false;
                }

                _currentStep = _currentStep.NextStep;
            }

            await DeleteMessages();
            return true;
        }

        private async Task DeleteMessages()
        {
            if(_channel.IsPrivate) { return;}

            foreach (var message in _messages)
            {
                await message.DeleteAsync();
            }
        }
    }
}
