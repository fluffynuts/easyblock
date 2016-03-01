using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PeanutButter.SimpleHTTPServer;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestFileDownloader
    {
        [Test]
        public async Task Download_GivenUrl_WhenCanDownload_ShouldReturnData()
        {
            //---------------Set up test pack-------------------
            using (var server = new HttpServer())
            using (var tempFile = new AutoTempFile())
            {
                var relativeUrl = GetRandomString();
                var expected = GetRandomBytes();
                server.AddFileHandler((processor, stream) =>
                {
                    if (processor.Path != "/" + relativeUrl)
                        throw new Exception($"Unexpected request {processor.Path}");
                    return expected;
                });
                var sut = Create();

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = await sut.DownloadDataAsync(server.GetFullUrlFor(relativeUrl));

                //---------------Test Result -----------------------
                Assert.IsTrue(result.Success);
                CollectionAssert.AreEqual(expected, result.Data);
            }
        }

        [Test]
        public async Task DownloadFile_GivenUrl_WhenCannotDownload_ShouldSetResultDataNull()
        {
            //---------------Set up test pack-------------------
            using (var server = new HttpServer())
            using (var tempFile = new AutoTempFile())
            {
                var sut = Create();
                var serverPath = GetRandomString();

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = await sut.DownloadDataAsync(server.GetFullUrlFor(serverPath));

                //---------------Test Result -----------------------
                Assert.IsFalse(result.Success);
                Assert.IsNull(result.Data);
            }
        }


        private IFileDownloader Create()
        {
            return new FileDownloader();
        }
    }
}
