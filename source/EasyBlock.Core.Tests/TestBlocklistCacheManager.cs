﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using static PeanutButter.RandomGenerators.RandomValueGen;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;

namespace EasyBlock.Core.Tests
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



        private IBlocklistCacheManager Create(ICacheFilenameGenerator cacheFilenameGenerator,
                                                ITextFileReaderFactory textFileReaderFactory = null)
        {
            return new BlocklistCacheManager(
                            cacheFilenameGenerator, 
                            textFileReaderFactory ?? Substitute.For<ITextFileReaderFactory>());
        }
    }
}
