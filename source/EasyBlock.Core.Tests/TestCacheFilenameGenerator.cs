using System;
using NUnit.Framework;
using PeanutButter.INIFile;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestCacheFilenameGenerator
    {
        [Test]
        public void Construct_GivenNullINIFile_ShouldThrowANE()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.Throws<ArgumentNullException>(() => new CacheFilenameGenerator((IINIFile)null));

            //---------------Test Result -----------------------
        }

        [Test]
        [Ignore("WIP")]
        public void GenerateFor_ShouldReturnPathUnderConfiguredCachePath()
        {
            //---------------Set up test pack-------------------
            //var expected = RandomValueGen.GetRandomFileName
            //var iniFile = IniBuilder.Create()
            //                    .WithCacheLocation(
            //var sut = Create(

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }


    }
}