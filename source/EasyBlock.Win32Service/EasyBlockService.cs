using Castle.Windsor;
using EasyBlock.Core;
using PeanutButter.ServiceShell;

namespace EasyBlock.Win32Service
{
    public class EasyBlockService: Shell
    {
        public IWindsorContainer Container => _container;
        private IWindsorContainer _container;

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
            var coordinator = _container.Resolve<IHostBlockCoordinator>();
            coordinator.Apply();
        }

        protected override void OnStop()
        {
            base.OnStop();
            var coordinator = _container.Resolve<IHostBlockCoordinator>();

        }
    }
}