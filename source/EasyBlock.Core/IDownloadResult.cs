using System;

namespace EasyBlock.Core
{
    public interface IDownloadResult
    {
        string Url { get; }
        byte[] Data { get; set; }
        bool Success { get; }
        Exception FailureException { get; }
    }
}