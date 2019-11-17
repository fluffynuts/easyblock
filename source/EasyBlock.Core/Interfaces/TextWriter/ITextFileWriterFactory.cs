namespace EasyBlock.Core.Interfaces.TextWriter
{
    public interface ITextFileWriterFactory
    {
        ITextFileWriter Open(string path);
    }
}