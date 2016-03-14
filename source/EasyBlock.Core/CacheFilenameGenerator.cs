using System;
using System.IO;
using System.Reflection;
using PeanutButter.Utils;

namespace EasyBlock.Core
{
    public interface ICacheFilenameGenerator
    {
        string GenerateFor(string sourceUrl);
    }

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