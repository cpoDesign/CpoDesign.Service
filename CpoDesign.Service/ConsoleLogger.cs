using System;

namespace CpoDesign.Service
{
    public class ConsoleLogger : IConsoleLogger
    {
        public void Log(string message)
        {
            Console.WriteLine("{0} {1}", DateTime.Now, message);
        }
    }
}
