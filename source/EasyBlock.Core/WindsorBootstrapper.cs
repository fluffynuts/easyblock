using System;
using System.IO;
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
            container.Register(Component.For<IINIFile>()
                                        .UsingFactoryMethod(LoadIniFromApplicationFolder)
                                        .LifestyleTransient());
            container.RegisterSingleton<IHostBlockCoordinator, HostBlockCoordinator>();
            container.RegisterAllOneToOneResolutionsAsTransientFrom(typeof(WindsorBootstrapper).Assembly);
            return container;
        }

        private static IINIFile LoadIniFromApplicationFolder()
        {
            var asmPath = new Uri(typeof(WindsorBootstrapper).Assembly.CodeBase).LocalPath;
            var programFolder = Path.GetDirectoryName(asmPath);
            // ReSharper disable once AssignNullToNotNullAttribute
            var iniPath = Path.Combine(programFolder, Constants.CONFIG_FILE);
            return new INIFile(iniPath);
        }
    }

}
