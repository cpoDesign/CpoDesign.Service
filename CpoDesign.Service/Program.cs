using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Topshelf;
using Topshelf.FileSystemWatcher;

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

    public class Program
    {

        private static readonly string _testDir = @"C:\Test\";

        public static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<CpodesignService>(s =>
                {
                    s.ConstructUsing(name => new CpodesignService());
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

        }
        private static void FileSystemCreated(TopshelfFileSystemEventArgs topshelfFileSystemEventArgs)
        {
            Console.WriteLine("New file created! ChangeType = {0} FullPath = {1} Name = {2} FileSystemEventType {3}", topshelfFileSystemEventArgs.ChangeType, topshelfFileSystemEventArgs.FullPath, topshelfFileSystemEventArgs.Name, topshelfFileSystemEventArgs.FileSystemEventType);
        }
    }
}
