namespace EasyBlock.Core.Interfaces.IO.TextWriter
{
    public interface ITextFileWriterFactory
    {
        ITextFileWriter Open(string path);
    }
}