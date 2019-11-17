using EasyBlock.Core.Interfaces.TextReader;

namespace EasyBlock.Core.Implementations.TextReader
{
    public class TextFileReaderFactory: ITextFileReaderFactory
    {
        public ITextFileReader Open(string path)
        {
            return new TextFileReader(path);
        }
    }
}
