using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EasyBlock.Core.Implementations;
using EasyBlock.Core.Implementations.IO;
using EasyBlock.Core.Implementations.IO.Settings;
using EasyBlock.Core.Interfaces;
using EasyBlock.Core.Interfaces.IO;
using PeanutButter.INIFile;
using PeanutButter.Utils.Windsor;

namespace EasyBlock.Core
{
    public class WindsorBootstrapper
    {
        public static IWindsorContainer Bootstrap()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IINIFile>()
                                        .UsingFactoryMethod(LoadIniFromApplicationFolder)
                                        .LifestyleTransient());
            container.RegisterSingleton<IHostBlockCoordinator, HostBlockCoordinator>();
            container.RegisterSingleton<ISimpleLoggerFacade, SimpleLoggerFacade>();
            container.RegisterAllOneToOneResolutionsAsTransientFrom(typeof(WindsorBootstrapper).Assembly);
            return container;
        }

        private static IINIFile LoadIniFromApplicationFolder()
        {
            var configGenerator = new StarterConfigGenerator();
            configGenerator.CreateConfigIfNotFound();
            return new INIFile(configGenerator.IniFilePath);
        }
    }

}
