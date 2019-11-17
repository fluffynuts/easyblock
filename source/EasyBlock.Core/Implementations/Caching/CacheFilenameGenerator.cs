using System;
using System.IO;
using EasyBlock.Core.Interfaces.Caching;
using EasyBlock.Core.Interfaces.Settings;
using PeanutButter.Utils;

namespace EasyBlock.Core.Implementations.Caching
{
    public class CacheFilenameGenerator: ICacheFilenameGenerator
    {
        private readonly ISettings _settings;

        public CacheFilenameGenerator(ISettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
        }

        public string GenerateFor(string sourceUrl)
        {
            var cacheFile = sourceUrl?.AsBytes()?.ToMD5String();
            if (cacheFile == null)
                return null;
            return Path.Combine(CacheFolder, cacheFile);
        }

        private string CacheFolder => Path.IsPathRooted(_settings.CacheFolder)
                                        ? _settings.CacheFolder
                                        : Path.Combine(ExecutingAssemblyPathFinder.GetExecutingAssemblyFolder(), _settings.CacheFolder);
    }

}