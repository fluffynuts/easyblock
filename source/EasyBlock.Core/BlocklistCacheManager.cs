using System;
using System.IO;

namespace EasyBlock.Core
{
    public class BlocklistCacheManager: IBlocklistCacheManager
    {
        private readonly ICacheFilenameGenerator _cacheFilenameGenerator;
        private readonly ITextFileReaderFactory _readerFactory;

        public BlocklistCacheManager(ICacheFilenameGenerator cacheFilenameGenerator,
                                      ITextFileReaderFactory readerFactory)
        {
            if (cacheFilenameGenerator == null) throw new ArgumentNullException(nameof(cacheFilenameGenerator));
            if (readerFactory == null) throw new ArgumentNullException(nameof(readerFactory));
            _cacheFilenameGenerator = cacheFilenameGenerator;
            _readerFactory = readerFactory;
        }

        public void Set(string source, byte[] data)
        {
            var path = _cacheFilenameGenerator.GenerateFor(source);
            var targetFolder = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(targetFolder) && !Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);
            File.WriteAllBytes(path, data);
        }

        public ITextFileReader GetReaderFor(string source)
        {
            var cacheFilepath = _cacheFilenameGenerator.GenerateFor(source);
            return File.Exists(cacheFilepath)
                        ? _readerFactory.Open(cacheFilepath)
                        : null;
        }

    }

}
