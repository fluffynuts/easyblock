using System;
using System.IO;
using System.Linq;
using EasyBlock.Core.Extensions;
using EasyBlock.Core.Implementations.Caching;
using EasyBlock.Core.Implementations.TextReader;
using EasyBlock.Core.Interfaces.Caching;
using EasyBlock.Core.Interfaces.Settings;
using EasyBlock.Core.Interfaces.TextReader;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests.Caching
{
    [TestFixture]
    public class TestBlocklistCacheManager
    {
        [Test]
        public void Type_ShouldImplement_IBlocklistCacheManager()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(BlocklistCacheManager);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<IBlocklistCacheManager>();

            //---------------Test Result -----------------------
        }

        [TestCase("cacheFilenameGenerator", typeof(ICacheFilenameGenerator))]
        [TestCase("readerFactory", typeof(ITextFileReaderFactory))]
        [TestCase("settings", typeof(ISettings))]
        public void Construct_ShouldExpectParameter_(string parameterName, Type parameterType)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ConstructorTestUtils.ShouldExpectNonNullParameterFor<BlocklistCacheManager>(parameterName, parameterType);

            //---------------Test Result -----------------------
        }


        [Test]
        public void Set_GivenSourceAndData_ShouldWriteDataToCacheLocation()
        {
            //---------------Set up test pack-------------------
            var sourceUrl = GetRandomHttpUrl();
            var expected = GetRandomBytes();
            var cacheFilenameGenerator = Substitute.For<ICacheFilenameGenerator>();
            using (var tempFile = new AutoTempFile())
            {
                cacheFilenameGenerator.GenerateFor(sourceUrl)
                                        .Returns(tempFile.Path);
                var sut = Create(cacheFilenameGenerator);
                //---------------Assert Precondition----------------
                CollectionAssert.IsEmpty(File.ReadAllBytes(tempFile.Path));

                //---------------Execute Test ----------------------
                sut.Set(sourceUrl, expected);

                //---------------Test Result -----------------------
                CollectionAssert.AreEqual(expected, File.ReadAllBytes(tempFile.Path));
            }
        }

        [Test]
        public void Set_GivenSourceAndData_WhenCacheFolderDoesNotExist_ShouldCreateIt()
        {
            //---------------Set up test pack-------------------
            var url = GetRandomHttpUrl();
            var expected = GetRandomBytes();
            var cacheFilenameGenerator = Substitute.For<ICacheFilenameGenerator>();
            using (var tempFolder = new AutoTempFolder())
            {
                var nestedFolder = string.Join(Path.DirectorySeparatorChar.ToString(), GetRandomCollection<string>(2,3));
                var cacheFileName = GetRandomFileName();
                var cacheFilePath = Path.Combine(tempFolder.Path, nestedFolder, cacheFileName);
                cacheFilenameGenerator.GenerateFor(url)
                        .Returns(cacheFilePath);
                var sut = Create(cacheFilenameGenerator);

                //---------------Assert Precondition----------------
                Assert.IsFalse(Directory.Exists(Path.GetDirectoryName(cacheFilePath)));

                //---------------Execute Test ----------------------
                sut.Set(url, expected);

                //---------------Test Result -----------------------
                CollectionAssert.AreEqual(expected, File.ReadAllBytes(cacheFilePath));
            }
        }


        [Test]
        public void Get_GivenSource_WhenNoExistingData_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var sourceUrl = GetRandomHttpUrl();
            var cacheFilenameGenerator = Substitute.For<ICacheFilenameGenerator>();
            using (var tempFile = new AutoTempFile())
            {
                File.Delete(tempFile.Path);
                cacheFilenameGenerator.GenerateFor(sourceUrl).Returns(tempFile.Path);
                var sut = Create(cacheFilenameGenerator);
                //---------------Assert Precondition----------------
                Assert.IsFalse(File.Exists(tempFile.Path));

                //---------------Execute Test ----------------------
                var result = sut.GetReaderFor(sourceUrl);

                //---------------Test Result -----------------------
                Assert.IsNull(result);
            }
        }

        [Test]
        public void Get_GivenSource_WhenCacheFileExists_ShouldReturnContents()
        {
            //---------------Set up test pack-------------------
            var sourceUrl = GetRandomHttpUrl();
            var cacheFilenameGenerator = Substitute.For<ICacheFilenameGenerator>();
            using (var tempFile = new AutoTempFile())
            {
                cacheFilenameGenerator.GenerateFor(sourceUrl).Returns(tempFile.Path);
                var expected = GetRandomCollection<string>(2,5);
                File.WriteAllBytes(tempFile.Path, expected.JoinWith(Environment.NewLine).AsBytes());
                var sut = Create(cacheFilenameGenerator, new TextFileReaderFactory());

                //---------------Assert Precondition----------------
                CollectionAssert.AreEqual(expected, tempFile.StringData.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

                //---------------Execute Test ----------------------
                var result = sut.GetReaderFor(sourceUrl);

                //---------------Test Result -----------------------
                Assert.IsNotNull(result);
                var resultLines = result.EnumerateLines().ToArray();
                CollectionAssert.AreEqual(expected, resultLines);
            }
        }

        [Test]
        [Ignore("WIP: requires an IFileInfoRetriever")]
        public void CacheFileIsValid()
        {
            //--------------- Arrange -------------------
            //--------------- Assume ----------------

            //--------------- Act ----------------------

            //--------------- Assert -----------------------
            Assert.Fail("Test not yet implemented");
        }





        private IBlocklistCacheManager Create(
            ICacheFilenameGenerator cacheFilenameGenerator,
            ITextFileReaderFactory textFileReaderFactory = null,
            ISettings settings = null)
        {
            return new BlocklistCacheManager(
                            cacheFilenameGenerator, 
                            textFileReaderFactory ?? Substitute.For<ITextFileReaderFactory>(),
                            settings ?? CreateSettings());
        }

        private static ISettings CreateSettings()
        {
            var settings = Substitute.For<ISettings>();
            settings.RefreshIntervalInMinutes.Returns(1440);
            return settings;
        }
    }
}
