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
            DisplayName = "Easy Block AdBlocker";
            ServiceName = "EasyBlock";
        }
    }
}