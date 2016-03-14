using Castle.Windsor;
using EasyBlock.Core;
using PeanutButter.ServiceShell;

namespace EasyBlock.Win32Service
{
    public class EasyBlockService: Shell
    {
        public IWindsorContainer WindsorContainer => _container;
        private readonly IWindsorContainer _container;

        public EasyBlockService(): this(WindsorBootstrapper.Bootstrap())
        {
        }

        public EasyBlockService(IWindsorContainer container)
        {
            _container = container;
            DisplayName = "EasyBlock AdBlocker";
            ServiceName = "EasyBlock";
            _container = container;
            Interval = _container.Resolve<ISettings>().RefreshIntervalInMinutes * 60;
        }

        protected override void RunOnce()
        {
            SetupLogging();
            var coordinator = _container.Resolve<IHostBlockCoordinator>();
            coordinator.Apply();
        }

        private void SetupLogging()
        {
            var loggerFacade = _container.Resolve<ISimpleLoggerFacade>();
            loggerFacade.SetLogger(this);
        }

        protected override void OnStop()
        {
            base.OnStop();
            SetupLogging();
            var coordinator = _container.Resolve<IHostBlockCoordinator>();
            coordinator.Unapply();
        }
    }
}