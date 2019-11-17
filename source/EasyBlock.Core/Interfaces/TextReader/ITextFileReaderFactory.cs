namespace EasyBlock.Core.Interfaces.TextReader
{
    public interface ITextFileReaderFactory
    {
        ITextFileReader Open(string path);
    }
}