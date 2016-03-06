using Castle.MicroKernel.Registration;
using Castle.Windsor;
using PeanutButter.INIFile;
using PeanutButter.Utils.Windsor;

namespace EasyBlock.Core
{
    public class WindsorBootstrapper
    {
        public static IWindsorContainer Bootstrap()
        {
            var container = new WindsorContainer();
            //container.Register(Component.For<IINIFile>()
            //                            .UsingFactory(() => LoadINIFromApplicationFolder
            container.RegisterSingleton<IHostBlockCoordinator, HostBlockCoordinator>();
            container.RegisterAllOneToOneResolutionsAsTransientFrom(typeof(WindsorBootstrapper).Assembly);
            return container;
        }
    }

}
