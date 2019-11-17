using System;
using System.IO;
using EasyBlock.Core.Interfaces.Caching;
using EasyBlock.Core.Interfaces.Settings;
using EasyBlock.Core.Interfaces.TextReader;

namespace EasyBlock.Core.Implementations.Caching
{
    public class BlocklistCacheManager: IBlocklistCacheManager
    {
        private readonly ICacheFilenameGenerator _cacheFilenameGenerator;
        private readonly ITextFileReaderFactory _readerFactory;
        private readonly ISettings _settings;

        public BlocklistCacheManager(
            ICacheFilenameGenerator cacheFilenameGenerator,
            ITextFileReaderFactory readerFactory,
            ISettings settings)
        {
            if (cacheFilenameGenerator == null) throw new ArgumentNullException(nameof(cacheFilenameGenerator));
            if (readerFactory == null) throw new ArgumentNullException(nameof(readerFactory));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _cacheFilenameGenerator = cacheFilenameGenerator;
            _readerFactory = readerFactory;
            _settings = settings;
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
