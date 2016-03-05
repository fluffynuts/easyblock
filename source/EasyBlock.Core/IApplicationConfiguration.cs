using System.Collections.Generic;

namespace EasyBlock.Core
{
    public interface IApplicationConfiguration
    {
        int RefreshIntervalInMinutes { get; }
        string HostsFile { get; }
        string CacheFolder { get; }
        IEnumerable<string> Blacklist { get; }
        IEnumerable<string> Whitelist { get; }
    }
}