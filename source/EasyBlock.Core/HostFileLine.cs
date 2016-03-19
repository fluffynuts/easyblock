using System;
using System.Linq;

namespace EasyBlock.Core
{
    public interface IHostFileLine
    {
        string Data { get; }
        bool IsComment { get; }
        // ReSharper disable once InconsistentNaming
        string IPAddress { get; set; }
        string HostName { get; }
        bool IsPrimary { get; }
    }

    public class HostFileLine: IHostFileLine
    {
        public string Data { get; private set; } = string.Empty;
        public bool IsComment { get; private set;  }
        public string IPAddress { get; set; } = string.Empty;
        public string HostName { get; private set;  } = string.Empty;
        public bool IsPrimary { get; private set; }

        public HostFileLine(string line, bool isPrimary)
        {
            IsPrimary = isPrimary;
            if (string.IsNullOrWhiteSpace(line))
                return;
            Grok(line);
        }

        private void Grok(string line)
        {
            Data = line;
            if (line.Trim().StartsWith("#"))
            {
                IsComment = true;
                return;
            }
            var parts = line.Split(new[] {" ", "\t"}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                IPAddress = parts.First();
                HostName = parts.Skip(1).First();
            }
        }

    }
}
