namespace EasyBlock.Core.Interfaces.TextWriter
{
    public interface ITextFileWriter
    {
        void AppendLine(string line);
        void Persist();
    }
}
