using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestCacheFilenameGenerator
    {
        [Test]
        public void Construct_GivenNullAppSettings_ShouldThrowANE()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.Throws<ArgumentNullException>(() => new CacheFilenameGenerator((ISettings)null));

            //---------------Test Result -----------------------
        }

        [Test]
        public void Type_ShouldImplement_ICacheFilenameGenerator()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(CacheFilenameGenerator);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<ICacheFilenameGenerator>();

            //---------------Test Result -----------------------
        }


        [Test]
        public void GenerateFor_ShouldReturnPathUnderConfiguredCachePath()
        {
            //---------------Set up test pack-------------------
            var appSettings = Substitute.For<ISettings>();
            var cacheFolder = GetRandomWindowsPath();
            appSettings.CacheFolder.Returns(cacheFolder);
            var sourceUrl = GetRandomHttpUrl() + "/somepath?arg1=value1&arg2=value2";
            var expectedFileName = sourceUrl.AsBytes().ToMD5String();
            var expected = Path.Combine(cacheFolder, expectedFileName);
            var sut = Create(appSettings);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = sut.GenerateFor(sourceUrl);

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }

        private ICacheFilenameGenerator Create(ISettings settings)
        {
            return new CacheFilenameGenerator(settings);
        }
    }
}