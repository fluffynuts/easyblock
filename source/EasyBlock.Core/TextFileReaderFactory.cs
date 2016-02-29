namespace EasyBlock.Core
{
    public interface ITextFileReaderFactory
    {
        ITextFileReader Open(string path);
    }

    public class TextFileReaderFactory: ITextFileReaderFactory
    {
        public ITextFileReader Open(string path)
        {
            return new TextFileReader(path);
        }
    }
}
