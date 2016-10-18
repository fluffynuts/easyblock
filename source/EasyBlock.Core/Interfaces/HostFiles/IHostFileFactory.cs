using EasyBlock.Core.Interfaces.IO.TextReader;
using EasyBlock.Core.Interfaces.IO.TextWriter;

namespace EasyBlock.Core.Interfaces.IO.HostFiles
{
    public interface IHostFileFactory
    {
        IHostFile Create(ITextFileReader reader, ITextFileWriter writer);
    }
}