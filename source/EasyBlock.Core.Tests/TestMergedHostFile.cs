using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestMergedHostFile
    {
        [Test]
        public void Construct_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => new MergedHostFile());

            //---------------Test Result -----------------------
        }

        [Test]
        public void Type_ShouldImplement_IMergedHostFile()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(MergedHostFile);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<IMergedHostFile>();

            //---------------Test Result -----------------------
        }

        [Test]
        public void LoadFrom_GivenReaderWithNoData_ShouldLeaveStaticLinesEmpty()
        {
            //---------------Set up test pack-------------------
            var reader = Substitute.For<ITextFileReader>();
            reader.ReadLine().Returns((string)null);
            var sut = Create();

            //---------------Assert Precondition----------------
            CollectionAssert.IsEmpty(sut.StaticLines);

            //---------------Execute Test ----------------------
            sut.LoadFrom(reader);

            //---------------Test Result -----------------------
            CollectionAssert.IsEmpty(sut.StaticLines);
        }

        [Test]
        public void LoadFrom_GivenReaderWithNoData_ShouldLeaveMergedLinesEmpty()
        {
            //---------------Set up test pack-------------------
            var reader = Substitute.For<ITextFileReader>();
            reader.ReadLine().Returns((string)null);
            var sut = Create();

            //---------------Assert Precondition----------------
            CollectionAssert.IsEmpty(sut.MergedLines);

            //---------------Execute Test ----------------------
            sut.LoadFrom(reader);

            //---------------Test Result -----------------------
            CollectionAssert.IsEmpty(sut.MergedLines);
        }

        [Test]
        public void LoadFrom_GivenReaderWithOneStaticLine_ShouldPutIntoStaticLines()
        {
            //---------------Set up test pack-------------------
            var reader = Substitute.For<ITextFileReader>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }


        private IMergedHostFile Create()
        {
            return new MergedHostFile();
        }


    }
}
