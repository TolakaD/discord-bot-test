using System.Timers;

namespace DiscordBotTest
{
    public class BotTimerTest
    {
        private BotTimer _botTimer;
        public int NumberOfLogs { get; set; }

        public BotTimerTest(BotTimer botTimer)
        {
            _botTimer = botTimer;
            _botTimer.Timer.Elapsed += new ElapsedEventHandler(UpdateNumberOfLogs);
        }

        private void UpdateNumberOfLogs(object obj, ElapsedEventArgs e)
        {
            NumberOfLogs += 10;
        }
    }
}
