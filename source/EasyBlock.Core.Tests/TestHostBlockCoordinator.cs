using System;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestHostBlockCoordinator
    {
        [Test]
        public void Type_ShouldImplement_IHostBlockCoordinator()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(HostBlockCoordinator);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<IHostBlockCoordinator>();

            //---------------Test Result -----------------------
        }

        [TestCase("iniFile", typeof(IINIFile))]
        [TestCase("fileDownloader", typeof(IFileDownloader))]
        [TestCase("hostFileFactory", typeof(IHostFileFactory))]
        [TestCase("textFileReaderFactory", typeof(ITextFileReaderFactory))]
        [TestCase("textFileWriterFactory", typeof(ITextFileWriterFactory))]
        public void Construct_ShouldExpectParameter_(string parameterName, Type parameterType)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ConstructorTestUtils.ShouldExpectNonNullParameterFor<HostBlockCoordinator>(parameterName, parameterType);

            //---------------Test Result -----------------------
        }

        [Test]
        [Ignore("WIP")]
        public void Apply_ShouldAttemptToDownloadAllUrlsInSourcesSectionOfINIFile()
        {
            //---------------Set up test pack-------------------
            var iniData = TextLines(

            );

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }


        private string TextLines(params string[] lines)
        {
            return lines.JoinWith(Environment.NewLine);
        }
    }
}
