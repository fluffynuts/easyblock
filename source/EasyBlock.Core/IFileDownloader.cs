using System.Threading.Tasks;

namespace EasyBlock.Core
{
    public interface IFileDownloader
    {
        Task<IDownloadResult> DownloadDataAsync(string url);
    }
}