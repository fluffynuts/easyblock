using EasyBlock.Core.Interfaces.IO.TextReader;

namespace EasyBlock.Core.Implementations.IO.TextReader
{
    public class TextFileReaderFactory: ITextFileReaderFactory
    {
        public ITextFileReader Open(string path)
        {
            return new TextFileReader(path);
        }
    }
}
