using System;
using System.Linq;

namespace EasyBlock.Core
{
    public interface IHostFileLine
    {
        string Data { get; }
        bool IsComment { get; }
        // ReSharper disable once InconsistentNaming
        string IPAddress { get; }
        string HostName { get; }
        bool IsPrimary { get; }
    }

    public class HostFileLine: IHostFileLine
    {
        public string Data { get; private set; } = string.Empty;
        public bool IsComment { get; private set;  }
        public string IPAddress { get; private set; } = string.Empty;
        public string HostName { get; private set;  } = string.Empty;
        public bool IsPrimary { get; private set; }

        public HostFileLine(string line, bool isPrimary)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;
            IsPrimary = isPrimary;
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

        public override bool Equals(object obj)
        {
            var x = obj as HostFileLine;
            if (x == null) return false;
            return x.IsComment == IsComment &&
                    x.Data == Data &&
                    x.HostName == HostName &&
                    x.IPAddress == IPAddress;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                var hashCode = Data?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ IsComment.GetHashCode();
                hashCode = (hashCode*397) ^ (IPAddress?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (HostName?.GetHashCode() ?? 0);
                return hashCode;
                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }
    }
}
