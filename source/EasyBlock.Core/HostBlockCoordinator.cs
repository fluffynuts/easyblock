using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeanutButter.INIFile;
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

        public HostBlockCoordinator(ISettings settings,
                                    IFileDownloader fileDownloader,
                                    IHostFileFactory hostFileFactory,
                                    ITextFileReaderFactory textFileReaderFactory,
                                    ITextFileWriterFactory textFileWriterFactory,
                                    IBlocklistCacheManager blocklistCacheManager)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (fileDownloader == null) throw new ArgumentNullException(nameof(fileDownloader));
            if (hostFileFactory == null) throw new ArgumentNullException(nameof(hostFileFactory));
            if (textFileReaderFactory == null) throw new ArgumentNullException(nameof(textFileReaderFactory));
            if (textFileWriterFactory == null) throw new ArgumentNullException(nameof(textFileWriterFactory));
            if (blocklistCacheManager == null) throw new ArgumentNullException(nameof(blocklistCacheManager));
            _settings = settings;
            _fileDownloader = fileDownloader;
            _hostFileFactory = hostFileFactory;
            _textFileReaderFactory = textFileReaderFactory;
            _textFileWriterFactory = textFileWriterFactory;
            _blocklistCacheManager = blocklistCacheManager;
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
        }

        private void UpdateBlocklists()
        {
            var blockLists = DownloadBlocklists();
            Cache(blockLists);
        }

        private void ApplyWhitelistOn(IHostFile hostFile)
        {
            _settings.Whitelist.ForEach(regex => hostFile.Whitelist(regex));
        }

        private void ApplyBlacklistOn(IHostFile hostFile)
        {
            _settings.Blacklist.ForEach(b => hostFile.Redirect(b, _settings.RedirectIp));
        }

        private void OverrideRedirectWithUserPreferenceOn(IHostFile hostFile)
        {
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
            hostFile.Merge(cachedHosts);
        }

        private void Cache(IDownloadResult[] blockLists)
        {
            blockLists.Where(b => b.Success)
                .ForEach(b => _blocklistCacheManager.Set(b.Url, b.Data));
        }

        private IDownloadResult[] DownloadBlocklists()
        {
            var tasks = _settings
                            .Sources
                            .Aggregate(new List<Task<IDownloadResult>>(), (accumulator, url) =>
                                    {
                                        accumulator.Add(_fileDownloader.DownloadDataAsync(url));
                                        return accumulator;
                                    });
            var waitOn = tasks.Cast<Task>().ToArray();
            Task.WaitAll(waitOn);
            return tasks.Select(t => t.Result).ToArray();
        }
    }
}
