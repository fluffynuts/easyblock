using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestHostFIle
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

        [Test]
        [Ignore("TODO")]
        public void Merge_WhenReaderHasNoLines_ShouldNotChangeLines()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------

        }

        [Test]
        [Ignore("TODO")]
        public void Merge_WhenReaderHasOneLineWithExistingHost_ShouldChangeLines()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------

        }

        [Test]
        [Ignore("TODO")]
        public void Merge_WhenReaderHasOneLineWithNewHost_ShouldAddMergeLine()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------

        }

        [Test]
        [Ignore("TODO")]
        public void Merge_WhenReaderHasTwoLines_AndOneIsNew_AndOneIsKnown_ShouldAddNewLineToMergeLines()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------

        }

        [Test]
        [Ignore("TODO")]
        public void Persist_WhenHaveNoMergeLines_ShouldOutputToSpecifiedFile()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------

        }

        [Test]
        [Ignore("TODO")]
        public void Persist_WhenHaveOneMergeLine_ShouldOutputToSpecifiedFile_WithCommentAndThenMergedLine()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------

        }

        private IHostFile Create(ITextFileReader reader = null,
                                    ITextFileWriter writer = null)
        {
            return new HostFile(reader ?? Substitute.For<ITextFileReader>(), 
                                writer ?? Substitute.For<ITextFileWriter>());
        }
    }

    public static class TestExtensionsForTextFileReader
    {
        public static void SetData(this ITextFileReader reader, IEnumerable<string> lines)
        {
            var queue = new Queue<string>(lines);
            reader.ReadLine().Returns(ci =>
            {
                if (!queue.Any())
                    return null;
                return queue.Dequeue();
            });
        }

    }
}
