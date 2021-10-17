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


        public virtual void Start()
        {
            Running = true;

            Thread = new Thread(DoTick);
            Thread.Start();
        }

        public virtual void Stop()
        {
            Running = false;
        }

        private void DoTick()
        {
            while (Thread.IsAlive && Running)
            {
                TickSynchronous();
                Tick?.Invoke();

                Thread.Sleep(TickRate);
            }
        }

        protected virtual void TickSynchronous() { }


        public virtual int TickRate { get; set; } = 1000;
        protected Thread Thread { get; set; }
        public bool Running { get; set; }
    }
}