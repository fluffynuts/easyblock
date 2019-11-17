using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Castle.Windsor;
using EasyBlock.Core;
using EasyBlock.Core.Interfaces;
using EasyBlock.Core.Interfaces.Settings;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.RandomGenerators;
using PeanutButter.ServiceShell;
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
            Assert.AreEqual(container, sut.WindsorContainer);
        }

        [Test]
        public void DefaultConstructor_ShouldUse_ContainerFrom_WindsorBootstrapper()
        {
            // obtuse proof: should resolve the same random INI file setting
            //---------------Set up test pack-------------------
            var asmPath = new Uri(typeof(WindsorBootstrapper).Assembly.CodeBase).LocalPath;
            var asmFolder = Path.GetDirectoryName(asmPath);
            var iniPath = Path.Combine(asmFolder, Constants.CONFIG_FILE);
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
                var result = sut.WindsorContainer.Resolve<IINIFile>()["settings"]["RefreshIntervalInMinutes"];

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
        public void RunOnce_ShouldInjectSelfAsSimpleLoggerBeforeInstructHostBlockCoordinator_To_Apply()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var simpleLoggerFacade = Substitute.For<ISimpleLoggerFacade>();
            var coordinator = Substitute.For<IHostBlockCoordinator>();
            container.Resolve<IHostBlockCoordinator>().Returns(coordinator);
            container.Resolve<ISimpleLoggerFacade>().Returns(simpleLoggerFacade);
            var sut = CreateWithRunOnce(container);

            //---------------Assert Precondition----------------
            coordinator.DidNotReceive().Apply();

            //---------------Execute Test ----------------------
            sut._RunOnce_();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                container.Resolve<ISimpleLoggerFacade>();
                simpleLoggerFacade.SetLogger(sut);
                container.Resolve<IHostBlockCoordinator>();
                coordinator.Apply();
            });
        }

        [Test]
        public void RunOnce_WhenCoordinatorThrows_ShouldLog()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var simpleLoggerFacade = Substitute.For<ISimpleLoggerFacade>();
            var coordinator = Substitute.For<IHostBlockCoordinator>();
            var expected = GetRandomString();
            coordinator.When(c => c.Apply())
                .Do(ci => { throw new Exception(expected); });
            container.Resolve<IHostBlockCoordinator>().Returns(coordinator);
            container.Resolve<ISimpleLoggerFacade>().Returns(simpleLoggerFacade);
            var sut = CreateWithRunOnce(container);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut._RunOnce_();

            //---------------Test Result -----------------------
            simpleLoggerFacade.Received(1).LogFatal(Arg.Is<string>(s => s.StartsWith($"Exception whilst applying block lists: {expected}")));
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


        [Test]
        public void Stop_ShouldInstructCoordinator_ToUnapply()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var coordinator = Substitute.For<IHostBlockCoordinator>();
            container.Resolve<IHostBlockCoordinator>().Returns(coordinator);
            var sut = Create(container);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Stop();

            //---------------Test Result -----------------------
            coordinator.Received(1).Unapply();
        }


        [Test]
        public void Stop_ShouldInjectSelfAsSimpleLoggerBeforeInstructingCoordinator_ToUnapply()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var coordinator = Substitute.For<IHostBlockCoordinator>();
            var logFacade = Substitute.For<ISimpleLoggerFacade>();
            container.Resolve<ISimpleLoggerFacade>().Returns(logFacade);
            container.Resolve<IHostBlockCoordinator>().Returns(coordinator);
            var sut = Create(container);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Stop();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                container.Resolve<ISimpleLoggerFacade>();
                logFacade.SetLogger(sut);
                logFacade.LogInfo("Unapplying blocklists...");
                container.Resolve<IHostBlockCoordinator>();
                coordinator.Unapply();
            });
        }


        [Test]
        public void Stop_WhenExceptionIsThrown_ShouldLogIt()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var coordinator = Substitute.For<IHostBlockCoordinator>();
            var logFacade = Substitute.For<ISimpleLoggerFacade>();
            container.Resolve<ISimpleLoggerFacade>().Returns(logFacade);
            container.Resolve<IHostBlockCoordinator>().Returns(coordinator);
            var expected = GetRandomString();
            coordinator.When(c => c.Unapply())
                .Do(ci => { throw new Exception(expected); });
            var sut = Create(container);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Stop();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                container.Resolve<ISimpleLoggerFacade>();
                logFacade.SetLogger(sut);
                logFacade.LogInfo("Unapplying blocklists...");
                container.Resolve<IHostBlockCoordinator>();
                coordinator.Unapply();
                logFacade.LogFatal($"Unable to Unapply blocklists: {expected}");
            });
        }

        [Test]
        public void Stop_ShouldCallBaseOnStop()
        {
            
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();
            var sut = CreateWithOverriddenLog(container);
            var expected = $"{sut.DisplayName} :: Stopping";

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Stop();

            //---------------Test Result -----------------------
            CollectionAssert.Contains(sut.LogCalls, expected);

        }


        private EasyBlockService_OVERRIDES_Log CreateWithOverriddenLog(IWindsorContainer container)
        {
            return new EasyBlockService_OVERRIDES_Log(container);
        }

        private class EasyBlockService_OVERRIDES_Log: EasyBlockService
        {
            public EasyBlockService_OVERRIDES_Log(IWindsorContainer container): base(container)
            {
            }
            public List<string> LogCalls { get; private set; } = new List<string>();
            public override void Log(string status)
            {
                var stackTrace = new StackTrace();
                var frames = stackTrace.GetFrames();
                var caller = frames.Skip(1).First();
                var callerMethod = caller.GetMethod();
                var callerType = callerMethod.DeclaringType;
                Assert.AreEqual(typeof(Shell), callerType);
                LogCalls.Add(status);
            }
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
