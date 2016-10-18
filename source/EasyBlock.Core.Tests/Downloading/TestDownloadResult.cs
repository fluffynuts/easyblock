using EasyBlock.Core.Implementations.Downloading;
using EasyBlock.Core.Interfaces.Downloading;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestDownloadResult
    {
        [Test]
        public void Type_ShouldImplement_IDownloadResult()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(DownloadResult);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<IDownloadResult>();

            //---------------Test Result -----------------------
        }

        [Test]
        public void Construct_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => new DownloadResult());

            //---------------Test Result -----------------------
        }


        [Test]
        public void Url_ShouldBeReadWrite()
        {
            //---------------Set up test pack-------------------
            var sut = GetRandom<DownloadResult>();


            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.IsNotNull(sut.Url);
            Assert.IsNotNull(sut.Data);
            Assert.IsNotNull(sut.FailureException);

            //---------------Test Result -----------------------
        }

    }
}
