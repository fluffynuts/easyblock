using System.IO;
using EasyBlock.Core.Implementations.Caching;
using EasyBlock.Core.Interfaces.Caching;
using NUnit.Framework;

namespace EasyBlock.Core.Tests.Caching
{
    [TestFixture]
    public class TestFileInfoTRetriever: AssertionHelper
    {
        [Test]
        public void GetFileInfoFor_GivenUnknownPath_ShouldReturnNull()
        {
            //--------------- Arrange -------------------
            var path = Path.GetTempFileName();
            var sut = Create();
            File.Delete(path);

            //--------------- Assume ----------------
            Expect(File.Exists(path), Is.False);

            //--------------- Act ----------------------

            var result = sut.GetFileInfoFor(path);

            //--------------- Assert -----------------------
            Expect(result, Is.Null);
        }

        private IFileInfoRetriever Create()
        {
            return new FileInfoRetriever();
        }
    }
}
