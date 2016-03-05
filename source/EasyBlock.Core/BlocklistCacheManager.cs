using System;
using System.IO;

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
            var path = _cacheFilenameGenerator.GenerateFor(source);
            File.WriteAllBytes(path, data);
        }

        public byte[] Get(string source)
        {
            var cacheFilepath = _cacheFilenameGenerator.GenerateFor(source);
            return File.Exists(cacheFilepath)
                        ? File.ReadAllBytes(cacheFilepath)
                        : null;
        }
    }

}
