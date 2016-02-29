namespace EasyBlock.Core
{
    public interface IHostFileFactory
    {
        IHostFile Create(ITextFileReader reader, ITextFileWriter writer);
    }
    public class HostFileFactory: IHostFileFactory
    {
        public IHostFile Create(ITextFileReader reader, ITextFileWriter writer)
        {
            return new HostFile(reader, writer);
        }
    }
}
