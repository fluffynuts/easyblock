namespace EasyBlock.Core
{
    public interface ITextFileWriterFactory
    {
        ITextFileWriter Open(string path);
    }

    public class TextFileWriterFactory: ITextFileWriterFactory
    {
        public ITextFileWriter Open(string path)
        {
            return new TextFileWriter(path);
        }
    }
}
