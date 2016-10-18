using System;
using EasyBlock.Core.Interfaces.Downloading;

namespace EasyBlock.Core.Implementations.Downloading
{
    public class DownloadResult: IDownloadResult
    {
        public string Url { get; set; }
        public byte[] Data { get; set; }
        public bool Success { get; set; }
        public Exception FailureException { get; set; }
    }
}