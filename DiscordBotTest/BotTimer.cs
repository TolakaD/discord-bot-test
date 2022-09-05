using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace DiscordBotTest
{
    // 0. Started by command. On start each 5 seconds increase some value by 1. Starting from 0. Should be stopped after 30 seconds. Other command outputs timer value
    // 1. Start on command
    // 2. Start()
    // 3. Stop()
    // 4. 
    public class BotTimer
    {
        private readonly Timer _timer;
        private const int MaxExecutionTimes = 5;
        
        public int ElapsedNumber { get; private set; }
        public Timer Timer => _timer;

        public BotTimer()
        {
            _timer = new Timer();
            Init();
        }

        private void Init()
        {
            _timer.Elapsed += new ElapsedEventHandler(OnElapsedEvent);
            _timer.Interval = 5000;
            _timer.AutoReset = true;
            ElapsedNumber = 1;
        }

        private void OnElapsedEvent(object obj, ElapsedEventArgs e)
        {
            Console.WriteLine($"Timer was executed '{ElapsedNumber}' times");
            if (ElapsedNumber >= MaxExecutionTimes)
            {
                Stop();
                Console.WriteLine("Timer was stopped");
                return;
            }
            ElapsedNumber++;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
