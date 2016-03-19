using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestHostFIle
    {
        [TestCase("reader", typeof(ITextFileReader))]
        [TestCase("writer", typeof(ITextFileWriter))]
        public void Construct_ShouldExpectParameter_(string parameterName, Type parameterType)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ConstructorTestUtils.ShouldExpectNonNullParameterFor<HostFile>(parameterName, parameterType);

            //---------------Test Result -----------------------
        }

        [Test]
        public void Construct_WhenHaveNoPriorMergedLines_ShouldLoadLinesFromReader()
        {
            //---------------Set up test pack-------------------
            var reader = Substitute.For<ITextFileReader>();
            reader.SetData(_startData);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(reader);

            //---------------Test Result -----------------------
            var line = sut.Lines.First();
            Assert.IsTrue(line.IsComment);
            Assert.AreEqual(line.Data, _startData[0]);
            line = sut.Lines.Skip(1).First();
            Assert.IsFalse(line.IsComment);
            Assert.AreEqual("127.0.0.1", line.IPAddress);
            Assert.AreEqual("localhost", line.HostName);
            line = sut.Lines.Last();
            Assert.AreEqual("4.4.4.4", line.IPAddress);
            Assert.AreEqual("google.stuff.com", line.HostName);
            Assert.IsTrue(sut.Lines.All(l => l.IsPrimary));
            reader.Received().Dispose();

        }

        [Test]
        public void Construct_WhenNoPriorMergedLines_ShouldLoadLinesFromReaderAsWellAsMergedLines()
        {
            //---------------Set up test pack-------------------
            var reader = Substitute.For<ITextFileReader>();
            var mergedIp = GetRandomIPv4Address();
            var mergedHost = GetRandomHostname();
            reader.SetData(_startData.And(Constants.MERGE_MARKER).And($"{mergedIp}\t{mergedHost}"));

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(reader);

            //---------------Test Result -----------------------
            var line = sut.Lines.First();
            Assert.IsTrue(line.IsComment);
            Assert.AreEqual(line.Data, _startData[0]);
            line = sut.Lines.Skip(1).First();
            Assert.IsFalse(line.IsComment);
            Assert.AreEqual("127.0.0.1", line.IPAddress);
            Assert.AreEqual("localhost", line.HostName);
            line = sut.Lines.Skip(2).First();
            Assert.AreEqual("4.4.4.4", line.IPAddress);
            Assert.AreEqual("google.stuff.com", line.HostName);
            line = sut.Lines.Skip(3).First();
            Assert.AreEqual(mergedIp, line.IPAddress);
            Assert.AreEqual(mergedHost, line.HostName);
            Assert.IsFalse(line.IsPrimary);

        }

        private readonly string[] _startData =
        {
            "# this is a comment",
            "127.0.0.1 localhost",
            "4.4.4.4    google.stuff.com"
        };

        [Test]
        public void Merge_WhenReaderHasNoLines_ShouldNotChangeLines()
        {
            //---------------Set up test pack-------------------
            var initialReader = Substitute.For<ITextFileReader>();
            initialReader.SetData(_startData);
            var nextReader = Substitute.For<ITextFileReader>();
            nextReader.SetData();
            var sut = Create(initialReader);

            //---------------Assert Precondition----------------
            var initialLines = sut.Lines.Clone();

            //---------------Execute Test ----------------------
            sut.Merge(nextReader);

            //---------------Test Result -----------------------
            CollectionAssert.AreEqual(initialLines, sut.Lines, new HostFileLineComparer());

        }

        [Test]
        public void Merge_WhenReaderHasOneLineWithNewHost_ShouldAddMergeLine()
        {
            //---------------Set up test pack-------------------
            var initial = CreateReaderFor(_startData);
            var ip = GetRandomIPv4Address();
            var host = GetRandomHostname();
            var mergeReader = CreateReaderFor($"{ip}  {host}");
            var sut = Create(initial);

            //---------------Assert Precondition----------------
            Assert.IsFalse(sut.Lines.Any(l => l.IPAddress == ip || l.HostName == host));

            //---------------Execute Test ----------------------
            sut.Merge(mergeReader);

            //---------------Test Result -----------------------
            Assert.IsNotNull(sut.Lines.Single(l => l.IPAddress == ip && 
                                                    l.HostName == host && 
                                                    !l.IsPrimary));

        }

        [Test]
        public void Merge_WhenReaderHasOneLineWithExistingMergeHost_ShouldChangeLines()
        {
            //---------------Set up test pack-------------------
            var ip = GetRandomIPv4Address();
            var host = GetRandomHostname();
            var initialReader = Substitute.For<ITextFileReader>();
            initialReader.SetData(_startData.And($"{ip} {host}"));
            var firstMergeReader = Substitute.For<ITextFileReader>();
            var anotherIp = GetAnother(ip, GetRandomIPv4Address);
            firstMergeReader.SetData($"{anotherIp}  {host}");
            var sut = Create(initialReader);

            //---------------Assert Precondition----------------
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.AreNotEqual(ip, anotherIp);
            Assert.DoesNotThrow(() => sut.Lines.Single(l => l.IsPrimary && 
                                                            l.HostName == host && 
                                                            l.IPAddress == ip));

            //---------------Execute Test ----------------------
            sut.Merge(firstMergeReader);

            //---------------Test Result -----------------------
            Assert.AreEqual(1, sut.Lines.Count(l => l.HostName == host));

        }

        [Test]
        public void Merge_WhenReaderHasTwoLines_AndOneIsNew_AndOneIsKnown_ShouldAddNewLineToMergeLines()
        {
            //---------------Set up test pack-------------------
            var host = GetRandomHostname();
            var ip1 = GetRandomIPv4Address();
            var initial = CreateReaderFor(_startData.And($"{ip1}    {host}"));
            var ip2 = GetAnother(ip1, GetRandomIPv4Address);
            var host2 = GetAnother(host, GetRandomHostname);
            var host2Ip = GetRandomIPv4Address();
            var mergeReader = CreateReaderFor($"{ip2}   {host}", $"{host2Ip}    {host2}");
            var sut = Create(initial);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Merge(mergeReader);

            //---------------Test Result -----------------------
            Assert.IsTrue(sut.Lines.Any(l => l.HostName == host2 && 
                                                l.IPAddress == host2Ip && 
                                                !l.IsPrimary));
            var unaltered = sut.Lines.Single(l => l.HostName == host);
            Assert.AreEqual(ip1, unaltered.IPAddress);
        }

        [Test]
        public void Persist_WhenHaveNoMergeLines_ShouldOutputWithWriter()
        {
            //---------------Set up test pack-------------------
            var reader = CreateReaderFor(_startData);
            var writer = Substitute.For<ITextFileWriter>();
            var sut = Create(reader, writer);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Persist();

            //---------------Test Result -----------------------
            writer.Received(3).AppendLine(Arg.Any<string>());
            Received.InOrder(() =>
            {
                writer.AppendLine(_startData[0]);
                writer.AppendLine(_startData[1]);
                writer.AppendLine(_startData[2]);
                writer.Persist();
            });

        }

        [Test]
        public void Persist_WhenHaveOneMergeLine_ShouldOutputToSpecifiedFile_WithCommentAndThenMergedLine()
        {
            //---------------Set up test pack-------------------
            var reader = CreateReaderFor(_startData);
            var writer = Substitute.For<ITextFileWriter>();
            var sut = Create(reader, writer);
            var host = GetRandomHostname();
            var ip = GetRandomIPv4Address();
            var mergeReader = CreateReaderFor($"{ip}    {host}");

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Merge(mergeReader);
            sut.Persist();

            //---------------Test Result -----------------------
            writer.Received(6).AppendLine(Arg.Any<string>());
            Received.InOrder(() =>
            {
                writer.AppendLine(_startData[0]);
                writer.AppendLine(_startData[1]);
                writer.AppendLine(_startData[2]);
                writer.AppendLine(string.Empty);
                writer.AppendLine(Constants.MERGE_MARKER);
                writer.AppendLine($"{ip}\t{host}");
                writer.Persist();
            });

        }

        [Test]
        public void Persist_WhenHaveTwoMergeLinesAndAComment_ShouldOutputToSpecifiedFile_WithCommentAndThenMergedLinesWithoutComments()
        {
            //---------------Set up test pack-------------------
            var reader = CreateReaderFor(_startData);
            var writer = Substitute.For<ITextFileWriter>();
            var sut = Create(reader, writer);
            var host = GetRandomHostname();
            var ip = GetRandomIPv4Address();
            var anotherHost = GetRandomHostname();
            var anotherIp = GetRandomIPv4Address();
            var mergeReader = CreateReaderFor($"{ip}    {host}", 
                                                "# list maintained by bob", 
                                                $"{anotherIp} {anotherHost}");

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Merge(mergeReader);
            sut.Persist();

            //---------------Test Result -----------------------
            writer.Received(7).AppendLine(Arg.Any<string>());
            Received.InOrder(() =>
            {
                writer.AppendLine(_startData[0]);
                writer.AppendLine(_startData[1]);
                writer.AppendLine(_startData[2]);
                writer.AppendLine(string.Empty);
                writer.AppendLine(Constants.MERGE_MARKER);
                writer.AppendLine($"{ip}\t{host}");
                writer.AppendLine($"{anotherIp}\t{anotherHost}");
                writer.Persist();
            });

        }

        [Test]
        public void SetRedirectIp_ShouldUpdateAllMergeLinesToUseRedirectIp()
        {
            //---------------Set up test pack-------------------
            var initialReader = CreateReaderFor("# this is the original file", "127.0.0.1 localhost");
            var originalDomain = GetRandomHostname();
            var originalIp = GetRandomIPv4Address();
            var redirectIp = GetAnother(originalIp, GetRandomIPv4Address);
            var mergeReader = CreateReaderFor($"{originalIp}  {originalDomain}");
            var sut = Create(initialReader);
            sut.Merge(mergeReader);

            //---------------Assert Precondition----------------
            Assert.IsTrue(sut.Lines.Any(l => l.HostName == originalDomain && 
                                                l.IPAddress == originalIp && 
                                                !l.IsPrimary));
            Assert.IsFalse(sut.Lines.Any(l => l.HostName == originalDomain && 
                                                l.IPAddress == redirectIp && 
                                                !l.IsPrimary));

            //---------------Execute Test ----------------------
            sut.SetRedirectIp(redirectIp);

            //---------------Test Result -----------------------
            Assert.IsTrue(sut.Lines.Any(l => l.HostName == originalDomain && 
                                                l.IPAddress == redirectIp && 
                                                !l.IsPrimary));
            Assert.IsFalse(sut.Lines.Any(l => l.HostName == originalDomain && 
                                                l.IPAddress == originalIp && 
                                                !l.IsPrimary));


        }


        [Test]
        public void Redirect_GivenDomainAndIpAddress_ShouldAddToLines()
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            var ip = GetRandomIPv4Address();
            var host = GetRandomHostname();

            //---------------Assert Precondition----------------
            CollectionAssert.IsEmpty(sut.Lines);

            //---------------Execute Test ----------------------
            sut.Redirect(host, ip);

            //---------------Test Result -----------------------
            Assert.IsTrue(sut.Lines.Any(l => !l.IsPrimary &&
                                                l.IPAddress == ip  &&
                                                l.HostName == host));
        }

        [Test]
        public void Whitelist_GivenUnknownDomain_ShouldChangeNothing()
        {
            //---------------Set up test pack-------------------
            var sut = Create(CreateReaderFor("127.0.0.1 localhost"));
            var whitelist = "a.b.*";

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Whitelist(whitelist);

            //---------------Test Result -----------------------
            var existing = sut.Lines.Single();
            Assert.IsTrue(existing.IsPrimary);
            Assert.AreEqual("127.0.0.1", existing.IPAddress);
            Assert.AreEqual("localhost", existing.HostName);

        }

        [Test]
        public void Whitelist_GivenDomainPrimarySection_ShouldChangeNothing()
        {
            //---------------Set up test pack-------------------
            var expectedIp = "192.168.1.1";
            var expectedHost = "a.b.c";
            var sut = Create(CreateReaderFor("127.0.0.1 localhost", $"{expectedIp}  {expectedHost}"));
            var whitelist = "a.b.*";

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Whitelist(whitelist);

            //---------------Test Result -----------------------
            Assert.AreEqual(2, sut.Lines.Count());
            Assert.IsTrue(sut.Lines.Any(l => l.IsPrimary && l.HostName == "localhost" && l.IPAddress == "127.0.0.1"));
            Assert.IsTrue(sut.Lines.Any(l => l.IsPrimary && l.HostName == expectedHost && l.IPAddress == expectedIp));
        }

        [Test]
        public void Whitelist_GivenDomainWhichMatchesPrimaryAndMergeDomain_ShouldOnlyRemoveMergeItem()
        {
            //---------------Set up test pack-------------------
            var expectedIp = "192.168.1.1";
            var expectedHost = "a.b.c";
            var sut = Create(CreateReaderFor("127.0.0.1 localhost", $"{expectedIp}  {expectedHost}"));
            sut.Redirect("a.b.d", "127.0.0.1");
            var whitelist = "a.b.*";
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.Whitelist(whitelist);

            //---------------Test Result -----------------------
            Assert.IsFalse(sut.Lines.Any(l => l.HostName == "a.b.d"));
        }

        [Test]
        public void Revert_ShouldRemoveAllMergeLines()
        {
            //---------------Set up test pack-------------------
            var expectedIp = GetRandomIPv4Address();
            var expectedHost = GetRandomHostname();
            var sut = Create(CreateReaderFor($"{expectedIp}  {expectedHost}"));
            var unexpectedIp = GetAnother(expectedIp, GetRandomIPv4Address);
            var unexpectedHost = GetAnother(expectedHost, GetRandomHostname);
            var mergeReader = CreateReaderFor($"{unexpectedIp}  {unexpectedHost}");
            sut.Merge(mergeReader);

            //---------------Assert Precondition----------------
            sut.Lines.ShouldHaveUnique(l => l.IsPrimary && l.IPAddress == expectedIp && l.HostName == expectedHost);
            sut.Lines.ShouldHaveUnique(l => !l.IsPrimary && l.IPAddress == unexpectedIp && l.HostName == unexpectedHost);

            //---------------Execute Test ----------------------
            sut.Revert();

            //---------------Test Result -----------------------
            sut.Lines.ShouldHaveUnique(l => l.IsPrimary && l.IPAddress == expectedIp && l.HostName == expectedHost);
            Assert.IsFalse(sut.Lines.Any(l => !l.IsPrimary));
        }

        [Test]
        public void Revert_WhenOriginalContainedMergeMarker_ShouldRemoveMergeMarker()
        {
            //---------------Set up test pack-------------------
            var expectedIp = GetRandomIPv4Address();
            var expectedHost = GetRandomHostname();
            var unexpectedIp = GetAnother(expectedIp, GetRandomIPv4Address);
            var unexpectedHost = GetAnother(expectedHost, GetRandomHostname);
            var writer = Substitute.For<ITextFileWriter>();
            var sut = Create(CreateReaderFor($"{expectedIp}  {expectedHost}",
                                              Constants.MERGE_MARKER,
                                              $"{unexpectedIp}  {unexpectedHost}"), writer);

            //---------------Assert Precondition----------------
            sut.Lines.ShouldHaveUnique(l => l.IsPrimary && l.IPAddress == expectedIp && l.HostName == expectedHost);
            sut.Lines.ShouldHaveUnique(l => !l.IsPrimary && l.IPAddress == unexpectedIp && l.HostName == unexpectedHost);

            //---------------Execute Test ----------------------
            sut.Revert();
            sut.Persist();

            //---------------Test Result -----------------------
            sut.Lines.ShouldHaveUnique(l => l.IsPrimary && l.IPAddress == expectedIp && l.HostName == expectedHost);
            Assert.IsFalse(sut.Lines.Any(l => !l.IsPrimary));
            Assert.IsFalse(sut.Lines.Any(l => l.Data == Constants.MERGE_MARKER));

            writer.Received(1).AppendLine(Arg.Is<string>(s => s.Contains(expectedIp) && s.Contains(expectedHost)));
            writer.DidNotReceive().AppendLine(Arg.Is<string>(s => s.Contains(unexpectedIp) && s.Contains(unexpectedHost)));
            writer.DidNotReceive().AppendLine(Arg.Is<string>(s => s == Constants.MERGE_MARKER));
        }

        [Test]
        public void Type_ShouldImplement_IDisposable()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(IHostFile);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<IDisposable>();

            //---------------Test Result -----------------------
        }


        [Test]
        [Ignore("WIP: Readers and writers need to be IDisposable")]
        public void Dispose_ShouldDisposeReadersAndWriters()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }




        private IHostFile Create(ITextFileReader reader = null,
                                    ITextFileWriter writer = null)
        {
            return new HostFile(reader ?? CreateReaderFor(new string[] { }),
                                writer ?? Substitute.For<ITextFileWriter>());
        }

        private ITextFileReader CreateReaderFor(params string[] lines)
        {
            var reader = Substitute.For<ITextFileReader>();
            reader.SetData(lines);
            return reader;
        }
    }
}
