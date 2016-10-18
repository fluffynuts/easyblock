using EasyBlock.Core.Interfaces.IO.HostFiles;
using EasyBlock.Core.Interfaces.IO.TextReader;
using EasyBlock.Core.Interfaces.IO.TextWriter;

namespace EasyBlock.Core.Implementations.IO.HostFiles
{
    public class HostFileFactory: IHostFileFactory
    {
        public IHostFile Create(ITextFileReader reader, ITextFileWriter writer)
        {
            return new HostFile(reader, writer);
        }
    }
}
