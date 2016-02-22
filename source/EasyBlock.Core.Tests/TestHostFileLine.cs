using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;

namespace EasyBlock.Core.Tests
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

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            HostFileLine sut = null;
            Assert.DoesNotThrow(() => sut = new HostFileLine(input));

            //---------------Test Result -----------------------
            Assert.IsFalse(sut.IsComment);
            Assert.AreEqual(string.Empty, sut.Data);
            Assert.AreEqual(string.Empty, sut.HostName);
            Assert.AreEqual(string.Empty, sut.IPAddress);
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
            var line = RandomValueGen.GetRandomString();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(line);

            //---------------Test Result -----------------------
            Assert.IsFalse(sut.IsComment);
            Assert.AreEqual(string.Empty, sut.HostName);
            Assert.AreEqual(string.Empty, sut.IPAddress);

        }


        private IHostFileLine Create(string data)
        {
            return new HostFileLine(data);
        }
    }
}
