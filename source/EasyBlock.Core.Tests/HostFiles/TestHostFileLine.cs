using EasyBlock.Core.Implementations.HostFiles;
using EasyBlock.Core.Interfaces.HostFiles;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests.HostFiles
{
    [TestFixture]
    public class TestHostFileLine
    {
        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("")]
        [TestCase("\r\n\t")]
        public void Construct_GivenBadLine_ShouldNotBarf(string input)
        {
            //---------------Set up test pack-------------------
            var isPrimary = GetRandomBoolean();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            HostFileLine sut = null;
            Assert.DoesNotThrow(() => sut = new HostFileLine(input, isPrimary));

            //---------------Test Result -----------------------
            Assert.IsFalse(sut.IsComment);
            Assert.AreEqual(string.Empty, sut.Data);
            Assert.AreEqual(string.Empty, sut.HostName);
            Assert.AreEqual(string.Empty, sut.IPAddress);
            Assert.AreEqual(isPrimary, sut.IsPrimary);
        }

        [Test]
        public void Type_ShouldImplement_IHostFileLine()
        {
            //---------------Set up test pack-------------------
            var sut = typeof (HostFileLine);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<IHostFileLine>();

            //---------------Test Result -----------------------

        }

        [Test]
        public void Constuct_GivenCommentLine_ShouldSetUpAsCommentLine()
        {
            //---------------Set up test pack-------------------
            var comment = "# this is a comment";

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(comment);

            //---------------Test Result -----------------------
            Assert.IsTrue(sut.IsComment);
            Assert.AreEqual(comment, sut.Data);
        }

        [Test]
        public void Construct_GivenHostFileLine_ShouldSetUpAsHostFileLine()
        {
            //---------------Set up test pack-------------------
            var line = "127.0.0.1   somehost.somedomain";

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(line);

            //---------------Test Result -----------------------
            Assert.IsFalse(sut.IsComment);
            Assert.AreEqual("127.0.0.1", sut.IPAddress);
            Assert.AreEqual("somehost.somedomain", sut.HostName);

        }

        [Test]
        public void Construct_GivenPartialLine_ShouldNotSetUpAsHostFileLine()
        {
            //---------------Set up test pack-------------------
            var line = GetRandomString();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(line);

            //---------------Test Result -----------------------
            Assert.IsFalse(sut.IsComment);
            Assert.AreEqual(string.Empty, sut.HostName);
            Assert.AreEqual(string.Empty, sut.IPAddress);

        }


        private IHostFileLine Create(string data, bool isPrimary = true)
        {
            return new HostFileLine(data, isPrimary);
        }
    }
}
