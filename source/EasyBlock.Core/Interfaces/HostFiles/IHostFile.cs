using System.Collections.Generic;
using EasyBlock.Core.Interfaces.TextReader;

namespace EasyBlock.Core.Interfaces.HostFiles
{
    public interface IHostFile
    {
        IEnumerable<IHostFileLine> Lines { get; }
        void Merge(ITextFileReader reader);
        void Persist();
        void SetRedirectIp(string redirectIp);
        void Redirect(string host, string ip);
        void Whitelist(string regex);
        void Revert();
    }
}