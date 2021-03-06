﻿using EasyBlock.Core.Implementations.HostFiles;
using EasyBlock.Core.Interfaces.HostFiles;
using EasyBlock.Core.Interfaces.TextReader;
using EasyBlock.Core.Interfaces.TextWriter;
using EasyBlock.Core.Tests.Extensions;
using NSubstitute;
using NUnit.Framework;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestHostFileFactory
    {
        [Test]
        public void Create_ShouldCreateHostFileWithProvided_ReaderAndWriter()
        {
            //---------------Set up test pack-------------------
            var reader = Substitute.For<ITextFileReader>();
            var commentLine = "# this is a comment";
            reader.SetData(commentLine);
            var writer = Substitute.For<ITextFileWriter>();
            var sut = Create();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var hostFile = sut.Create(reader, writer);
            hostFile.Persist();

            //---------------Test Result -----------------------
            reader.Received(2).ReadLine();
            writer.Received(1).AppendLine(commentLine);
            writer.Received(1).Persist();
        }

        private IHostFileFactory Create()
        {
            return new HostFileFactory();
        }
    }
}
