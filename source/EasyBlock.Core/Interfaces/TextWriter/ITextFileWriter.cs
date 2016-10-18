namespace EasyBlock.Core.Interfaces.IO.TextWriter
{
    public interface ITextFileWriter
    {
        void AppendLine(string line);
        void Persist();
    }
}
