using EasyBlock.Core.Interfaces.TextWriter;

namespace EasyBlock.Core.Implementations.TextWriter
{
    public class TextFileWriterFactory: ITextFileWriterFactory
    {
        public ITextFileWriter Open(string path)
        {
            return new TextFileWriter(path);
        }
    }
}
