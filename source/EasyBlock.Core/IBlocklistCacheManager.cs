namespace EasyBlock.Core
{
    public interface IBlocklistCacheManager
    {
        void Set(string source, byte[] data);
        byte[] Get(string source);
    }
}