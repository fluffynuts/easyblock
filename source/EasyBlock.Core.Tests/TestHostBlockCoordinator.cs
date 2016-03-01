using System;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.TestUtils.Generic;
using static PeanutButter.RandomGenerators.RandomValueGen;

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
        [Ignore("Discovery to do with variable expansion, etc")]
        public void ReadExistingHosts()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var raw = "%SYSTEMROOT%\\system32\\drivers\\etc\\hosts";
            var path = Environment.ExpandEnvironmentVariables(raw);
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }


        [Test]
        public void Apply_ShouldAttemptToDownloadAllUrlsInSourcesSectionOfINIFile()
        {
            //---------------Set up test pack-------------------
            var url1 = GetRandomHttpUrl();
            var url2 = GetRandomHttpUrl();
            var ini = IniBuilder.Create()
                            .WithSources(url1, url2)
                            .Build();
            var downloader = Substitute.For<IFileDownloader>();
            var sut = Create(ini, downloader);

            //---------------Assert Precondition----------------
            var iniSources = ini["sources"].Keys.ToArray();
            Assert.AreEqual(url1, iniSources[0]);
            Assert.AreEqual(url2, iniSources[1]);

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            downloader.Received().DownloadDataAsync(url1);
            downloader.Received().DownloadDataAsync(url2);
        }

        [Test]
        [Ignore("WIP: requires IBlocklistCacheManager")]
        public void Apply_ShouldPassResultsOnToCacheManager()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }


        private IHostBlockCoordinator Create(IINIFile ini = null, 
                                            IFileDownloader downloader = null,
                                            IHostFileFactory hostFileFactory = null,
                                            ITextFileReaderFactory textFileReaderFactory = null,
                                            ITextFileWriterFactory textFileWriterFactory = null)
        {
            return new HostBlockCoordinator(ini ?? Substitute.For<IINIFile>(),
                                            downloader ?? Substitute.For<IFileDownloader>(),
                                            hostFileFactory ?? Substitute.For<IHostFileFactory>(),
                                            textFileReaderFactory ?? Substitute.For<ITextFileReaderFactory>(),
                                            textFileWriterFactory ?? Substitute.For<ITextFileWriterFactory>());
        }

        private IINIFile CreateIniFileFor(string[] iniData)
        {
            var iniFile = new INIFile();
            iniFile.Parse(string.Join(Environment.NewLine, iniData));
            return iniFile;
        }
    }
}
