using System;
using Castle.Windsor;
using NUnit.Framework;

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
        [Ignore("WIP: container needs to know how to resolve INI factory and ISettings first")]
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
