using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestTextFileReader
    {
        [Test]
        public void Type_ShouldImplement_ITextFileReader()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(TextFileReader);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<ITextFileReader>();

            //---------------Test Result -----------------------
        }


        [Test]
        public void IDisposableReader_ShouldImplement_IDisposable()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(ITextFileReader);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<IDisposable>();

            //---------------Test Result -----------------------

        }

        [Test]
        public void Construct_GivenUnknownFile_ShouldThrow()
        {
            //---------------Set up test pack-------------------
            var fileName = Path.GetTempFileName();
            if (File.Exists(fileName))
                File.Delete(fileName);
            //---------------Assert Precondition----------------
            Assert.IsFalse(File.Exists(fileName));

            //---------------Execute Test ----------------------
            Assert.Throws<FileNotFoundException>(() => Create(fileName));
            
            //---------------Test Result -----------------------
        }

        [Test]
        public void Discovery_WhatExceptionDoesStreamReaderThrowWhenTheFileDoesntExist()
        {
            //---------------Set up test pack-------------------
            var tempFile = Path.GetTempFileName();
            if (File.Exists(tempFile))
                File.Delete(tempFile);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<FileNotFoundException>(() => new StreamReader(tempFile));

            //---------------Test Result -----------------------

        }

        [Test]
        public void ReadLine_ShouldReadOneLineFromTheFile()
        {
            //---------------Set up test pack-------------------
            using (var tempFile = new AutoTempFile())
            {
                var lines = RandomValueGen.GetRandomCollection<string>(3,3);
                var linesAsBytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));
                File.WriteAllBytes(tempFile.Path, linesAsBytes);
                var sut = Create(tempFile.Path);
                //---------------Assert Precondition----------------
                // pedantic? sure!
                CollectionAssert.AreEqual(linesAsBytes, File.ReadAllBytes(tempFile.Path));

                //---------------Execute Test ----------------------
                var result1 = sut.ReadLine();
                var result2 = sut.ReadLine();
                var result3 = sut.ReadLine();

                //---------------Test Result -----------------------
                Assert.AreEqual(result1, lines.First());
                Assert.AreEqual(result2, lines.Skip(1).First());
                Assert.AreEqual(result3, lines.Skip(2).First());

            }
        }

        [Test]
        public void ReadLine_WhenOutOfLines_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var lines = new[] {"first line"};
            using (var tempFile = new AutoTempFile())
            {
                File.WriteAllBytes(tempFile.Path, Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines)));
                var sut = Create(tempFile.Path);
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var first = sut.ReadLine();
                Assert.IsNotNull(first);
                Assert.AreEqual(lines.First(), first);
                var second = sut.ReadLine();
                Assert.IsNull(second);

                //---------------Test Result -----------------------
            }
        }

        private ITextFileReader Create(string fileName)
        {
            return new TextFileReader(fileName);
        }
    }
}
