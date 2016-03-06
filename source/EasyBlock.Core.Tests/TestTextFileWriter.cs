using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PeanutButter.RandomGenerators;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;

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
        public void ITextFileWriter_ShouldImplement_IDisposable()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(ITextFileWriter);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<IDisposable>();

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
                sut.AppendLine(RandomValueGen.GetRandomString());

                //---------------Test Result -----------------------
                CollectionAssert.IsEmpty(File.ReadAllBytes(tempFile.Path));
            }
        }

        [Test]
        public void Dispose_ShouldFlushLinesToFile()
        {
            //---------------Set up test pack-------------------
            using (var tempFile = new AutoTempFile())
            {
                var lines = RandomValueGen.GetRandomCollection<string>(3,3);
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                using (var writer = Create(tempFile.Path))
                {
                    lines.ForEach(writer.AppendLine);
                }

                //---------------Test Result -----------------------
                var inFile = File.ReadAllText(tempFile.Path).Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                CollectionAssert.AreEqual(lines, inFile);
            }
        }

        [Test]
        public void AppendLine_then_Dispose_WhenDestinationFolderDoesntExist_ShouldCreateIt()
        {
            //---------------Set up test pack-------------------
            var baseFolder = Path.Combine(Path.GetTempPath(), RandomValueGen.GetRandomAlphaNumericString(3));
            var lines = RandomValueGen.GetRandomCollection<string>(3);
            using (var tempFile = new AutoTempFile(baseFolder, string.Empty))
            {
                File.Delete(tempFile.Path);
                Directory.Delete(baseFolder, true);
                //---------------Assert Precondition----------------
                Assert.IsFalse(Directory.Exists(baseFolder));

                //---------------Execute Test ----------------------
                using (var sut = Create(tempFile.Path))
                {
                    Assert.IsFalse(File.Exists(tempFile.Path));
                    lines.ForEach(sut.AppendLine);
                }
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