using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.SimpleHTTPServer;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestFileDownloader
    {
        [Test]
        public async Task Download_GivenUrlAndOutputFile_WhenCanDownload_ShouldPutFileInRequiredOutput()
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
                var result = await sut.DownloadFileAsync(server.GetFullUrlFor(relativeUrl), tempFile.Path);

                //---------------Test Result -----------------------
                Assert.IsTrue(result.Success);
                var writtenBytes = File.ReadAllBytes(tempFile.Path);
                Assert.AreEqual(writtenBytes.Length, result.DownloadSize);
                CollectionAssert.AreEqual(expected, writtenBytes);
            }
        }

        [Test]
        public async Task DownloadFile_GivenUrlAndOutputPath_WhenCannotDownload_ShouldNotOverwriteOutputPath()
        {
            //---------------Set up test pack-------------------
            using (var server = new HttpServer())
            using (var tempFile = new AutoTempFile())
            {
                var expected = GetRandomBytes();
                File.WriteAllBytes(tempFile.Path, expected);
                var sut = Create();
                var serverPath = GetRandomString();

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = await sut.DownloadFileAsync(server.GetFullUrlFor(serverPath), tempFile.Path);

                //---------------Test Result -----------------------
                Assert.IsFalse(result.Success);
                Assert.AreEqual(0, result.DownloadSize);
                var bytesOnDisk = File.ReadAllBytes(tempFile.Path);
                CollectionAssert.AreEqual(expected, bytesOnDisk);
            }
        }


        private IFileDownloader Create()
        {
            return new FileDownloader();
        }
    }
}
