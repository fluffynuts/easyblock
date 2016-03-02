using System;
using PeanutButter.INIFile;

namespace EasyBlock.Core
{
    public interface IBlocklistCacheManager
    {
        void Cache(string source, byte[] data);
        byte[] Find(string source);
    }

    public class BlocklistCacheManager: IBlocklistCacheManager
    {
        private readonly ICacheFilenameGenerator _cacheFilenameGenerator;

        public BlocklistCacheManager(ICacheFilenameGenerator cacheFilenameGenerator)
        {
            if (cacheFilenameGenerator == null) throw new ArgumentNullException(nameof(cacheFilenameGenerator));
            _cacheFilenameGenerator = cacheFilenameGenerator;
        }

        public void Cache(string source, byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] Find(string source)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICacheFilenameGenerator
    {
        string GenerateFor(string sourceUrl);
    }

    public class CacheFilenameGenerator
    {
        private readonly IINIFile _iniFile;

        public CacheFilenameGenerator(IINIFile iniFile)
        {
            if (iniFile == null) throw new ArgumentNullException(nameof(iniFile));
            _iniFile = iniFile;
        }
    }
}
