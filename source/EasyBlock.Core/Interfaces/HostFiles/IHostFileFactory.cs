using EasyBlock.Core.Interfaces.TextReader;
using EasyBlock.Core.Interfaces.TextWriter;

namespace EasyBlock.Core.Interfaces.HostFiles
{
    public interface IHostFileFactory
    {
        IHostFile Create(ITextFileReader reader, ITextFileWriter writer);
    }
}