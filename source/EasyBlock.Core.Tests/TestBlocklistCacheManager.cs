using System;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;

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
        [Ignore("WIP: requires cache filename generator")]
        public void Cache_GivenSourceAndData_ShouldWriteDataToCacheLocation()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }
    }
}
