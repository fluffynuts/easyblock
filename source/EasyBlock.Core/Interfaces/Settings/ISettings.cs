using System.Collections.Generic;

namespace EasyBlock.Core.Interfaces.IO.Settings
{
    public interface ISettings
    {
        int RefreshIntervalInMinutes { get; }
        string HostsFile { get; }
        string CacheFolder { get; }
        IEnumerable<string> Sources { get; }
        IEnumerable<string> Blacklist { get; }
        IEnumerable<string> Whitelist { get; }
        string RedirectIp { get; }
    }
}