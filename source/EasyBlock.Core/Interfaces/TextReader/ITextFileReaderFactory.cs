namespace EasyBlock.Core.Interfaces.IO.TextReader
{
    public interface ITextFileReaderFactory
    {
        ITextFileReader Open(string path);
    }
}