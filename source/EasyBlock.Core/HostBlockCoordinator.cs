using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PeanutButter.Utils;

namespace EasyBlock.Core
{
    public interface IHostBlockCoordinator
    {
        void Apply();
        void Unapply();
    }

    public class HostBlockCoordinator: IHostBlockCoordinator
    {
        private readonly ISettings _settings;
        private readonly IFileDownloader _fileDownloader;
        private readonly IHostFileFactory _hostFileFactory;
        private readonly ITextFileReaderFactory _textFileReaderFactory;
        private readonly ITextFileWriterFactory _textFileWriterFactory;
        private readonly IBlocklistCacheManager _blocklistCacheManager;
        private readonly ISimpleLoggerFacade _logger;

        public HostBlockCoordinator(ISettings settings,
                                    IFileDownloader fileDownloader,
                                    IHostFileFactory hostFileFactory,
                                    ITextFileReaderFactory textFileReaderFactory,
                                    ITextFileWriterFactory textFileWriterFactory,
                                    IBlocklistCacheManager blocklistCacheManager,
                                    ISimpleLoggerFacade logger)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (fileDownloader == null) throw new ArgumentNullException(nameof(fileDownloader));
            if (hostFileFactory == null) throw new ArgumentNullException(nameof(hostFileFactory));
            if (textFileReaderFactory == null) throw new ArgumentNullException(nameof(textFileReaderFactory));
            if (textFileWriterFactory == null) throw new ArgumentNullException(nameof(textFileWriterFactory));
            if (blocklistCacheManager == null) throw new ArgumentNullException(nameof(blocklistCacheManager));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _settings = settings;
            _fileDownloader = fileDownloader;
            _hostFileFactory = hostFileFactory;
            _textFileReaderFactory = textFileReaderFactory;
            _textFileWriterFactory = textFileWriterFactory;
            _blocklistCacheManager = blocklistCacheManager;
            _logger = logger;
        }

        public void Apply()
        {
            UpdateBlocklists();
            ApplyBlocklists();
        }

        public void Unapply()
        {
            var hostFile = OpenHostsFile();
            hostFile.Revert();
            hostFile.Persist();
        }

        private void ApplyBlocklists()
        {
            var hostFile = OpenHostsFile();
            MergeBlocklistsInto(hostFile);
            OverrideRedirectWithUserPreferenceOn(hostFile);
            ApplyBlacklistOn(hostFile);
            ApplyWhitelistOn(hostFile);
            hostFile.Persist();
            _logger.LogInfo($"Wrote out hosts file to {_settings.HostsFile}");
            _logger.LogInfo($" -> installed {hostFile.Lines.Count(l => !l.IsPrimary && !l.IsComment)} blocked hosts");
        }

        private void UpdateBlocklists()
        {
            var blockLists = DownloadBlocklists();
            Cache(blockLists);
        }

        private void ApplyWhitelistOn(IHostFile hostFile)
        {
            _logger.LogInfo($"Applying {_settings.Whitelist.Count()} whitelist hosts");
            _settings.Whitelist.ForEach(hostFile.Whitelist);
        }

        private void ApplyBlacklistOn(IHostFile hostFile)
        {
            _logger.LogInfo($"Applying {_settings.Blacklist.Count()} blacklist hosts");
            _settings.Blacklist.ForEach(b => hostFile.Redirect(b, _settings.RedirectIp));
        }

        private void OverrideRedirectWithUserPreferenceOn(IHostFile hostFile)
        {
            _logger.LogInfo($"Redirecting adserver hosts to {_settings.RedirectIp}");
            hostFile.SetRedirectIp(_settings.RedirectIp);
        }

        private void MergeBlocklistsInto(IHostFile hostFile)
        {
            _settings.Sources.ForEach(url => Merge(url, hostFile));
        }

        private IHostFile OpenHostsFile()
        {
            var reader = GetHostsFileReader();
            var writer = GetHostsFileWriter();
            var hostFile = _hostFileFactory.Create(reader, writer);
            return hostFile;
        }

        private ITextFileReader GetHostsFileReader()
        {
            return _textFileReaderFactory.Open(_settings.HostsFile);
        }

        private ITextFileWriter GetHostsFileWriter()
        {
            return _textFileWriterFactory.Open(_settings.HostsFile);
        }

        private void Merge(string url, IHostFile hostFile)
        {
            var cachedHosts = _blocklistCacheManager.GetReaderFor(url);
            if (cachedHosts == null)
            {
                _logger.LogWarning($"Unable to retrieve cached data for {url}");
                return;
            }

            _logger.LogDebug($"Retrieved cached data for {url}");
            hostFile.Merge(cachedHosts);
        }

        private void Cache(IDownloadResult[] blockLists)
        {
            blockLists.Where(b => b != null && b.Success)
                .ForEach(b => _blocklistCacheManager.Set(b.Url, b.Data));
        }

        private IDownloadResult[] DownloadBlocklists()
        {
            var tasks = _settings
                            .Sources
                            .Aggregate(new List<Task<IDownloadResult>>(), (accumulator, url) =>
                                    {
                                        _logger.LogInfo($"Downloading hosts file: {url}");
                                        accumulator.Add(_fileDownloader.DownloadDataAsync(url));
                                        return accumulator;
                                    });
            var waitOn = tasks.Cast<Task>().ToArray();
            Task.WaitAll(waitOn);
            var results = tasks.Select(t => t.Result).ToArray();
            LogDownloadResults(results);
            return results;
        }

        private void LogDownloadResults(IDownloadResult[] results)
        {
            results.ForEach(result =>
            {
                if (result == null)
                {
                    _logger.LogWarning("null result encountered");
                    return;
                }
                if (result.Success)
                    _logger.LogInfo($"Successful download: {result.Url}");
                else
                    _logger.LogWarning($"Failed download: {result.Url} ({result.FailureException?.Message})");
            });
        }
    }
}
