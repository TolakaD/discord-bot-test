using System;

namespace DiscordBotTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
            //BotTimer timer = new BotTimer();
            //BotTimerTest test = new BotTimerTest(timer);
            //timer.Start();
            //Console.ReadLine();
            //Console.WriteLine(test.NumberOfLogs);
        }
    }
}
