using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PeanutButter.Utils;

namespace EasyBlock.Core
{
    public interface IDownloadResult
    {
        string Url { get; }
        string OutputPath { get; }
        int DownloadSize { get; }
        bool Success { get; }
        Exception FailureException { get; }
    }

    public class DownloadResult: IDownloadResult
    {
        public string Url { get; set; }
        public string OutputPath { get; set; }
        public int DownloadSize { get; set; }
        public bool Success { get; set; }
        public Exception FailureException { get; set; }
    }

    public interface IFileDownloader
    {
        Task<IDownloadResult> DownloadFileAsync(string url, string outputPath);
    }

    public class FileDownloader: IFileDownloader
    {
        public Task<IDownloadResult> DownloadFileAsync(string url, string outputPath)
        {
            return Task.Run<IDownloadResult>(async () =>
            {
                var result = new DownloadResult()
                {
                    Url = url,
                    OutputPath = outputPath
                };

                var client = new WebClient();
                try
                {
                    var downloadBytes = await client.DownloadDataTaskAsync(url);
                    File.WriteAllBytes(outputPath, downloadBytes);
                    result.DownloadSize = downloadBytes.Length;
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.FailureException = ex;
                }
                return result;
            });
        }
    }
}
