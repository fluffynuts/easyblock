using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestHostFile
    {
        [TestCase("reader", typeof(ITextFileReader))]
        [TestCase("writer", typeof(ITextFileWriter))]
        public void Construct_ShouldExpectParameter_(string parameterName, Type parameterType)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ConstructorTestUtils.ShouldExpectNonNullParameterFor<HostFile>(parameterName, parameterType);

            //---------------Test Result -----------------------
        }

        [Test]
        public void Construct_ShouldLoadLinesFromReader()
        {
            //---------------Set up test pack-------------------
            var reader = Substitute.For<ITextFileReader>();
            var lines = new[]
            {
                "# this is a comment",
                "127.0.0.1 localhost",
                "4.4.4.4    google.stuff.com"
            };
            reader.SetData(lines);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(reader);

            //---------------Test Result -----------------------
            var line = sut.Lines.First();
            Assert.IsTrue(line.IsComment);
            Assert.AreEqual(line.Data, lines[0]);
            line = sut.Lines.Skip(1).First();
            Assert.IsFalse(line.IsComment);
            Assert.AreEqual("127.0.0.1", line.IPAddress);
            Assert.AreEqual("localhost", line.HostName);
            line = sut.Lines.Last();
            Assert.AreEqual("4.4.4.4", line.IPAddress);
            Assert.AreEqual("google.stuff.com", line.HostName);

        }

        private IHostFile Create(ITextFileReader reader = null,
                                    ITextFileWriter writer = null)
        {
            return new HostFile(reader ?? Substitute.For<ITextFileReader>(), 
                                writer ?? Substitute.For<ITextFileWriter>());
        }
    }
}
