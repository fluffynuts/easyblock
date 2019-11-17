using EasyBlock.Core.Interfaces.HostFiles;
using EasyBlock.Core.Interfaces.TextReader;
using EasyBlock.Core.Interfaces.TextWriter;

namespace EasyBlock.Core.Implementations.HostFiles
{
    public class HostFileFactory: IHostFileFactory
    {
        public IHostFile Create(ITextFileReader reader, ITextFileWriter writer)
        {
            return new HostFile(reader, writer);
        }
    }
}
