using EasyBlock.Core.Interfaces.TextReader;

namespace EasyBlock.Core.Interfaces.Caching
{
    public interface IBlocklistCacheManager
    {
        void Set(string source, byte[] data);
        ITextFileReader GetReaderFor(string source);
    }
}