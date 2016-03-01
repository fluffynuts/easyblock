using System;
using System.Net;
using System.Threading.Tasks;

namespace EasyBlock.Core
{
    public class FileDownloader: IFileDownloader
    {
        public Task<IDownloadResult> DownloadDataAsync(string url)
        {
            return Task.Run<IDownloadResult>(async () =>
            {
                var result = new DownloadResult()
                {
                    Url = url,
                };

                var client = new WebClient();
                try
                {
                    result.Data = await client.DownloadDataTaskAsync(url);
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
