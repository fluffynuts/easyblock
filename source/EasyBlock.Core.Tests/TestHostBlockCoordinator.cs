using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;
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

        [TestCase("applicationConfiguration", typeof(IApplicationConfiguration))]
        [TestCase("fileDownloader", typeof(IFileDownloader))]
        [TestCase("hostFileFactory", typeof(IHostFileFactory))]
        [TestCase("textFileReaderFactory", typeof(ITextFileReaderFactory))]
        [TestCase("textFileWriterFactory", typeof(ITextFileWriterFactory))]
        [TestCase("blocklistCacheManager", typeof(IBlocklistCacheManager))]
        public void Construct_ShouldExpectParameter_(string parameterName, Type parameterType)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ConstructorTestUtils.ShouldExpectNonNullParameterFor<HostBlockCoordinator>(parameterName, parameterType);

            //---------------Test Result -----------------------
        }

        [Test]
        public void Discovery_DealingWithEnvironmentVariableExpansionInStrings()
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
            var config = Substitute.For<IApplicationConfiguration>();
            config.Sources.Returns(new[] { url1, url2 });
            var downloader = Substitute.For<IFileDownloader>();
            var sut = Create(config, downloader);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            downloader.Received().DownloadDataAsync(url1);
            downloader.Received().DownloadDataAsync(url2);
        }

        [Test]
        public void Apply_ShouldPassSuccessfulResultsOnToCacheManager()
        {
            //---------------Set up test pack-------------------
            var source1 = GetRandomHttpUrl();
            var source2 = GetRandomHttpUrl();
            var config = ApplicationConfigurationBuilder.Create()
                            .WithSources(source1, source2)
                            .Build();
            var downloader = Substitute.For<IFileDownloader>();
            var result1 = GetRandomBytes();
            var result2 = GetRandomBytes();
            downloader.SetDownloadResult(source1, result1);
            downloader.SetDownloadResult(source2, result2);
            var cacheManager = Substitute.For<IBlocklistCacheManager>();
            var sut = Create(config, downloader,blocklistCacheManager:cacheManager);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            cacheManager.Received().Set(source1, result1);
            cacheManager.Received().Set(source2, result2);
        }

        [Test]
        public void Apply_ShouldNotPassFailedResultsOnToCacheManager()
        {
            //---------------Set up test pack-------------------
            var source1 = GetRandomHttpUrl();
            var source2 = GetRandomHttpUrl();
            var config = ApplicationConfigurationBuilder.Create()
                            .WithSources(source1, source2)
                            .Build();
            var downloader = Substitute.For<IFileDownloader>();
            var result1 = GetRandomBytes();
            downloader.SetDownloadResult(source1, result1);
            downloader.SetFailedDownloadResultFor(source2);

            var cacheManager = Substitute.For<IBlocklistCacheManager>();
            var sut = Create(config, downloader,blocklistCacheManager:cacheManager);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            cacheManager.Received().Set(source1, result1);
            cacheManager.DidNotReceive().Set(source2, Arg.Any<byte[]>());
        }

        [Test]
        public void Apply_WhenHaveNoBlackListsOrWhiteLists_ShouldMergeAllHostsFromAllSourcesKnownToCacheManager()
        {
            //---------------Set up test pack-------------------
            var source1 = GetRandomHttpUrl();
            var source2 = GetRandomHttpUrl();
            var expectedIp1 = GetRandomIPv4Address();
            var expectedHost1 = GetRandomHostname();
            var expectedIp2 = GetRandomIPv4Address();
            var expectedHost2 = GetRandomHostname();
            var cacheManager = Substitute.For<IBlocklistCacheManager>();
            var mergeReader1 = ReaderFor($"{expectedIp1}   {expectedHost1}");
            cacheManager.GetReaderFor(source1).Returns(mergeReader1);
            var mergeReader2 = ReaderFor($"{expectedIp2}   {expectedHost2}");
            cacheManager.GetReaderFor(source2).Returns(mergeReader2);
            var config = Substitute.For<IApplicationConfiguration>();
            config.Sources.Returns(new[] { source1, source2 });
            var hostFilePath = GetRandomWindowsPath();
            config.HostsFile.Returns(hostFilePath);
            var reader = Substitute.For<ITextFileReader>();
            var writer = Substitute.For<ITextFileWriter>();
            var readerFactory = Substitute.For<ITextFileReaderFactory>();
            readerFactory.Open(hostFilePath).Returns(reader);
            var writerFactory = Substitute.For<ITextFileWriterFactory>();
            writerFactory.Open(hostFilePath).Returns(writer);
            var hostFileFactory = Substitute.For<IHostFileFactory>();
            var hostFile = Substitute.For<IHostFile>();
            hostFileFactory.Create(reader, writer).Returns(hostFile);
            var sut = Create(config, 
                                hostFileFactory:hostFileFactory, 
                                blocklistCacheManager:cacheManager,
                                textFileReaderFactory: readerFactory,
                                textFileWriterFactory: writerFactory);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                readerFactory.Open(hostFilePath);
                writerFactory.Open(hostFilePath);
                hostFileFactory.Create(reader, writer);

                cacheManager.GetReaderFor(source1);
                hostFile.Merge(mergeReader1);

                cacheManager.GetReaderFor(source2);
                hostFile.Merge(mergeReader2);

                hostFile.Persist();

            });
        }

        [Test]
        [Ignore("WIP")]
        public void Apply_ShouldAddUserBlackListItems()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }

        [Test]
        [Ignore("WIP")]
        public void Apply_ShouldNotApplyHostsWhichAreInUserWhiteListByRegEx()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }




        [Test]
        [Ignore("WIP")]
        public void Unapply_ShouldReturnHostsFileBackToOriginalState()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }


        private ITextFileReader ReaderFor(params string[] lines)
        {
            var reader = Substitute.For<ITextFileReader>();
            reader.SetData(lines);
            return reader;
        }


        public class ApplicationConfigurationBuilder: GenericBuilder<ApplicationConfigurationBuilder, IApplicationConfiguration>
        {
            public override IApplicationConfiguration ConstructEntity()
            {
                return Substitute.For<IApplicationConfiguration>();
            }

            public ApplicationConfigurationBuilder()
            {
                WithInterval(Constants.Defaults.ONE_DAY)
                    .WithHostFile(Constants.Defaults.WINDOWS_HOSTS_FILE_LOCATION);
            }

            public ApplicationConfigurationBuilder WithHostFile(string path)
            {
                return WithProp(o => o.HostsFile.Returns(path));
            }

            public ApplicationConfigurationBuilder WithInterval(int interval)
            {
                return WithProp(o => o.RefreshIntervalInMinutes.Returns(interval));
            }

            public ApplicationConfigurationBuilder WithSources(params string[] sources)
            {
                return WithProp(o => o.Sources.Returns(sources));
            }
        }

        private IHostBlockCoordinator Create(IApplicationConfiguration applicationConfiguration = null, 
                                            IFileDownloader downloader = null,
                                            IHostFileFactory hostFileFactory = null,
                                            ITextFileReaderFactory textFileReaderFactory = null,
                                            ITextFileWriterFactory textFileWriterFactory = null,
                                            IBlocklistCacheManager blocklistCacheManager = null)
        {
            return new HostBlockCoordinator(
                applicationConfiguration ?? Substitute.For<IApplicationConfiguration>(),
                downloader ?? Substitute.For<IFileDownloader>(),
                hostFileFactory ?? Substitute.For<IHostFileFactory>(),
                textFileReaderFactory ?? Substitute.For<ITextFileReaderFactory>(),
                textFileWriterFactory ?? Substitute.For<ITextFileWriterFactory>(),
                blocklistCacheManager ?? Substitute.For<IBlocklistCacheManager>());
        }

        private IINIFile CreateIniFileFor(string[] iniData)
        {
            var iniFile = new INIFile();
            iniFile.Parse(string.Join(Environment.NewLine, iniData));
            return iniFile;
        }
    }

    public static class FileDownloaderExtensions
    {
        public static void SetDownloadResult(this IFileDownloader downloader, string url, byte[] data)
        {
            var downloadResult = Substitute.For<IDownloadResult>();
            downloadResult.Success.Returns(true);
            downloadResult.Data.Returns(data);
            downloadResult.Url.Returns(url);
            downloader.SetDownloadResult(url, downloadResult);
        }

        public static void SetDownloadResult(this IFileDownloader downloader, string url, IDownloadResult downloadResult)
        {
            downloader.DownloadDataAsync(url).Returns(Task.Run(() => downloadResult));
        }

        public static void SetFailedDownloadResultFor(this IFileDownloader downloader, string url)
        {
            var downloadResult = Substitute.For<IDownloadResult>();
            downloadResult.Url.Returns(url);
            downloadResult.Success.Returns(false);
            downloadResult.Data.Returns((byte[])null);
            downloader.SetDownloadResult(url, downloadResult);
        }
    }
}
