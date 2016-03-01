using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeanutButter.INIFile;

namespace EasyBlock.Core
{
    public interface IHostBlockCoordinator
    {
        void Apply();
        void Unapply();
    }

    public class HostBlockCoordinator: IHostBlockCoordinator
    {
        private readonly IINIFile _iniFile;
        private readonly IFileDownloader _fileDownloader;
        private readonly IHostFileFactory _hostFileFactory;
        private readonly ITextFileReaderFactory _textFileReaderFactory;
        private readonly ITextFileWriterFactory _textFileWriterFactory;

        public HostBlockCoordinator(IINIFile iniFile,
                                    IFileDownloader fileDownloader,
                                    IHostFileFactory hostFileFactory,
                                    ITextFileReaderFactory textFileReaderFactory,
                                    ITextFileWriterFactory textFileWriterFactory)
        {
            if (iniFile == null) throw new ArgumentNullException(nameof(iniFile));
            if (fileDownloader == null) throw new ArgumentNullException(nameof(fileDownloader));
            if (hostFileFactory == null) throw new ArgumentNullException(nameof(hostFileFactory));
            if (textFileReaderFactory == null) throw new ArgumentNullException(nameof(textFileReaderFactory));
            if (textFileWriterFactory == null) throw new ArgumentNullException(nameof(textFileWriterFactory));
            _iniFile = iniFile;
            _fileDownloader = fileDownloader;
            _hostFileFactory = hostFileFactory;
            _textFileReaderFactory = textFileReaderFactory;
            _textFileWriterFactory = textFileWriterFactory;
        }

        public void Apply()
        {
            var blockLists = DownloadBlocklists();
        }

        private IDownloadResult[] DownloadBlocklists()
        {
            var sourceUrls = _iniFile["sources"].Keys.ToArray();
            var tasks = sourceUrls.Aggregate(new List<Task<IDownloadResult>>(), (accumulator, url) =>
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
