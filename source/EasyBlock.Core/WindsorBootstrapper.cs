using Castle.Windsor;
using PeanutButter.Utils.Windsor;

namespace EasyBlock.Core
{
    public class WindsorBootstrapper
    {
        public static IWindsorContainer Bootstrap()
        {
            var container = new WindsorContainer();
            //container.RegisterSingleton<IHostBlockCoordinator, HostBlockCoordinator>();
            container.RegisterAllOneToOneResolutionsAsTransientFrom(typeof(WindsorBootstrapper).Assembly);
            return container;
        }
    }

}
