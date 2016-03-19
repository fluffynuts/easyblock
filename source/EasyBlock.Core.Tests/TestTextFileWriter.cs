using System;
using System.IO;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;
// ReSharper disable PossibleMultipleEnumeration

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestTextFileWriter
    {
        [Test]
        public void Type_ShouldImplement_ITextFileWriter()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(TextFileWriter);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<ITextFileWriter>();

            //---------------Test Result -----------------------
        }

        [Test]
        public void AppendLine_ShouldNotYetChangeTheFile()
        {
            //---------------Set up test pack-------------------
            using (var tempFile = new AutoTempFile())
            {
                var sut = Create(tempFile.Path);

                //---------------Assert Precondition----------------
                Assert.IsTrue(File.Exists(tempFile.Path));
                CollectionAssert.IsEmpty(File.ReadAllBytes(tempFile.Path));

                //---------------Execute Test ----------------------
                sut.AppendLine(GetRandomString());

                //---------------Test Result -----------------------
                CollectionAssert.IsEmpty(File.ReadAllBytes(tempFile.Path));
            }
        }

        [Test]
        public void Persist_ShouldFlushLinesToFile()
        {
            //---------------Set up test pack-------------------
            using (var tempFile = new AutoTempFile())
            {
                var lines = GetRandomCollection<string>(3,3);
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var writer = Create(tempFile.Path);
                lines.ForEach(writer.AppendLine);
                writer.Persist();

                //---------------Test Result -----------------------
                var inFile = File.ReadAllText(tempFile.Path).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                CollectionAssert.AreEqual(lines, inFile);
            }
        }

        [Test]
        public void AppendLine_then_Persist_WhenDestinationFolderDoesntExist_ShouldCreateIt()
        {
            //---------------Set up test pack-------------------
            var baseFolder = Path.Combine(Path.GetTempPath(), GetRandomAlphaNumericString(3));
            var lines = GetRandomCollection<string>(3);
            using (var tempFile = new AutoTempFile(baseFolder, string.Empty))
            {
                File.Delete(tempFile.Path);
                Directory.Delete(baseFolder, true);
                //---------------Assert Precondition----------------
                Assert.IsFalse(Directory.Exists(baseFolder));

                //---------------Execute Test ----------------------
                var sut = Create(tempFile.Path);
                Assert.IsFalse(File.Exists(tempFile.Path));
                lines.ForEach(sut.AppendLine);
                sut.Persist();

                //---------------Test Result -----------------------
                Assert.IsTrue(File.Exists(tempFile.Path));
                var inFile = tempFile.StringData.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                CollectionAssert.AreEqual(lines, inFile);
            }
        }

        [Test]
        public void FileMove_ShouldThrowWhenTargetFileExists()
        {
            // Discovery -- and this is why I don't just use a rename
            //   to get a file atomically moved
            //---------------Set up test pack-------------------
            using (var tempFile1 = new AutoTempFile())
            using (var tempFile2 = new AutoTempFile())
            {
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                Assert.Throws<IOException>(() => File.Move(tempFile2.Path, tempFile1.Path));

                //---------------Test Result -----------------------
            }
        }

        private ITextFileWriter Create(string path)
        {
            return new TextFileWriter(path);
        }
    }
}