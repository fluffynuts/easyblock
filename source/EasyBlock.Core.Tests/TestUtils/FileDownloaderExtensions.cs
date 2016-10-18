using System.Threading.Tasks;
using EasyBlock.Core.Interfaces.Downloading;
using NSubstitute;
// ReSharper disable MemberCanBePrivate.Global

namespace EasyBlock.Core.Tests
{
    public static class FileDownloaderExtensions
    {
        public static void SetDownloadResult(this IFileDownloader downloader, string url, byte[] data)
        {
            var downloadResult = Substitute.For<IDownloadResult>();
            downloadResult.Success.Returns(true);
            downloadResult.Data.Returns(data);
            downloadResult.Url.Returns(url);
            downloader.SetDownloadResult(url, downloadResult);
        }

        public static void SetDownloadResult(this IFileDownloader downloader, string url, IDownloadResult downloadResult)
        {
            downloader.DownloadDataAsync(url).Returns(Task.Run(() => downloadResult));
        }

        public static void SetFailedDownloadResultFor(this IFileDownloader downloader, string url)
        {
            var downloadResult = Substitute.For<IDownloadResult>();
            downloadResult.Url.Returns(url);
            downloadResult.Success.Returns(false);
            downloadResult.Data.Returns((byte[])null);
            downloader.SetDownloadResult(url, downloadResult);
        }
    }
}