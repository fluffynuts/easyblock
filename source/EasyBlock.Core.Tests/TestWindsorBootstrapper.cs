using System;
using System.IO;
using Castle.Windsor;
using log4net;
using log4net.Config;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.ServiceShell;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestWindsorBootstrapper
    {
        [Test]
        public void Bootstrap_ShouldReturnContainer()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var container = Create();

            //---------------Test Result -----------------------
            Assert.IsNotNull(container);
            Assert.IsInstanceOf<WindsorContainer>(container);
        }

        [TestCase(typeof(ITextFileReaderFactory), typeof(TextFileReaderFactory))]
        [TestCase(typeof(ITextFileWriterFactory), typeof(TextFileWriterFactory))]
        [TestCase(typeof(IHostFileFactory), typeof(HostFileFactory))]
        [TestCase(typeof(IFileDownloader), typeof(FileDownloader))]
        [TestCase(typeof(ISimpleLoggerFacade), typeof(SimpleLoggerFacade))]
        public void Container_ShouldResolve_(Type serviceType, Type expectedResolution)
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = container.Resolve(serviceType);

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(expectedResolution, result);
        }

        [Test]
        public void Resolve_INIFile_ShouldResolveNewINIFileFromApplicationFolder()
        {
            //---------------Set up test pack-------------------
            var asmPath = new Uri(typeof(WindsorBootstrapper).Assembly.CodeBase).LocalPath;
            var asmFolder = Path.GetDirectoryName(asmPath);
            var iniPath = Path.Combine(asmFolder, Constants.CONFIG_FILE);
            using (new AutoDeleter(iniPath))
            {
                var iniFile = new INIFile(iniPath);
                var expected = GetRandomInt(5, 55).ToString();
                iniFile["settings"]["RefreshIntervalInMinutes"] = expected;
                iniFile.Persist();
                var container = WindsorBootstrapper.Bootstrap();
                //---------------Assert Precondition----------------
                Assert.IsTrue(File.Exists(iniPath));

                //---------------Execute Test ----------------------
                var resolvedIni = container.Resolve<IINIFile>();
                var result = resolvedIni["settings"]["RefreshIntervalInMinutes"];

                //---------------Test Result -----------------------
                Assert.AreEqual(expected, result);

            }
        }

        [Test]
        public void Container_ShouldResolveHostBlockCoordinator_ToSingleton()
        {
            //---------------Set up test pack-------------------
            var container = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result1 = container.Resolve<IHostBlockCoordinator>();
            var result2 = container.Resolve<IHostBlockCoordinator>();

            //---------------Test Result -----------------------
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsInstanceOf<HostBlockCoordinator>(result1);
            Assert.IsInstanceOf<HostBlockCoordinator>(result2);
            Assert.AreEqual(result1, result2);
        }



        private IWindsorContainer Create()
        {
            return WindsorBootstrapper.Bootstrap();
        }
    }
}
