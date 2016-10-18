namespace EasyBlock.Core.Interfaces.IO.HostFiles
{
    public interface IHostFileLine
    {
        string Data { get; }
        bool IsComment { get; }
        // ReSharper disable once InconsistentNaming
        string IPAddress { get; set; }
        string HostName { get; }
        bool IsPrimary { get; }
    }
}