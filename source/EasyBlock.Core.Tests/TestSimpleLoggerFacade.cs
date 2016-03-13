using System;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.ServiceShell;
using PeanutButter.TestUtils.Generic;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestSimpleLoggerFacade
    {
        [Test]
        public void Construct_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => new SimpleLoggerFacade());
            //---------------Test Result -----------------------
        }

        [Test]
        public void Type_ShouldImplement_ISimpleLogger_FromPeanutButterServiceShell()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(SimpleLoggerFacade);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<ISimpleLogger>();

            //---------------Test Result -----------------------
        }

        [Test]
        public void Construct_ShouldHaveFallback_ConsoleWriteLine()
        {
            //---------------Set up test pack-------------------
            var sut = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = sut.Fallback;

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
        }

        [Test]
        public void LogDebug_WhenNoLoggerHasBeenProvided_ShouldLogToFallback()
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            var message = GetRandomString();
            string received = null;
            sut.Fallback = s => received = s;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogDebug(message));

            //---------------Test Result -----------------------
            Assert.AreEqual("DEBUG: " + message, received);
        }

        [Test]
        public void LogDebug_WhenHaveProvidedLogger_ShouldUseIt()
        {
            //---------------Set up test pack-------------------
            var logger = Substitute.For<ISimpleLogger>();
            var message = GetRandomString();
            var sut = Create();
            sut.SetLogger(logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.LogDebug(message);

            //---------------Test Result -----------------------
            logger.Received(1).LogDebug(message);
        }

        [Test]
        public void LogDebug_WhenHaveProvidedLoggerWhichThrows_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var logger = Substitute.For<ISimpleLogger>();
            var message = GetRandomString();
            var sut = Create();
            sut.SetLogger(logger);
            logger.When(l => l.LogDebug(message))
                .Do(ci => { throw new Exception("boo"); });

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogDebug(message));

            //---------------Test Result -----------------------
            logger.Received(1).LogDebug(message);
        }


        [Test]
        public void LogDebug_WhenHaveNoProvidedLoggerAndFallbackThrows_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var message = GetRandomString();
            var sut = Create();
            sut.Fallback = s => { throw new Exception("moo"); };

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogDebug(message));

            //---------------Test Result -----------------------
        }

        [Test]
        public void LogInfo_WhenNoLoggerHasBeenProvided_ShouldLogToFallback()
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            var message = GetRandomString();
            string received = null;
            sut.Fallback = s => received = s;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogInfo(message));

            //---------------Test Result -----------------------
            Assert.AreEqual("INFO: " + message, received);
        }

        [Test]
        public void LogInfo_WhenHaveProvidedLogger_ShouldUseIt()
        {
            //---------------Set up test pack-------------------
            var logger = Substitute.For<ISimpleLogger>();
            var message = GetRandomString();
            var sut = Create();
            sut.SetLogger(logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.LogInfo(message);

            //---------------Test Result -----------------------
            logger.Received(1).LogInfo(message);
        }

        [Test]
        public void LogInfo_WhenHaveProvidedLoggerWhichThrows_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var logger = Substitute.For<ISimpleLogger>();
            var message = GetRandomString();
            var sut = Create();
            sut.SetLogger(logger);
            logger.When(l => l.LogInfo(message))
                .Do(ci => { throw new Exception("boo"); });

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogInfo(message));

            //---------------Test Result -----------------------
            logger.Received(1).LogInfo(message);
        }


        [Test]
        public void LogInfo_WhenHaveNoProvidedLoggerAndFallbackThrows_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var message = GetRandomString();
            var sut = Create();
            sut.Fallback = s => { throw new Exception("moo"); };

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogInfo(message));

            //---------------Test Result -----------------------
        }


        [Test]
        public void LogWarning_WhenNoLoggerHasBeenProvided_ShouldLogToFallback()
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            var message = GetRandomString();
            string received = null;
            sut.Fallback = s => received = s;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogWarning(message));

            //---------------Test Result -----------------------
            Assert.AreEqual("WARNING: " + message, received);
        }

        [Test]
        public void LogWarning_WhenHaveProvidedLogger_ShouldUseIt()
        {
            //---------------Set up test pack-------------------
            var logger = Substitute.For<ISimpleLogger>();
            var message = GetRandomString();
            var sut = Create();
            sut.SetLogger(logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.LogWarning(message);

            //---------------Test Result -----------------------
            logger.Received(1).LogWarning(message);
        }

        [Test]
        public void LogWarning_WhenHaveProvidedLoggerWhichThrows_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var logger = Substitute.For<ISimpleLogger>();
            var message = GetRandomString();
            var sut = Create();
            sut.SetLogger(logger);
            logger.When(l => l.LogWarning(message))
                .Do(ci => { throw new Exception("boo"); });

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogWarning(message));

            //---------------Test Result -----------------------
            logger.Received(1).LogWarning(message);
        }


        [Test]
        public void LogWarning_WhenHaveNoProvidedLoggerAndFallbackThrows_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var message = GetRandomString();
            var sut = Create();
            sut.Fallback = s => { throw new Exception("moo"); };

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogWarning(message));

            //---------------Test Result -----------------------
        }

        [Test]
        public void LogFatal_WhenNoLoggerHasBeenProvided_ShouldLogToFallback()
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            var message = GetRandomString();
            string received = null;
            sut.Fallback = s => received = s;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogFatal(message));

            //---------------Test Result -----------------------
            Assert.AreEqual("FATAL: " + message, received);
        }

        [Test]
        public void LogFatal_WhenHaveProvidedLogger_ShouldUseIt()
        {
            //---------------Set up test pack-------------------
            var logger = Substitute.For<ISimpleLogger>();
            var message = GetRandomString();
            var sut = Create();
            sut.SetLogger(logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.LogFatal(message);

            //---------------Test Result -----------------------
            logger.Received(1).LogFatal(message);
        }

        [Test]
        public void LogFatal_WhenHaveProvidedLoggerWhichThrows_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var logger = Substitute.For<ISimpleLogger>();
            var message = GetRandomString();
            var sut = Create();
            sut.SetLogger(logger);
            logger.When(l => l.LogFatal(message))
                .Do(ci => { throw new Exception("boo"); });

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogFatal(message));

            //---------------Test Result -----------------------
            logger.Received(1).LogFatal(message);
        }


        [Test]
        public void LogFatal_WhenHaveNoProvidedLoggerAndFallbackThrows_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var message = GetRandomString();
            var sut = Create();
            sut.Fallback = s => { throw new Exception("moo"); };

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => sut.LogFatal(message));

            //---------------Test Result -----------------------
        }



        private SimpleLoggerFacade Create()
        {
            return new SimpleLoggerFacade();
        }
    }
}
