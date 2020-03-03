using Autofac;
using Serilog;
using System;
using System.Timers;
using Topshelf;
using Topshelf.Autofac;

namespace CpoDesign.Service.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(windowsService =>
            {
                windowsService.UseAutofacContainer(ConstructDIContainer());

                windowsService.Service<ServiceExample>(s =>
                {
                    s.ConstructUsingAutofacContainer();
                    // s.ConstructUsing(service => new ServiceExample());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                windowsService.RunAsLocalSystem();
                windowsService.StartAutomatically();

                windowsService.SetDescription("TopshelfDotNetCoreExample");
                windowsService.SetDisplayName("TopshelfDotNetCoreExample");
                windowsService.SetServiceName("TopshelfDotNetCoreExample");
            });
        }

        private static IContainer ConstructDIContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ServiceExample>().AsSelf();
            builder.RegisterType<ConsoleLogger>().As<IConsoleLogger>();
            var container = builder.Build();
            return container;
        }

        class ServiceExample
        {
            private Timer timer;
            private int count;

            public ServiceExample(IConsoleLogger consoleLogger)
            {
                this.timer = new Timer() { AutoReset = true, Interval = 1000 };
                this.timer.Elapsed += (s, e) => { Log.Information($"Count = {this.count++}");
                    consoleLogger.Log("Test");
                };
            }

            public void Start()
            {
                Log.Information("Started");
                this.timer.Start();
            }

            public void Stop()
            {
                Log.Information("Stopped");
                this.timer.Stop();
            }
        }
    }
    public class ConsoleLogger : IConsoleLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {message}");
        }
    }
    public interface IConsoleLogger
    {
        void Log(string message);
    }
}
