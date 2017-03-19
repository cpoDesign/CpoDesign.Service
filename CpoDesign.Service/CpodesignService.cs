using System.Timers;

namespace CpoDesign.Service
{
    public class CpodesignService
    {
        readonly Timer _timer;
        private IConsoleLogger _logger;
        public CpodesignService(IConsoleLogger logger)
        {
            _logger = logger;
            _timer = new Timer(10000) { AutoReset = true };
            _timer.Elapsed += ElapsedEvent;
        }

        #region Service events
        public void Start()
        {
            _timer.Start();
            _logger.Log("Service started");
        }

        public void Stop()
        {
            _timer.Stop();
            _logger.Log("service stopped");
        } 

        #endregion
        private void ElapsedEvent(object sender, ElapsedEventArgs e)
        {
            _logger.Log("All is well");
        }
    }
}
