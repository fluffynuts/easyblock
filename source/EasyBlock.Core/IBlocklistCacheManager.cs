namespace EasyBlock.Core
{
    public interface IBlocklistCacheManager
    {
        void Set(string source, byte[] data);
        ITextFileReader GetReaderFor(string source);
    }
}