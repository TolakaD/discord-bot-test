using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DiscordBotTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            // Old bot launch
            //var bot = new Bot();
            //bot.RunAsync().GetAwaiter().GetResult();

            // Timer
            //BotTimer timer = new BotTimer();
            //BotTimerTest test = new BotTimerTest(timer);
            //timer.Start();
            //Console.ReadLine();
            //Console.WriteLine(test.NumberOfLogs);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
