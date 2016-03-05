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
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IFileDownloader _fileDownloader;
        private readonly IHostFileFactory _hostFileFactory;
        private readonly ITextFileReaderFactory _textFileReaderFactory;
        private readonly ITextFileWriterFactory _textFileWriterFactory;
        private readonly IBlocklistCacheManager _blocklistCacheManager;

        public HostBlockCoordinator(IApplicationConfiguration applicationConfiguration,
                                    IFileDownloader fileDownloader,
                                    IHostFileFactory hostFileFactory,
                                    ITextFileReaderFactory textFileReaderFactory,
                                    ITextFileWriterFactory textFileWriterFactory,
                                    IBlocklistCacheManager blocklistCacheManager)
        {
            if (applicationConfiguration == null) throw new ArgumentNullException(nameof(applicationConfiguration));
            if (fileDownloader == null) throw new ArgumentNullException(nameof(fileDownloader));
            if (hostFileFactory == null) throw new ArgumentNullException(nameof(hostFileFactory));
            if (textFileReaderFactory == null) throw new ArgumentNullException(nameof(textFileReaderFactory));
            if (textFileWriterFactory == null) throw new ArgumentNullException(nameof(textFileWriterFactory));
            if (blocklistCacheManager == null) throw new ArgumentNullException(nameof(blocklistCacheManager));
            _applicationConfiguration = applicationConfiguration;
            _fileDownloader = fileDownloader;
            _hostFileFactory = hostFileFactory;
            _textFileReaderFactory = textFileReaderFactory;
            _textFileWriterFactory = textFileWriterFactory;
            _blocklistCacheManager = blocklistCacheManager;
        }

        public void Apply()
        {
            var blockLists = DownloadBlocklists();
            Cache(blockLists);
            var hostFilePath = _applicationConfiguration.HostsFile;
            var reader = _textFileReaderFactory.Open(hostFilePath);
            var writer = _textFileWriterFactory.Open(hostFilePath);
            var hostFile = _hostFileFactory.Create(reader, writer);
            _applicationConfiguration.Sources.ForEach(url => Merge(url, hostFile));
            hostFile.Persist();
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
            var tasks = _applicationConfiguration
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

        public void Unapply()
        {
            throw new NotImplementedException();
        }
    }
}
