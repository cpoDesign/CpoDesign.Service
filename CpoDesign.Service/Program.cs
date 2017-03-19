using Autofac;
using System;
using System.IO;
using Topshelf;
using Topshelf.FileSystemWatcher;
using Topshelf.Autofac;

namespace CpoDesign.Service
{
    public class Program
    {
        private static readonly string _testDir = @"C:\Test\";

        public static void Main()
        {
            IContainer container = ConstructDIContainer();

            HostFactory.Run(x =>
             {
                 x.UseAutofacContainer(container);
                 x.Service<CpodesignService>(s =>
                 {

                     s.ConstructUsingAutofacContainer();
                     s.WhenStopped(tc => tc.Stop());
                     s.WhenStarted((service, host) =>
                     {
                         service.Start();
                         if (!Directory.Exists(_testDir))
                         {
                             Directory.CreateDirectory(_testDir);
                         }

                         var filePath = Path.Combine(_testDir, "testfile.ext");
                         using (FileStream fs = File.Create(filePath))
                         {
                         }
                         return true;
                     });

                     s.WhenFileSystemChanged(configurator =>
                         configurator.AddDirectory(dir =>
                         {
                             dir.Path = _testDir;
                             dir.CreateDir = true;
                             dir.IncludeSubDirectories = true;

                             dir.NotifyFilters = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                         }), FileSystemCreated);
                 });

                 x.RunAsLocalSystem();
                 x.SetDescription("Sample Topshelf Host");
                 x.SetDisplayName("Stuff");
                 x.SetServiceName("Stuff");
             });

            // If your service fails this will show you why. Usually DI Issue
            //Console.ReadKey();
        }

        private static IContainer ConstructDIContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<CpodesignService>().AsSelf();
            builder.RegisterType<ConsoleLogger>().As<IConsoleLogger>();
            var container = builder.Build();
            return container;
        }

        private static void FileSystemCreated(TopshelfFileSystemEventArgs topshelfFileSystemEventArgs)
        {
            Console.WriteLine("New file created! ChangeType = {0} FullPath = {1} Name = {2} FileSystemEventType {3}", topshelfFileSystemEventArgs.ChangeType, topshelfFileSystemEventArgs.FullPath, topshelfFileSystemEventArgs.Name, topshelfFileSystemEventArgs.FileSystemEventType);
        }
    }
}