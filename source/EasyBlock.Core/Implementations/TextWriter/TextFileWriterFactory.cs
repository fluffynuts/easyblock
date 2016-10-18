using EasyBlock.Core.Interfaces.IO.TextWriter;

namespace EasyBlock.Core.Implementations.IO.TextWriter
{
    public class TextFileWriterFactory: ITextFileWriterFactory
    {
        public ITextFileWriter Open(string path)
        {
            return new TextFileWriter(path);
        }
    }
}
