using System.Threading.Tasks;

namespace EasyBlock.Core.Interfaces.Downloading
{
    public interface IFileDownloader
    {
        Task<IDownloadResult> DownloadDataAsync(string url);
    }
}