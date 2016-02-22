using NSubstitute;
using NUnit.Framework;
using PeanutButter.RandomGenerators;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestTextFileReaderSubstituteExtensions
    {
        [Test]
        public void ReadLine_WhenOutOfData_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var line1 = RandomValueGen.GetRandomString();
            var line2 = RandomValueGen.GetRandomString();
            var reader = Substitute.For<ITextFileReader>();
            reader.SetData(new[] { line1, line2 });

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var first = reader.ReadLine();
            var second = reader.ReadLine();
            var last = reader.ReadLine();
            var lastAgain = reader.ReadLine();

            //---------------Test Result -----------------------
            Assert.AreEqual(line1, first);
            Assert.AreEqual(line2, second);
            Assert.IsNull(last);
            Assert.IsNull(lastAgain);
        }

    }
}