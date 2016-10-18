using System;
using System.IO;
using EasyBlock.Core.Implementations.IO.TextWriter;
using EasyBlock.Core.Interfaces.IO.TextWriter;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestTextFileWriterFactory
    {
        [Test]
        public void Construct_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(() => new TextFileWriterFactory());

            //---------------Test Result -----------------------
        }

        [Test]
        public void Type_ShouldImplement_ITextFileReaderFactory()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(TextFileWriterFactory);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<ITextFileWriterFactory>();

            //---------------Test Result -----------------------
        }


        [Test]
        public void Open_ShouldReturnReaderForProvidedFilePath()
        {
            //---------------Set up test pack-------------------
            using (var tempFile = new AutoTempFile())
            {
                var lines = RandomValueGen.GetRandomCollection<string>(3,5);
                File.WriteAllBytes(tempFile.Path, lines.JoinWith("\n").AsBytes());
                var sut = Create();

                //---------------Assert Precondition----------------
                CollectionAssert.AreEqual(lines.JoinWith("\n").AsBytes(), File.ReadAllBytes(tempFile.Path));

                //---------------Execute Test ----------------------
                var newLines = RandomValueGen.GetRandomCollection<string>(4, 6);
                var writer = sut.Open(tempFile.Path);
                newLines.ForEach(writer.AppendLine);
                writer.Persist();

                //---------------Test Result -----------------------
                var linesOnDisk = File.ReadAllBytes(tempFile.Path).ToUTF8String().Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                CollectionAssert.AreEqual(linesOnDisk, newLines);
            }
        }

        private ITextFileWriterFactory Create()
        {
            return new TextFileWriterFactory();
        }
    }
}