﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EasyBlock.Core.Extensions;
using EasyBlock.Core.Interfaces.HostFiles;
using EasyBlock.Core.Interfaces.TextReader;
using EasyBlock.Core.Interfaces.TextWriter;
using PeanutButter.Utils;

namespace EasyBlock.Core.Implementations.HostFiles
{
    public class HostFile: IHostFile
    {
        public IEnumerable<IHostFileLine> Lines => _lines;
        private readonly List<IHostFileLine> _lines = new List<IHostFileLine>();
        private readonly ITextFileWriter _writer;
        private readonly HashSet<string> _knownHosts = new HashSet<string>(); 

        public HostFile(ITextFileReader reader, ITextFileWriter writer)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            _writer = writer;
            LoadFrom(reader);
        }

        public void Merge(ITextFileReader reader)
        {
            reader.EnumerateLines()
                .ForEach(AddMergeLine);
        }

        public void Persist()
        {
            PersistPrimaryLines();
            if (_lines.Any(l => !l.IsPrimary))
            {
                PersistMarker();
                PersistMergeLines();
            }
            _writer.Persist();
        }

        public void SetRedirectIp(string redirectIp)
        {
            Lines.Where(l => !l.IsPrimary)
                    .ForEach(l => l.IPAddress = redirectIp);
        }

        public void Redirect(string host, string ip)
        {
            AddMergeLine($"{ip} {host}");
        }

        public void Whitelist(string regex)
        {
            var re = new Regex(regex);
            _lines.RemoveAll(l => !l.IsPrimary &&
                                    re.IsMatch(l.HostName));
        }

        public void Revert()
        {
            _lines.RemoveAll(l => !l.IsPrimary);
        }

        private void PersistMergeLines()
        {
            _lines.Where(l => !l.IsPrimary)
                    .ForEach(AppendMergeLine);
        }

        private void AppendMergeLine(IHostFileLine line)
        {
            _writer.AppendLine(line.IsComment
                                ? line.Data
                                :$"{line.IPAddress}\t{line.HostName}");
        }

        private void PersistMarker()
        {
            _writer.AppendLine(string.Empty);
            _writer.AppendLine(Constants.MERGE_MARKER);
        }

        private void PersistPrimaryLines()
        {
            _lines.Where(l => l.IsPrimary)
                    .ForEach(l => _writer.AppendLine(l.Data));
        }

        private void LoadFrom(ITextFileReader reader)
        {
            var inMergeSection = false;
            var mergeMarger = Constants.MERGE_MARKER.ToLower().Trim().Replace(" ", string.Empty);
            reader.EnumerateLines()
                .ForEach(line =>
                {
                    if (inMergeSection && string.IsNullOrWhiteSpace(line))
                        return;
                    if (line.Trim().ToLower().Replace(" ", string.Empty) == mergeMarger)
                    {
                        inMergeSection = true;
                        return;
                    }
                    if (inMergeSection)
                        AddMergeLine(line);
                    else
                        AddPrimaryLine(line);
                });
            reader.Dispose();
        }

        private void AddMergeLine(string line)
        {
            AddLine(line, false, true);
        }

        private void AddPrimaryLine(string line)
        {
            AddLine(line, true, false);
        }

        private void AddLine(string line, bool isPrimary, bool skipIfComment)
        {
            var hostFileLine = new HostFileLine(line, isPrimary);
            if ((hostFileLine.IsComment && skipIfComment) ||
                (!isPrimary && AlreadyHaveLineForHostOf(hostFileLine)))
                return;
            _knownHosts.Add(hostFileLine.HostName.ToLowerInvariant());
            _lines.Add(hostFileLine);
        }

        private bool AlreadyHaveLineForHostOf(HostFileLine hostFileLine)
        {
            var lowerHost = hostFileLine?.HostName?.ToLowerInvariant();
            if (_knownHosts.Contains(lowerHost))
                return true;
            _knownHosts.Add(lowerHost);
            return false;
        }
    }
}
