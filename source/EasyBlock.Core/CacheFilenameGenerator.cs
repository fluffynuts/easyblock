using System;
using System.IO;
using PeanutButter.Utils;

namespace EasyBlock.Core
{
    public interface ICacheFilenameGenerator
    {
        string GenerateFor(string sourceUrl);
    }

    public class CacheFilenameGenerator: ICacheFilenameGenerator
    {
        private readonly IApplicationConfiguration _applicationConfiguration;

        public CacheFilenameGenerator(IApplicationConfiguration applicationConfiguration)
        {
            if (applicationConfiguration == null) throw new ArgumentNullException(nameof(applicationConfiguration));
            _applicationConfiguration = applicationConfiguration;
        }

        public string GenerateFor(string sourceUrl)
        {
            var cacheFile = sourceUrl?.AsBytes()?.ToMD5String();
            if (cacheFile == null)
                return null;
            return Path.Combine(_applicationConfiguration.CacheFolder, cacheFile);
        }
    }
}