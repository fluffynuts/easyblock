using System;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.INIFile;
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

        [TestCase("settings", typeof(ISettings))]
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
            var config = Substitute.For<ISettings>();
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
            var config = SettingsBuilder.Create()
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
            var config = SettingsBuilder.Create()
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
            var hostFilePath = GetRandomWindowsPath();
            var settings = SettingsBuilder.Create()
                                .WithSources(source1, source2)
                                .WithHostFile(hostFilePath)
                                .WithRedirectIp(GetRandomIPv4Address())
                                .Build();
            var reader = Substitute.For<ITextFileReader>();
            var writer = Substitute.For<ITextFileWriter>();
            var readerFactory = CreateReaderFactoryFor(hostFilePath, reader);
            var writerFactory = CreateWriterFactoryFor(hostFilePath, writer);
            var hostFile = Substitute.For<IHostFile>();
            var hostFileFactory = CreateHostFileFactoryFor(hostFile);
            var sut = Create(settings, 
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

                hostFile.SetRedirectIp(settings.RedirectIp);

                hostFile.Persist();

            });
        }

        [Test]
        public void Apply_ShouldAddUserBlacklistItems()
        {
            //---------------Set up test pack-------------------
            var expected = GetRandomCollection<string>(GetRandomHostname, 2, 5);
            var settings = SettingsBuilder.Create()
                                .WithRedirectIp(GetRandomIPv4Address())
                                .WithBlacklist(expected.ToArray())
                                .Build();
            var hostFile = Substitute.For<IHostFile>();
            var factory = CreateHostFileFactoryFor(hostFile);
            var sut = Create(settings, hostFileFactory: factory);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                expected.ForEach(e =>
                    hostFile.Redirect(e, settings.RedirectIp));
                hostFile.Persist();
            });
        }

        [Test]
        public void Apply_ShouldApplyUserWhitelistItems()
        {
            //---------------Set up test pack-------------------
            var blacklist = GetRandomCollection<string>(GetRandomHostname, 2, 5);
            var whitelist = GetRandomCollection<string>(GetRandomHostname, 2, 5);
            var settings = SettingsBuilder.Create()
                                .WithRedirectIp(GetRandomIPv4Address())
                                .WithWhitelist(whitelist.ToArray())
                                .WithBlacklist(blacklist.ToArray())
                                .Build();
            var hostFile = Substitute.For<IHostFile>();
            var factory = CreateHostFileFactoryFor(hostFile);
            var sut = Create(settings, hostFileFactory: factory);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                blacklist.ForEach(e =>
                    hostFile.Redirect(e, settings.RedirectIp));
                whitelist.ForEach(e =>
                    hostFile.Whitelist(e));
                hostFile.Persist();
            });
        }


        [Test]
        public void Unapply_ShouldReturnHostsFileBackToOriginalState()
        {
            //---------------Set up test pack-------------------
            var settings = SettingsBuilder.Create()
                                .WithHostFile(GetRandomWindowsPath())
                                .Build();
            var reader = Substitute.For<ITextFileReader>();
            var writer = Substitute.For<ITextFileWriter>();
            var readerFactory = CreateReaderFactoryFor(settings.HostsFile, reader);
            var writerFactory = CreateWriterFactoryFor(settings.HostsFile, writer);
            var unexpected1 = GetRandomHostname();
            var unexpected2 = GetAnother(unexpected1, GetRandomHostname);
            reader.SetData(
                "# original header",
                "127.0.0.1       localhost",
                "192.168.1.100   squishy",
                Constants.MERGE_MARKER,
                $"127.0.0.1       {unexpected1}",
                $"127.0.0.1       {unexpected2}"
            );
            var hostFile = Substitute.For<IHostFile>();
            var hostFileFactory = CreateHostFileFactoryFor(hostFile);
            var sut = Create(settings, 
                                textFileReaderFactory: readerFactory, 
                                textFileWriterFactory: writerFactory,
                                hostFileFactory: hostFileFactory);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Unapply();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                readerFactory.Open(settings.HostsFile);
                writerFactory.Open(settings.HostsFile);
                hostFileFactory.Create(reader, writer);
                hostFile.Revert();
                hostFile.Persist();
            });
        }


        private ITextFileReaderFactory CreateReaderFactoryFor(string path, ITextFileReader reader)
        {
            var factory = Substitute.For<ITextFileReaderFactory>();
            factory.Open(path).Returns(reader);
            return factory;
        }

        private ITextFileWriterFactory CreateWriterFactoryFor(string path, ITextFileWriter reader)
        {
            var factory = Substitute.For<ITextFileWriterFactory>();
            factory.Open(path).Returns(reader);
            return factory;
        }

        private IHostFileFactory CreateHostFileFactoryFor(IHostFile hostFile)
        {
            var factory = Substitute.For<IHostFileFactory>();
            factory.Create(Arg.Any<ITextFileReader>(), Arg.Any<ITextFileWriter>())
                .Returns(hostFile);
            return factory;
        }

        private ITextFileReader ReaderFor(params string[] lines)
        {
            var reader = Substitute.For<ITextFileReader>();
            reader.SetData(lines);
            return reader;
        }


        private IHostBlockCoordinator Create(ISettings settings = null, 
                                            IFileDownloader downloader = null,
                                            IHostFileFactory hostFileFactory = null,
                                            ITextFileReaderFactory textFileReaderFactory = null,
                                            ITextFileWriterFactory textFileWriterFactory = null,
                                            IBlocklistCacheManager blocklistCacheManager = null)
        {
            return new HostBlockCoordinator(
                settings ?? Substitute.For<ISettings>(),
                downloader ?? Substitute.For<IFileDownloader>(),
                hostFileFactory ?? Substitute.For<IHostFileFactory>(),
                textFileReaderFactory ?? Substitute.For<ITextFileReaderFactory>(),
                textFileWriterFactory ?? Substitute.For<ITextFileWriterFactory>(),
                blocklistCacheManager ?? Substitute.For<IBlocklistCacheManager>());
        }
    }
}
