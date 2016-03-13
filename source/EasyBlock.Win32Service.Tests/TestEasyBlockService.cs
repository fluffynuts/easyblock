using System;
using System.IO;
using Castle.Windsor;
using EasyBlock.Core;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.RandomGenerators;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Win32Service.Tests
{
    [TestFixture]
    public class TestEasyBlockService
    {
        [Test]
        public void Construct_GivenContainer_ShouldUseIt()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = new EasyBlockService(container);

            //---------------Test Result -----------------------
            Assert.AreEqual(container, sut.Container);
        }

        [Test]
        public void DefaultConstructor_ShouldUse_ContainerFrom_WindsorBootstrapper()
        {
            // obtuse proof: should resolve the same random INI file setting
            //---------------Set up test pack-------------------
            var asmPath = new Uri(typeof(WindsorBootstrapper).Assembly.CodeBase).LocalPath;
            var asmFolder = Path.GetDirectoryName(asmPath);
            var iniPath = Path.Combine(asmFolder, "EasyBlock.ini");
            using (new AutoDeleter(iniPath))
            {
                var iniFile = new INIFile(iniPath);
                iniFile["settings"]["RefreshIntervalInMinutes"] = GetRandomInt(5, 55).ToString();
                iniFile.Persist();
                var referenceContainer = WindsorBootstrapper.Bootstrap();
                var sut = CreateDefault();
                //---------------Assert Precondition----------------
                Assert.IsTrue(File.Exists(iniPath));

                //---------------Execute Test ----------------------
                var resolvedIni = referenceContainer.Resolve<IINIFile>();
                var expected = resolvedIni["settings"]["RefreshIntervalInMinutes"];
                var result = sut.Container.Resolve<IINIFile>()["settings"]["RefreshIntervalInMinutes"];

                //---------------Test Result -----------------------
                Assert.AreEqual(expected, result);
            }
        }

        [Test]
        public void Construct_ShouldSet_DisplayName()
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = sut.DisplayName;

            //---------------Test Result -----------------------
            Assert.AreEqual("EasyBlock AdBlocker", result);
        }

        [Test]
        public void Construct_ShouldSet_ServiceName()
        {
            //---------------Set up test pack-------------------
            var sut = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = sut.ServiceName;

            //---------------Test Result -----------------------
            Assert.AreEqual("EasyBlock", result);
        }

        [Test]
        public void Construct_ShouldSetIntervalFrom_ResolvedSettings()
        {
            //---------------Set up test pack-------------------
            var minutes = GetRandomInt(30, 240);
            var settings = Substitute.For<ISettings>();
            settings.RefreshIntervalInMinutes.Returns(minutes);
            var expected = minutes * 60;
            var container = Substitute.For<IWindsorContainer>();
            container.Resolve<ISettings>().Returns(settings);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(container);
            var result = sut.Interval;

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void RunOnce_ShouldResolveHostBlockCoordinator_FromContainer()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var sut = CreateWithRunOnce(container);

            //---------------Assert Precondition----------------
            container.DidNotReceive().Resolve<IHostBlockCoordinator>();

            //---------------Execute Test ----------------------
            sut._RunOnce_();

            //---------------Test Result -----------------------
            container.Received(1).Resolve<IHostBlockCoordinator>();
        }

        [Test]
        public void RunOnce_ShouldInstructHostBlockCoordinator_To_Apply()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var coordinator = Substitute.For<IHostBlockCoordinator>();
            container.Resolve<IHostBlockCoordinator>().Returns(coordinator);
            var sut = CreateWithRunOnce(container);

            //---------------Assert Precondition----------------
            coordinator.DidNotReceive().Apply();

            //---------------Execute Test ----------------------
            sut._RunOnce_();

            //---------------Test Result -----------------------
            coordinator.Received(1).Apply();

        }

        [Test]
        public void Stop_ShouldResolveHostBlockCoordinator_FromContainer()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var sut = Create(container);

            //---------------Assert Precondition----------------
            container.DidNotReceive().Resolve<IHostBlockCoordinator>();

            //---------------Execute Test ----------------------
            sut.Stop();

            //---------------Test Result -----------------------
            container.Received(1).Resolve<IHostBlockCoordinator>();
        }



        private EasyBlockService_EXPOSES_Internals CreateWithRunOnce(IWindsorContainer container = null)
        {
            return new EasyBlockService_EXPOSES_Internals(container ?? Substitute.For<IWindsorContainer>());
        }


        private class EasyBlockService_EXPOSES_Internals: EasyBlockService
        {
            public EasyBlockService_EXPOSES_Internals(IWindsorContainer container): base(container)
            {
            }

            public void _RunOnce_()
            {
                base.RunOnce();
            }

        }



        private EasyBlockService Create(IWindsorContainer container = null)
        {
            return new EasyBlockService(container ?? Substitute.For<IWindsorContainer>());
        }


        private EasyBlockService CreateDefault()
        {
            return new EasyBlockService();
        }
    }

}
