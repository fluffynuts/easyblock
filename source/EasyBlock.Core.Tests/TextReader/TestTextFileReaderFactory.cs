using System.IO;
using System.Linq;
using EasyBlock.Core.Extensions;
using EasyBlock.Core.Implementations.TextReader;
using EasyBlock.Core.Interfaces.TextReader;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests.TextReader
{
    [TestFixture]
    public class TestTextFileReaderFactory
    {
        [Test]
        public void Construct_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => new TextFileReaderFactory());

            //---------------Test Result -----------------------
        }

        [Test]
        public void Type_ShouldImplement_ITextFileReaderFactory()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(TextFileReaderFactory);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<ITextFileReaderFactory>();

            //---------------Test Result -----------------------
        }


        [Test]
        public void Open_ShouldReturnReaderForProvidedFilePath()
        {
            //---------------Set up test pack-------------------
            using (var tempFile = new AutoTempFile())
            {
                var lines = GetRandomCollection<string>(3,5);
                File.WriteAllBytes(tempFile.Path, lines.JoinWith("\n").AsBytes());
                var sut = Create();

                //---------------Assert Precondition----------------
                CollectionAssert.AreEqual(lines.JoinWith("\n").AsBytes(), File.ReadAllBytes(tempFile.Path));

                //---------------Execute Test ----------------------
                var reader = sut.Open(tempFile.Path);

                //---------------Test Result -----------------------
                var linesOnDisk = reader.EnumerateLines().ToArray();
                CollectionAssert.AreEqual(linesOnDisk, lines);
            }
        }

        private ITextFileReaderFactory Create()
        {
            return new TextFileReaderFactory();
        }
    }
}
