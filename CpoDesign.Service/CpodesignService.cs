using System;
using System.Timers;

namespace CpoDesign.Service
{
    public class CpodesignService
    {
        readonly Timer _timer;
        public CpodesignService()
        {
            _timer = new Timer(10000) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} and all is well", DateTime.Now);
        }
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }
    }
}
