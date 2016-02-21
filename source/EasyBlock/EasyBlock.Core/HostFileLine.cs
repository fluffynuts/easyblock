using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBlock.Core
{
    public interface IHostFileLine
    {
        string Data { get; }
        bool IsComment { get; }
        string IPAddress { get; }
        string HostName { get; }
    }

    public class HostFileLine: IHostFileLine
    {
        public string Data { get; private set; } = string.Empty;
        public bool IsComment { get; private set; }
        public string IPAddress { get; private set; } = string.Empty;
        public string HostName { get; private set; } = string.Empty;

        public HostFileLine(string line)
        {
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
            if (parts.Count() > 1)
            {
                IPAddress = parts.First();
                HostName = parts.Skip(1).First();
            }
        }
    }
}
