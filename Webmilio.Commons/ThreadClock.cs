using System;
using System.Threading;

namespace Webmilio.Commons
{
    public class ThreadClock
    {
        private volatile object _lock = new();
        public event Action Tick;


        public ThreadClock()
        {
        }


        public void Start()
        {
            Running = true;

            Thread = new Thread(DoTick);
            Thread.Start();
        }

        public void Stop()
        {
            Running = false;
        }

        private void DoTick()
        {
            while (Thread.IsAlive && Running)
            {
                Tick?.Invoke();
                Thread.Sleep(TickRate);
            }
        }


        public virtual int TickRate { get; set; } = 1000;
        protected Thread Thread { get; set; }
        public bool Running { get; set; }
    }
}