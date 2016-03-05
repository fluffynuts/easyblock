using System;

namespace EasyBlock.Core
{
    public class BlocklistCacheManager: IBlocklistCacheManager
    {
        private readonly ICacheFilenameGenerator _cacheFilenameGenerator;

        public BlocklistCacheManager(ICacheFilenameGenerator cacheFilenameGenerator)
        {
            if (cacheFilenameGenerator == null) throw new ArgumentNullException(nameof(cacheFilenameGenerator));
            _cacheFilenameGenerator = cacheFilenameGenerator;
        }

        public void Set(string source, byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] Get(string source)
        {
            throw new NotImplementedException();
        }
    }

}
