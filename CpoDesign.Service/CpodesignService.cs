using Akka.Actor;
using System.Timers;
using System;
using Akka.DI.AutoFac;
using BulkProcessor.Actors.BatchesProcessor;
using BulkProcessor.Actors;
using BulkProcessor.Actors.SystemMessages;
using Autofac;
using BulkProcessor.DI;
using BulkProcessor.DataAccess;
using BulkProcessor.Actors.BatchesProcessor.BulkProcessor.BatchTypeManager.Payments;

namespace CpoDesign.Service
{

    public class CpodesignService
    {
        private static ActorSystem BulkProcessingActorSystem;
        const string SystemName = "BulkProcessingActorSystem";

        readonly Timer _timer;
        private IConsoleLogger _logger;
        public CpodesignService(IConsoleLogger logger)
        {
            _logger = logger;
            _timer = new Timer(10000) { AutoReset = true };
            _timer.Elapsed += ElapsedEvent;
            SetupActorSystem();
        }

        private void SetupActorSystem()
        {
            _logger.Log("Creating BulkProcessingActorSystem");
            BulkProcessingActorSystem = ActorSystem.Create(SystemName);

            // register DI container to the system as : IDependencyResolver
            var container = CreateAutofacContainer();
            new AutoFacDependencyResolver(container, BulkProcessingActorSystem);

            _logger.Log("Creating actor supervisory hierarchy");
            // setup the system
            BulkProcessingActorSystem.ActorOf(Props.Create<BulkProcessorActor>(), "BulkProcessorActor");
            
            // send message to start processing the data
            var batchesManager = BulkProcessingActorSystem.ActorOf(Props.Create<BatchesManagerActor>(), "BatchesManagerActor");
            BulkProcessingActorSystem.Scheduler
                .Schedule(TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(30),
                    batchesManager,
                    new StartBulkProcessingMessage());
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

        private static IContainer CreateAutofacContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SystemConfig>().As<ISystemConfig>();
            builder.RegisterType<DemoPaymentGateway>().As<IPaymentGateway>();
            builder.RegisterType<PersonDataAccess>().As<IPersonDataAccess>();
            builder.RegisterType<PaymentWorkerActor>();
            builder.RegisterType<PaymentJobCoordinatorActor>();
            builder.RegisterType<ConfigActor>();
            builder.RegisterType<PersonCreatorWorker>();
            builder.RegisterType<PeopleJobCoordinatorActor>();

            var container = builder.Build();
            return container;
        }

        public static void ShortPause()
        {
            System.Threading.Thread.Sleep(1000);
        }
    }
}
