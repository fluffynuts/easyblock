using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyBlock.Core.Implementations;
using EasyBlock.Core.Implementations.HostFiles;
using EasyBlock.Core.Interfaces;
using EasyBlock.Core.Interfaces.Caching;
using EasyBlock.Core.Interfaces.Downloading;
using EasyBlock.Core.Interfaces.HostFiles;
using EasyBlock.Core.Interfaces.Settings;
using EasyBlock.Core.Interfaces.TextReader;
using EasyBlock.Core.Interfaces.TextWriter;
using EasyBlock.Core.Tests.Extensions;
using EasyBlock.Core.Tests.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;
// ReSharper disable PossibleMultipleEnumeration

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
        [TestCase("logger", typeof(ISimpleLoggerFacade))]
        public void Construct_ShouldExpectParameter_(string parameterName, Type parameterType)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ConstructorTestUtils.ShouldExpectNonNullParameterFor<HostBlockCoordinator>(parameterName, parameterType);

            //---------------Test Result -----------------------
        }

        [Test]
        [Ignore("Discovery test")]
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
            var expected = GetRandomCollection(GetRandomHostname, 2, 5);
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
            var blacklist = GetRandomCollection(GetRandomHostname, 2, 5);
            var whitelist = GetRandomCollection(GetRandomHostname, 2, 5);
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

        [Test]
        public void Apply_ShouldLogEachSourceDownload()
        {
            //---------------Set up test pack-------------------
            var settings = Substitute.For<ISettings>();
            var sources = GetRandomCollection(GetRandomHttpUrl, 2, 4);
            settings.Sources.Returns(sources);
            var logger = Substitute.For<ISimpleLoggerFacade>();
            var sut = Create(settings, logger: logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            sources.ForEach(s =>
            {
                logger.Received().LogInfo($"Downloading hosts file: {s}");
            });
        }

        [Test]
        public void Apply_ShouldLogSuccessfulAndFailedAndNullDownloadStates()
        {
            //---------------Set up test pack-------------------
            var settings = Substitute.For<ISettings>();
            var sources = GetRandomCollection(GetRandomHttpUrl, 3, 3);
            settings.Sources.Returns(sources);
            var downloader = Substitute.For<IFileDownloader>();
            var successfulSource = sources.First();
            var nullSource = sources.Skip(1).First();
            var failedSource = sources.Last();
            var expectedFailureString = GetRandomString();
            downloader.DownloadDataAsync(successfulSource).Returns(ci =>
            {
                return Task.Run(() => CreateSuccessfulDownloadResultFor(successfulSource));
            });
            downloader.DownloadDataAsync(failedSource).Returns(ci =>
            {
                return Task.Run(() => CreateFailedDownloadResultFor(failedSource, expectedFailureString));
            });
            downloader.DownloadDataAsync(nullSource).Returns(ci =>
            {
                return Task.Run(() => (IDownloadResult)null);
            });
            var logger = Substitute.For<ISimpleLoggerFacade>();
            var sut = Create(settings, downloader, logger: logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            logger.Received().LogWarning("null result encountered");
            logger.Received().LogInfo($"Successful download: {successfulSource}");
            logger.Received().LogWarning($"Failed download: {failedSource} ({expectedFailureString})");
        }

        [Test]
        public void Apply_ShouldLogWhenCacheManagerCanProvideCachedResult()
        {
            //---------------Set up test pack-------------------
            var settings = Substitute.For<ISettings>();
            var source = GetRandomHttpUrl();
            settings.Sources.Returns(new[] { source });
            var cacheManager = Substitute.For<IBlocklistCacheManager>();
            var reader = Substitute.For<ITextFileReader>();
            reader.SetData("127.0.0.1   a.b.c");
            cacheManager.GetReaderFor(source).Returns(reader);
            var logger = Substitute.For<ISimpleLoggerFacade>();
            var sut = Create(settings, blocklistCacheManager:cacheManager, logger: logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            logger.Received().LogDebug($"Retrieved cached data for {source}");
        }

        [Test]
        public void Apply_ShouldLogWhenCacheManagerCannotProvideCachedResult()
        {
            //---------------Set up test pack-------------------
            var settings = Substitute.For<ISettings>();
            var source = GetRandomHttpUrl();
            settings.Sources.Returns(new[] { source });
            var cacheManager = Substitute.For<IBlocklistCacheManager>();
            cacheManager.GetReaderFor(source).Returns((ITextFileReader)null);
            var logger = Substitute.For<ISimpleLoggerFacade>();
            var sut = Create(settings, blocklistCacheManager:cacheManager, logger: logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            logger.Received().LogWarning($"Unable to retrieve cached data for {source}");
        }

        [Test]
        public void Apply_WhenCannotGetCacheReaderForSource_ShouldNotInstructHostFileToMerge()
        {
            //---------------Set up test pack-------------------
            var hostFile = Substitute.For<IHostFile>();
            var hostFileFactory = CreateHostFileFactoryFor(hostFile);
            var settings = Substitute.For<ISettings>();
            var source = GetRandomHttpUrl();
            settings.Sources.Returns(new[] { source });
            var cacheManager = Substitute.For<IBlocklistCacheManager>();
            cacheManager.GetReaderFor(source).Returns((ITextFileReader)null);
            var logger = Substitute.For<ISimpleLoggerFacade>();
            var sut = Create(settings, 
                            blocklistCacheManager:cacheManager, 
                            hostFileFactory: hostFileFactory,
                            logger: logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            hostFile.DidNotReceive().Merge(Arg.Any<ITextFileReader>());
        }

        [Test]
        public void Apply_ShouldLogWhenOverridingRedirectIp()
        {
            //---------------Set up test pack-------------------
            var settings = Substitute.For<ISettings>();
            settings.Sources.Returns(new[] { GetRandomHttpUrl() });
            settings.RedirectIp.Returns(GetRandomIPv4Address());
            var hostFile = Substitute.For<IHostFile>();
            var hostFileFactory = CreateHostFileFactoryFor(hostFile);
            var logger = Substitute.For<ISimpleLoggerFacade>();
            var sut = Create(settings, hostFileFactory: hostFileFactory, logger: logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                hostFile.Merge(Arg.Any<ITextFileReader>());
                logger.LogInfo($"Redirecting adserver hosts to {settings.RedirectIp}");
                hostFile.SetRedirectIp(settings.RedirectIp);
            });
        }

        [Test]
        public void Apply_ShouldLogHowManyBlacklistHostsAreBeingApplied()
        {
            //---------------Set up test pack-------------------
            var settings = Substitute.For<ISettings>();
            settings.RedirectIp.Returns(GetRandomIPv4Address());
            var blacklist = GetRandomCollection(GetRandomHostname, 3, 5);
            settings.Blacklist.Returns(blacklist);
            var logger = Substitute.For<ISimpleLoggerFacade>();
            var sut = Create(settings, logger: logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                logger.LogInfo($"Redirecting adserver hosts to {settings.RedirectIp}");
                logger.LogInfo($"Applying {blacklist.Count()} blacklist hosts");
            });
        }

        [Test]
        public void Apply_ShouldLogHowManyWhitelistHostsAreBeingApplied()
        {
            //---------------Set up test pack-------------------
            var settings = Substitute.For<ISettings>();
            settings.RedirectIp.Returns(GetRandomIPv4Address());
            var whitelist = GetRandomCollection(GetRandomHostname, 3, 5);
            settings.Whitelist.Returns(whitelist);
            var logger = Substitute.For<ISimpleLoggerFacade>();
            var sut = Create(settings, logger: logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                logger.LogInfo($"Redirecting adserver hosts to {settings.RedirectIp}");
                logger.LogInfo($"Applying {whitelist.Count()} whitelist hosts");
            });
        }

        [Test]
        public void Apply_ShouldLogAfterPersistingHostfile()
        {
            //---------------Set up test pack-------------------
            var settings = Substitute.For<ISettings>();
            settings.HostsFile.Returns(GetRandomWindowsPath());
            var source = GetRandomHttpUrl();
            settings.Sources.Returns(new[] { source });
            var hostFile = Substitute.For<IHostFile>();
            var lines = new[]
            {
                new HostFileLine("# this is a comment", true),
                new HostFileLine("127.0.0.1  a.b.c", false),
                new HostFileLine("127.0.0.1  a.b.d", false)
            };
            hostFile.Lines.Returns(lines);
            
            var hostFileFactory = CreateHostFileFactoryFor(hostFile);
            var logger = Substitute.For<ISimpleLoggerFacade>();
            var sut = Create(settings, hostFileFactory: hostFileFactory, logger: logger);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Apply();

            //---------------Test Result -----------------------
            Received.InOrder(() =>
            {
                hostFile.Persist();
                logger.LogInfo($"Wrote out hosts file to {settings.HostsFile}");
                logger.LogInfo($" -> installed 2 blocked hosts");
            });
        }



        private static IDownloadResult CreateFailedDownloadResultFor(string url, string expectedFailureString)
        {
            var result = Substitute.For<IDownloadResult>();
            result.Data.Returns((byte[]) null);
            result.FailureException.Returns(new Exception(expectedFailureString));
            result.Success.Returns(false);
            result.Url.Returns(url);
            return result;
        }

        private static IDownloadResult CreateSuccessfulDownloadResultFor(string url)
        {
            var result = Substitute.For<IDownloadResult>();
            result.Data.Returns(GetRandomBytes());
            result.FailureException.Returns((Exception) null);
            result.Success.Returns(true);
            result.Url.Returns(url);
            return result;
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
                                            IBlocklistCacheManager blocklistCacheManager = null,
                                            ISimpleLoggerFacade logger = null)
        {
            return new HostBlockCoordinator(
                settings ?? Substitute.For<ISettings>(),
                downloader ?? Substitute.For<IFileDownloader>(),
                hostFileFactory ?? Substitute.For<IHostFileFactory>(),
                textFileReaderFactory ?? Substitute.For<ITextFileReaderFactory>(),
                textFileWriterFactory ?? Substitute.For<ITextFileWriterFactory>(),
                blocklistCacheManager ?? Substitute.For<IBlocklistCacheManager>(),
                logger ?? Substitute.For<ISimpleLoggerFacade>());
        }
    }
}
