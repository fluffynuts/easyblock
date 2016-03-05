using System;
using System.IO;
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

        [Test]
        public void Construct_GivenNullCacheFilenameGenerator_ShouldThrowANE()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.Throws<ArgumentNullException>(() => new BlocklistCacheManager((ICacheFilenameGenerator)null));

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
                var result = sut.Get(sourceUrl);

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
                var expected = GetRandomBytes();
                File.WriteAllBytes(tempFile.Path, expected);
                var sut = Create(cacheFilenameGenerator);

                //---------------Assert Precondition----------------
                CollectionAssert.AreEqual(expected, tempFile.BinaryData);

                //---------------Execute Test ----------------------
                var result = sut.Get(sourceUrl);

                //---------------Test Result -----------------------
                CollectionAssert.AreEqual(expected, result);
            }
        }



        private IBlocklistCacheManager Create(ICacheFilenameGenerator cacheFilenameGenerator)
        {
            return new BlocklistCacheManager(cacheFilenameGenerator);
        }
    }
}
