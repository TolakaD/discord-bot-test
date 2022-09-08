using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DiscordBotTest.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DiscordBotTest
{
    public class Bot
    {
        public Bot(IServiceProvider serviceProvider)
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };

            Client = new DiscordClient(config);
            Client.Ready += OnClientReady;

            var interactivityConfig = new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(5),
                ButtonBehavior = ButtonPaginationBehavior.DeleteButtons
            };
            Client.UseInteractivity(interactivityConfig);

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] { configJson.Prefix },
                EnableMentionPrefix = true,
                //EnableDefaultHelp = false --> will allow to build my own custom help command
                Services = serviceProvider,
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<TeamCommands>();
            Commands.RegisterCommands<TestRSCommands>();
            Commands.RegisterCommands<TestRSCommandsDropDown>();
            Commands.RegisterCommands<RPGCommands>();

            Client.ConnectAsync();
        }

        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
