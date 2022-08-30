using DSharpPlus.Entities;

namespace DiscordBotTest.Model
{
    public class Research
    {
        private readonly string _name;

        public Research(string name, string description)
        {
            _name = name;
            Description = description;
        }

        public DiscordEmoji SelectionStatus { get; set; }
        public string Name => $"{SelectionStatus} {_name}";
        public string Description { get; }
    }
}