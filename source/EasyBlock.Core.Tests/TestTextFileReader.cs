using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

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
                Assert.AreEqual(lines.First(), result1);
                Assert.AreEqual(lines.Skip(1).First(), result2);
                Assert.AreEqual(lines.Skip(2).First(), result3);

            }
        }

        [Test]
        public void ReadLine_WhenOutOfLines_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            var lines = new[] {"first line"};
            using (var tempFile = new AutoTempFile())
            {
                File.WriteAllLines(tempFile.Path, lines);
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

        [Test]
        public void Dispose_ShouldDisposeUnderlyingReader_AndSubsequentCallsToReadLine_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            using (var tempFile = new AutoTempFile())
            {
                File.WriteAllLines(tempFile.Path, RandomValueGen.GetRandomCollection<string>(2,2).ToArray());
                var sut = Create(tempFile.Path);
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                sut.Dispose();
                var result = sut.ReadLine();

                //---------------Test Result -----------------------
                Assert.IsNull(result);
            }
        }

        [Test]
        public void Readers_ShouldNotBlockEachOther()
        {
            //---------------Set up test pack-------------------
            var data = GetRandomString(10, 20);
            using (var tempFile = new AutoTempFile(data.AsBytes()))
            {
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var reader1 = Create(tempFile.Path);
                var reader2 = Create(tempFile.Path);

                //---------------Test Result -----------------------
                Assert.AreEqual(data, reader1.ReadLine());
                Assert.AreEqual(data, reader2.ReadLine());
            }
        }



        private ITextFileReader Create(string fileName)
        {
            return new TextFileReader(fileName);
        }
    }
}
